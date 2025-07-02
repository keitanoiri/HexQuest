
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;	//DOTween���g���Ƃ��͂���using������
using Cysharp.Threading.Tasks;
using UnityEditor;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.EventSystems;
using TMPro;
using ArtsHandlers;
using Unity.Collections.LowLevel.Unsafe;
using DG.Tweening.Core.Easing;
using static TerrainTile;

public class PlayerController : MonoBehaviour
{
    //����
    Camera cam;
    [SerializeField] Transform player;
    [SerializeField] TextMeshProUGUI APUI;
    [SerializeField] GameObject CombatPanel;



    [SerializeField] int MaxAP = 4;
    [SerializeField] float MoveSpeed = 1f;
    [SerializeField] GameObject serectEffect;

    bool moving = false;
    public static bool IsCombat = false;
    public Unit PlayerUnit = null;

    [SerializeField] PlayerAnimationController PlayerAnimationController;

    bool IsRefleshTiming = false;//�I�����Ă���Z�����ύX���ꂽ�Ƃ��Ȃǂ̍X�V�����݂񂮂������Ă����
    Vector3Int searchcell_;
    Vector3Int SerectCell//�}�E�X�ȂǂőI�𒆂̃Z��
    {
        get => searchcell_;
        set
        {
            if (searchcell_ == value) return;
            IsRefleshTiming = true;
            searchcell_ = value;
        }
    }


    int _ap;
    public int AP
    {
        get => _ap;
        set
        {
            if (value == _ap) return;
            MakeEffects.DestroyAllEffect();
            if (value < _ap)//�����AP���o���l�l���H
            {
                EXP += _ap - value;
            }

            _ap = value;
            APUI.text = value.ToString();
        }
    }
    int _exp;
    int EXP
    {
        get => _exp;
        set
        {
            if (value >= 20 + (LEVEL-1) * 2)
            {
                _exp = value - (20 + (LEVEL - 1) * 2);
                
                LEVEL++;
                //�V�A�[�c�l��
                getNewArts.gameObject.SetActive(true);
            }else _exp = value;
        }
    }

    int LEVEL;


    //�T�����[�h
    [SerializeField] GameObject ExplorePanel;
    List<Vector3Int> root = new List<Vector3Int>();
    static int _exproreIndex = 0;
    public static int ExproreIndex
    {
        get => _exproreIndex;//���[�h�̃T�C�N��
        set
        {

            if (_exproreIndex == value) return;


            if (value < 0) value = 5;
            else if (value > 5) value = 0;

            _exproreIndex = value;
        }
    }

    HexPathFinder pathF;

    GetNewArts getNewArts;

    private void Awake()
    {
        getNewArts = FindAnyObjectByType<GetNewArts>(FindObjectsInactive.Include);
        cam = Camera.main;
        pathF = GetComponent<HexPathFinder>();
    }

    private void Start()
    {
        AP = MaxAP;
    }

    void Update()
    {


        //�I�𒆂̃Z���́H
        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        if (HexUtil.TryWorldToCell(world, out Vector3Int cell)) SerectCell = cell;
        else return;

        //�I�����Ă��郁�j���[��؂�ւ��鏈��(����)
        ChengeIndex();

        if (EventSystem.current.IsPointerOverGameObject())//UI���쒆�͔��肵�Ȃ�
            return;

        if (GameManager.playerturn == false) return;

        //���[�h���ƂɈقȂ�U�镑��
        if (IsCombat) CombatMode();
        else ExproreMode();
    }

    public void StartTurn()
    {
        //�^�[���J�n���̏���
        AP = MaxAP;
    }
    private void ChengeIndex()
    {

        switch (Input.mouseScrollDelta.y)
        {
            case 0:
                return;
            case 1:
                MakeEffects.DestroyAllEffect();
                if (IsCombat) CombatIndex += 1;
                else ExproreIndex += 1;
                return;
            case -1:
                MakeEffects.DestroyAllEffect();
                if (IsCombat)
                {
                    Debug.Log("����");
                    CombatIndex -= 1;
                }
                else ExproreIndex -= 1;
                return;

        }
    }

    private void ExproreMode()
    {
        switch (ExproreIndex)
        {
            case 0:
                MoveMode();
                break;
            case 1:
                ModeChengeMode();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

        }
    }

    private async void MoveMode()
    {
        
        if (moving) return;
        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int searchedCell = new();
        if (HexUtil.TryWorldToCell(world, out searchedCell)) SerectCell = searchedCell;

        //���t���b�V���^�C�~���O�ł���΃p�X���X�V
        if (IsRefleshTiming)
        {
            //��U�G�t�F�N�g�͑S�ď���
            MakeEffects.DestroyAllEffect();
            root = pathF.FindPath(PlayerUnit.CELL, SerectCell);
            //�^�C���\��
            if (root != null)
            {
                foreach (var t in root)
                {
                    MakeEffects.MakeSerectEffect(t);
                }
            }
        }

        //�{�^���������ꂽ�炻�̎��̃��[�g�ɉ����Ĉړ�
        if (Input.GetMouseButtonUp(0))
        {
            //��U�G�t�F�N�g�͑S�ď���
            MakeEffects.DestroyAllEffect();

            moving = true;
            if (root != null)
            {
                foreach (var t in root)
                {
                    await PlayerUnit.Move(t, 0.1f);
                }
            }
            moving = false;
        }
    }
    /// <summary>
    ///�@�퓬���[�h�ւ̐؂�ւ�
    /// </summary>
    private void ModeChengeMode()
    {
        MakeEffects.MakeSerectEffectOnes(PlayerUnit.CELL);
        if (Input.GetMouseButtonUp(0) && SerectCell == PlayerUnit.CELL)
        {
            MakeEffects.DestroyAllEffect();
            IsCombat = true;
            //������
            SetCanUseArts(new Vector2Int(0, 0));
            //turn���I���
        }

    }

