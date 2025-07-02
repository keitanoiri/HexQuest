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
    public Transform ObjectTransform;//�I�u�W�F�N�g�̃��[���h���W���i�[���邱���ύX����
    public Vector3Int CELL//�v����
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
                //�|���ꂽ����
                UnitDead();
            }
            else hp_ = value;
        }
    }

    //�X�e�[�^�X
    public StatusContainer status = new();

    [Tooltip("�s���p�^�[���������ւ�")]
    [SerializeField] MonoBehaviour behaviourComponent;
    IUnitTurnBehaviour behaviour;

    [Tooltip("���̂̃v���n�u�u")]
    [SerializeField] GameObject DeadBodyPrefab;

    void Awake()
    {
        behaviour = (IUnitTurnBehaviour)behaviourComponent;

        //Unit�̏�����
        ObjectTransform = this.transform;
        GameManager.RegisterUnit(this);//�����o�^

        //HP���ő�l��
        HP = MAXHP;
    }

    private void Start()
    {
        //�ʒu�̏����ݒ�
        ObjectTransform.position = HexUtil.CellToWorld(CELL);
    }

    public async UniTask StartTurn()
    {
        //�^�[���J�n��(�s������)
        if (behaviour == null)
            Debug.LogError($"{NAME} �ɍs���R���|�[�l���g���ݒ�");
        else
            await behaviour.ExecuteTurn(this);

    }
    void UnitDead()
    {
        //�|���ꂽ����
        GameManager.UmregisterUnit(this.ID);
        Instantiate(DeadBodyPrefab, ObjectTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Attack(Unit target,int damage)
    {
        status.Notify(ExpireTrigger.BeforeAttack, this);

        // �����Ń_���[�W����Ȃ�
        target.BeHit(damage);

        status.Notify(ExpireTrigger.AfterAttack, this);
    }

    public void BeHit(int dmg)
    {
        //status�K�[�h���_���[�W���z������
        int guardStack = status.StackOf("Guard");
        if (guardStack > 0)
        {
            int absorbed = Mathf.Min(dmg, guardStack); // �z����
            dmg -= absorbed;                     �@�@�@// �c�_��
            guardStack -= absorbed;                    // �X�^�b�N��

            Debug.Log($"{NAME} �� Guard �� {absorbed} �z�� -> �c {guardStack}");

            if (guardStack <= 0)
                status.Consume("Guard");
            else
                status.SetStack("Guard", guardStack);    

            if (dmg == 0)                                // ���S�z���Ȃ�ʏ폈������ return
                return;
        }

        HP -= dmg;
        Debug.Log($"{NAME} �� {dmg} �_���[�W���󂯂� (HP={HP})");
        status.Notify(ExpireTrigger.WhenHit, this);
    }

    public async UniTask Move(Vector3Int movePos,float time)
    {
        await ObjectTransform.DOMove(HexUtil.CellToWorld(movePos), time).SetEase(Ease.Linear);
        CELL = movePos;//�C���f�b�N�X�X�V
    }

    public async UniTask Jump(Vector3Int jumpPos,float jumpPower,float time)
    {
        await ObjectTransform.DOJump(HexUtil.CellToWorld(jumpPos),jumpPower, 1, time).SetEase(Ease.Linear);
        //�ړ�������������v���C��̈ʒu�����X�V
        CELL = jumpPos;
    }

    public void EndTurn()
    {
        status.Notify(ExpireTrigger.EndTurn, this);
    }

    /* ��Ԉُ� Tick �����p: �_���[�W���ڌ��Z */
    public void TakeDirectDamage(int dmg)
    {
        HP -= dmg;
    }


}
