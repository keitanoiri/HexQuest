
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;	//DOTweenを使うときはこのusingを入れる
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
    //共通
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

    bool IsRefleshTiming = false;//選択しているセルが変更されたときなどの更新たいみんぐを教えてくれる
    Vector3Int searchcell_;
    Vector3Int SerectCell//マウスなどで選択中のセル
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
            if (value < _ap)//消費したAP文経験値獲得？
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
                //新アーツ獲得
                getNewArts.gameObject.SetActive(true);
            }else _exp = value;
        }
    }

    int LEVEL;


    //探索モード
    [SerializeField] GameObject ExplorePanel;
    List<Vector3Int> root = new List<Vector3Int>();
    static int _exproreIndex = 0;
    public static int ExproreIndex
    {
        get => _exproreIndex;//モードのサイクル
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


        //選択中のセルは？
        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        if (HexUtil.TryWorldToCell(world, out Vector3Int cell)) SerectCell = cell;
        else return;

        //選択しているメニューを切り替える処理(統一)
        ChengeIndex();

        if (EventSystem.current.IsPointerOverGameObject())//UI操作中は判定しない
            return;

        if (GameManager.playerturn == false) return;

        //モードごとに異なる振る舞い
        if (IsCombat) CombatMode();
        else ExproreMode();
    }

    public void StartTurn()
    {
        //ターン開始時の処理
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
                    Debug.Log("正常");
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

        //リフレッシュタイミングであればパスを更新
        if (IsRefleshTiming)
        {
            //一旦エフェクトは全て消す
            MakeEffects.DestroyAllEffect();
            root = pathF.FindPath(PlayerUnit.CELL, SerectCell);
            //タイル表示
            if (root != null)
            {
                foreach (var t in root)
                {
                    MakeEffects.MakeSerectEffect(t);
                }
            }
        }

        //ボタンが押されたらその時のルートに沿って移動
        if (Input.GetMouseButtonUp(0))
        {
            //一旦エフェクトは全て消す
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
    ///　戦闘モードへの切り替え
    /// </summary>
    private void ModeChengeMode()
    {
        MakeEffects.MakeSerectEffectOnes(PlayerUnit.CELL);
        if (Input.GetMouseButtonUp(0) && SerectCell == PlayerUnit.CELL)
        {
            MakeEffects.DestroyAllEffect();
            IsCombat = true;
            //初期化
            SetCanUseArts(new Vector2Int(0, 0));
            //turnを終わる
        }

    }

    //コンバットモード
    public static Vector2Int UseArtsIndex;//使用するアーツのインデックス
    Arts[] canUseArts = new Arts[9];　//使用可能な技のリスト
    [SerializeField] GameObject[] ArtesUIs;//0移動1~6アクション7リセット８納刀
    int _combatIndex = 0;

    Arts serectedArts;
    public int CombatIndex
    {
        get => _combatIndex;//モードのサイクル
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
    /// <param name="cell">の周囲のマスにあるアーツを使用可能なアーツとして<param name="canUseArts">に格納
    /// </summary>
    /// <param name="cell"></param>
    public void SetCanUseArts(Vector2Int cell)
    {
        CombatIndex = 0;


        Vector2Int[] Ncells = HexUtil.GetNeighborsCell(cell);
        int i = 1;//1~6が変更するタイルナタメ
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

        ArtesUIs[0].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(7));//ステップ
        ArtesUIs[7].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(2));//リカバー
        ArtesUIs[8].GetComponent<ArtsUI>().ChengeCellLock(ArtsDatabase.Get(8));//納刀
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
            //使用可能かの判定//エフェクト表示
            serectedArts = canUseArts[CombatIndex];

            MakeEffects.DestroyAllEffect();
            CorectTargetList = ArtsActionDatabase.ArtsTargets(serectedArts, PlayerUnit);

            foreach (var target in CorectTargetList)
            {
                MakeEffects.MakeSerectEffect(target);
            }
            IsRefleshTiming = false;//変更時に一度だけ実行するように
        }


        if (!Input.GetMouseButtonUp(0) || !CorectTargetList.Contains(SerectCell)) return;

        MakeEffects.DestroyAllEffect();

        if (serectedArts.apCost > AP)
        {
            //APが足りず技が使えない;

            return;
        }


        //AP消費
        AP -= serectedArts.apCost;
        //実行
        if (serectedArts.CANUSE)//まだ使用していないなら使える
        {
            serectedArts.CANUSE = false;//Resolveで復帰することもあるので必ず先にfalseにする
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

        Debug.Log("インデックス＝" + UseArtsIndex);
        SetCanUseArts(UseArtsIndex);

    }

}

