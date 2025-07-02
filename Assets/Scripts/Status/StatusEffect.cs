using UnityEngine;

namespace StatusSystem
{
    public class StatusEffect
    {
        public StatusDef def;
        public int stack;
        public int remainingTurns;
        public int usesLeft = 1;    // Trigger系で使う（必要に応じて変更）

        public StatusEffect(StatusDef source)
        {
            def = source;
            stack = 1;
            remainingTurns = source.baseTurns;
        }

        /// <summary>ターン経過時など毎Tick呼ばれる効果。</summary>
        public void Tick(Unit owner)
        {
            if (def.id == "Burn")
            {
                int dmg = def.damagePerStack * stack;
                owner.TakeDirectDamage(dmg);        // HP‐減算のみ
                Debug.Log($"{owner.NAME} が Burn で {dmg} DMG");
            }
        }

        /// <summary> 指定トリガーで消滅するか判定し、内部カウンタも消費 </summary>
        public bool CheckExpire(ExpireTrigger trig)
        {
            if (def.expireTrigger != trig) return false;

            switch (trig)
            {
                case ExpireTrigger.EndTurn:
                    return --remainingTurns <= 0;

                default: // 一発系
                    return --usesLeft <= 0;
            }
        }
    }
}
