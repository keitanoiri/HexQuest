using Cysharp.Threading.Tasks;
using DG.Tweening;
using StatusSystem;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using UnityEngine;

public class Unit: MonoBehaviour
{
    public int ID;
    public string NAME;

    [SerializeField]Vector3Int cell_;
    public Transform ObjectTransform;//オブジェクトのワールド座標を格納するこれを変更する
    public Vector3Int CELL//要検証
    {
        get => cell_;
        set
        {
            if (cell_ != value)
            {
                GameManager.UnitPos.Remove(cell_);
                GameManager.UnitPos.Add(value, ID);
                ObjectTransform.position = HexUtil.CellToWorld(value);
                cell_ = value; 
            }
        }
    }

    public int MAXHP;
    int hp_;
    public int HP
    {
        get => hp_;
        set
        {
            if (hp_ == value)return;
            if (value > MAXHP) hp_ = MAXHP;
            else if (value <= 0)
            {
                hp_ = 0;
                //倒された処理
                UnitDead();
            }
            else hp_ = value;
        }
    }

    //ステータス
    public StatusContainer status = new();

    [Tooltip("行動パターンを差し替え")]
    [SerializeField] MonoBehaviour behaviourComponent;
    IUnitTurnBehaviour behaviour;

    [Tooltip("死体のプレハブ「")]
    [SerializeField] GameObject DeadBodyPrefab;

    void Awake()
    {
        behaviour = (IUnitTurnBehaviour)behaviourComponent;

        //Unitの初期化
        ObjectTransform = this.transform;
        GameManager.RegisterUnit(this);//辞書登録

        //HPを最大値に
        HP = MAXHP;
    }

    private void Start()
    {
        //位置の初期設定
        ObjectTransform.position = HexUtil.CellToWorld(CELL);
    }

    public async UniTask StartTurn()
    {
        //ターン開始時(行動入力)
        if (behaviour == null)
            Debug.LogError($"{NAME} に行動コンポーネント未設定");
        else
            await behaviour.ExecuteTurn(this);

    }
    void UnitDead()
    {
        //倒された処理
        GameManager.UmregisterUnit(this.ID);
        Instantiate(DeadBodyPrefab, ObjectTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Attack(Unit target,int damage)
    {
        status.Notify(ExpireTrigger.BeforeAttack, this);

        // ここでダメージ判定など
        target.BeHit(damage);

        status.Notify(ExpireTrigger.AfterAttack, this);
    }

    public void BeHit(int dmg)
    {
        //statusガードがダメージを吸収する
        int guardStack = status.StackOf("Guard");
        if (guardStack > 0)
        {
            int absorbed = Mathf.Min(dmg, guardStack); // 吸収量
            dmg -= absorbed;                     　　　// 残ダメ
            guardStack -= absorbed;                    // スタック減

            Debug.Log($"{NAME} の Guard が {absorbed} 吸収 -> 残 {guardStack}");

            if (guardStack <= 0)
                status.Consume("Guard");
            else
                status.SetStack("Guard", guardStack);    

            if (dmg == 0)                                // 完全吸収なら通常処理せず return
                return;
        }

        HP -= dmg;
        Debug.Log($"{NAME} は {dmg} ダメージを受けた (HP={HP})");
        status.Notify(ExpireTrigger.WhenHit, this);
    }

    public async UniTask Move(Vector3Int movePos,float time)
    {
        await ObjectTransform.DOMove(HexUtil.CellToWorld(movePos), time).SetEase(Ease.Linear);
        CELL = movePos;//インデックス更新
    }

    public async UniTask Jump(Vector3Int jumpPos,float jumpPower,float time)
    {
        await ObjectTransform.DOJump(HexUtil.CellToWorld(jumpPos),jumpPower, 1, time).SetEase(Ease.Linear);
        //移動が完了したらプレイやの位置情報を更新
        CELL = jumpPos;
    }

    public void EndTurn()
    {
        status.Notify(ExpireTrigger.EndTurn, this);
    }

    /* 状態異常 Tick 内部用: ダメージ直接減算 */
    public void TakeDirectDamage(int dmg)
    {
        HP -= dmg;
    }


}
