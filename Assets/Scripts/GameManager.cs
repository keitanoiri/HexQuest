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

    public static Tilemap[] groundLayers;   // ← 最上段から順に並べる(makemapにて)

    public static bool playerturn;//プレイヤーのターンか否か

    public static Vector2Int PlayerCombatInsex;//プレイヤーのアーツの位置

    /// <summary>
    /// Unitを辞書登録する
    /// </summary>
    /// <param name="u"></param>
    public static void RegisterUnit(Unit u)
    {
        AllUnits[u.ID] = u;
        UnitPos[u.CELL] = u.ID;

    }
    /// <summary>
    /// Unitを辞書から外す
    /// </summary>
    /// <param name="id"></param>
    public static void UmregisterUnit(int id)
    {
        UnitPos.Remove(AllUnits[id].CELL);        
        AllUnits.Remove(id);
        
    }

    private void Awake()
    {
        //プレイヤーを配置する
        Instantiate(playerprefab, parentGrid.transform);

        // StreamingAssets/Arts.csv を読む場合
        // webGLやMacの場合は別途処理が必要らしい？
        string path = Path.Combine(Application.streamingAssetsPath, "Arts.csv");
        ArtsDatabase.Load(path);

        //アーツの初期設定
        AllArts.Clear();
        AllArts.Add(new Vector2Int(0, 0), new Arts(ArtsDatabase.Get(1)));//リセット
        AllArts.Add(new Vector2Int(0, 1), new Arts(ArtsDatabase.Get(3)));//スラッシュ
        AllArts.Add(new Vector2Int(1, 0), new Arts(ArtsDatabase.Get(3)));//スラッシュ
        AllArts.Add(new Vector2Int(0, -1), new Arts(ArtsDatabase.Get(3)));//スラッシュ
        AllArts.Add(new Vector2Int(-1, 1), new Arts(ArtsDatabase.Get(4)));//ガード
        AllArts.Add(new Vector2Int(-1, 0), new Arts(ArtsDatabase.Get(4)));//ガード
        AllArts.Add(new Vector2Int(-1, -1), new Arts(ArtsDatabase.Get(4)));//ガード
            

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
            //ターンカウント

            //プレイヤーターン
            //while(
            //エネミーターン
            await UnitMoves();
        }
    }

    private async UniTask UnitMoves()
    {
        playerturn = true;
        await AllUnits[1].StartTurn();//プレイヤーの動き

        foreach (KeyValuePair<int, Unit> pair in AllUnits)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
            playerturn = false;
            if (pair.Key != 1)//プレイヤーでなければ
            {
                await pair.Value.StartTurn();//順番に処理
            }
        }
    }
}
