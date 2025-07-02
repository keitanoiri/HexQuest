using UnityEngine;

namespace StatusSystem
{
    public enum ExpireTrigger
    {
        EndTurn,
        BeforeAttack,
        AfterAttack,
        WhenHit,
        Custom
    }

    [CreateAssetMenu(menuName = "Status/Status Definition", fileName = "NewStatusDef")]
    public class StatusDef : ScriptableObject
    {
        [Header("Meta")]
        public string id = "Burn";              // ��ӃL�[
        public Sprite icon;
        [Tooltip("�ǂ̃^�C�~���O�ŏ��Ŕ�������邩")]
        public ExpireTrigger expireTrigger = ExpireTrigger.EndTurn;

        [Header("��{�p�����[�^")]
        public int baseTurns = 2;
        public int maxStack = 3;
        public bool refreshOnAdd = true;

        [Header("�X�^�b�N���ƂɃX�L���Ő�����}�C�i�X���ʂ̒l")]
        public int damagePerStack = 2;          
    }


}
