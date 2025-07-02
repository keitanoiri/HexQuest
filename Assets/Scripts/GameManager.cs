using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerprefab;
    [SerializeField] GameObject parentGrid;

    public static Dictionary<int, Unit> AllUnits = new();
    public static Dictionary<Vector3Int, int> UnitPos = new();
    public static Dictionary<Vector2Int, Arts> AllArts = new();

    public static Tilemap[] groundLayers;   // �� �ŏ�i���珇�ɕ��ׂ�(makemap�ɂ�)

    public static bool playerturn;//�v���C���[�̃^�[�����ۂ�

    public static Vector2Int PlayerCombatInsex;//�v���C���[�̃A�[�c�̈ʒu

    /// <summary>
    /// Unit�������o�^����
    /// </summary>
    /// <param name="u"></param>
    public static void RegisterUnit(Unit u)
    {
        AllUnits[u.ID] = u;
        UnitPos[u.CELL] = u.ID;

    }
    /// <summary>
    /// Unit����������O��
    /// </summary>
    /// <param name="id"></param>
    public static void UmregisterUnit(int id)
    {
        UnitPos.Remove(AllUnits[id].CELL);        
        AllUnits.Remove(id);
        
    }

    private void Awake()
    {
        //�v���C���[��z�u����
        Instantiate(playerprefab, parentGrid.transform);

        // StreamingAssets/Arts.csv ��ǂޏꍇ
        // webGL��Mac�̏ꍇ�͕ʓr�������K�v�炵���H
        string path = Path.Combine(Application.streamingAssetsPath, "Arts.csv");
        ArtsDatabase.Load(path);

        //�A�[�c�̏����ݒ�
        AllArts.Clear();
        AllArts.Add(new Vector2Int(0, 0), new Arts(ArtsDatabase.Get(1)));//���Z�b�g
        AllArts.Add(new Vector2Int(0, 1), new Arts(ArtsDatabase.Get(3)));//�X���b�V��
        AllArts.Add(new Vector2Int(1, 0), new Arts(ArtsDatabase.Get(3)));//�X���b�V��
        AllArts.Add(new Vector2Int(0, -1), new Arts(ArtsDatabase.Get(3)));//�X���b�V��
        AllArts.Add(new Vector2Int(-1, 1), new Arts(ArtsDatabase.Get(4)));//�K�[�h
        AllArts.Add(new Vector2Int(-1, 0), new Arts(ArtsDatabase.Get(4)));//�K�[�h
        AllArts.Add(new Vector2Int(-1, -1), new Arts(ArtsDatabase.Get(4)));//�K�[�h
            

    }

    // Start is called before the first frame update
    void Start()
    {
        GameRoop();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private async void GameRoop()
    {
        while (true)
        {
            //�^�[���J�E���g

            //�v���C���[�^�[��
            //while(
            //�G�l�~�[�^�[��
            await UnitMoves();
        }
    }

    private async UniTask UnitMoves()
    {
        playerturn = true;
        await AllUnits[1].StartTurn();//�v���C���[�̓���

        foreach (KeyValuePair<int, Unit> pair in AllUnits)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
            playerturn = false;
            if (pair.Key != 1)//�v���C���[�łȂ����
            {
                await pair.Value.StartTurn();//���Ԃɏ���
            }
        }
    }
}
