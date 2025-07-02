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
        public string id = "Burn";              // 一意キー
        public Sprite icon;
        [Tooltip("どのタイミングで消滅判定をするか")]
        public ExpireTrigger expireTrigger = ExpireTrigger.EndTurn;

        [Header("基本パラメータ")]
        public int baseTurns = 2;
        public int maxStack = 3;
        public bool refreshOnAdd = true;

        [Header("スタックごとにスキルで生じるマイナス効果の値")]
        public int damagePerStack = 2;          
    }


}