    //�R���o�b�g���[�h
    public static Vector2Int UseArtsIndex;//�g�p����A�[�c�̃C���f�b�N�X
    Arts[] canUseArts = new Arts[9];�@//�g�p�\�ȋZ�̃��X�g
    [SerializeField] GameObject[] ArtesUIs;//0�ړ�1~6�A�N�V����7���Z�b�g�W�[��
    int _combatIndex = 0;

    Arts serectedArts;
    public int CombatIndex
    {
        get => _combatIndex;//���[�h�̃T�C�N��
        set
        {
            if (value < 0) value = 8;
            else if (value > 8) value = 0;

            while (!ArtesUIs[value].activeSelf)
            {
                if (_combatIndex < value) value++;
                else value--;

                if (value < 0) value = 8;
                else if (value > 8) value = 0;

            }
            IsRefleshTiming = true;
            _combatIndex = value;

        }
    }

    /// <summary>
    /// <param name="cell">�̎��͂̃}�X�ɂ���A�[�c���g�p�\�ȃA�[�c�Ƃ���<param name="canUseArts">�Ɋi�[
    /// </summary>
    /// <param name="cell"></param>
    public void SetCanUseArts(Vector2Int cell)
    {
        CombatIndex = 0;


        Vector2Int[] Ncells = HexUtil.GetNeighborsCell(cell);
        int i = 1;//1~6���ύX����^�C���i�^��
        foreach (Vector2Int n in Ncells)
        {
            Arts arts;
            if (GameManager.AllArts.TryGetValue(n, out arts))
            {
                ArtesUIs[i].SetActive(true);
                ArtesUIs[i].GetComponent<ArtsUI>().ChengeCellLock(arts);
                canUseArts[i] = arts;
            }
            else
            {
                ArtesUIs[i].SetActive(false);
                canUseArts[i] = null;
            }
            i++;
        }

        ArtesUIs[0].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(7));//�X�e�b�v
        ArtesUIs[7].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(2));//���J�o�[
        ArtesUIs[8].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(8));//�[��
        canUseArts[0] = new Arts(ArtsDatabase.Get(7));
        canUseArts[7] = new Arts(ArtsDatabase.Get(2));
        canUseArts[8] = new Arts(ArtsDatabase.Get(8));



    }

    List<Vector3Int> CorectTargetList = new();
    private async void CombatMode()
    {
        if (moving) return;

        if (IsRefleshTiming)
        {
            //�g�p�\���̔���//�G�t�F�N�g�\��
            serectedArts = canUseArts[CombatIndex];

            MakeEffects.DestroyAllEffect();
            CorectTargetList = ArtsActionDatabase.ArtsTargets(serectedArts, PlayerUnit);

            foreach (var target in CorectTargetList)
            {
                MakeEffects.MakeSerectEffect(target);
            }
            IsRefleshTiming = false;//�ύX���Ɉ�x�������s����悤��
        }


        if (!Input.GetMouseButtonUp(0) || !CorectTargetList.Contains(SerectCell)) return;

        MakeEffects.DestroyAllEffect();

        if (serectedArts.apCost > AP)
        {
            //AP�����肸�Z���g���Ȃ�;

            return;
        }


        //AP����
        AP -= serectedArts.apCost;
        //���s
        if (serectedArts.CANUSE)//�܂��g�p���Ă��Ȃ��Ȃ�g����
        {
            serectedArts.CANUSE = false;//Resolve�ŕ��A���邱�Ƃ�����̂ŕK�����false�ɂ���
            PlayerAnimationController.ChengeAnimation(serectedArts, SerectCell, PlayerUnit.CELL);

            moving=true;
            await ArtsActionDatabase.ResolveArt(serectedArts, PlayerUnit, SerectCell);
            moving = false;

        }

        Vector2Int[] neiborcells = HexUtil.GetNeighborsCell(UseArtsIndex);
        switch (CombatIndex)
        {
            case 1: UseArtsIndex = neiborcells[0]; break;
            case 2: UseArtsIndex = neiborcells[1]; break;
            case 3: UseArtsIndex = neiborcells[2]; break;
            case 4: UseArtsIndex = neiborcells[3]; break;
            case 5: UseArtsIndex = neiborcells[4]; break;
            case 6: UseArtsIndex = neiborcells[5]; break;
        }

        Debug.Log("�C���f�b�N�X��" + UseArtsIndex);
        SetCanUseArts(UseArtsIndex);

    }

}

