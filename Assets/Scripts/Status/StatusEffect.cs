using UnityEngine;

namespace StatusSystem
{
    public class StatusEffect
    {
        public StatusDef def;
        public int stack;
        public int remainingTurns;
        public int usesLeft = 1;    // Trigger�n�Ŏg���i�K�v�ɉ����ĕύX�j

        public StatusEffect(StatusDef source)
        {
            def = source;
            stack = 1;
            remainingTurns = source.baseTurns;
        }

        /// <summary>�^�[���o�ߎ��Ȃǖ�Tick�Ă΂����ʁB</summary>
        public void Tick(Unit owner)
        {
            if (def.id == "Burn")
            {
                int dmg = def.damagePerStack * stack;
                owner.TakeDirectDamage(dmg);        // HP�]���Z�̂�
                Debug.Log($"{owner.NAME} �� Burn �� {dmg} DMG");
            }
        }

        /// <summary> �w��g���K�[�ŏ��ł��邩���肵�A�����J�E���^������ </summary>
        public bool CheckExpire(ExpireTrigger trig)
        {
            if (def.expireTrigger != trig) return false;

            switch (trig)
            {
                case ExpireTrigger.EndTurn:
                    return --remainingTurns <= 0;

                default: // �ꔭ�n
                    return --usesLeft <= 0;
            }
        }
    }
}
