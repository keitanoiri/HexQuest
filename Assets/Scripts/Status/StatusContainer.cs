using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

namespace StatusSystem
{
    public class StatusContainer
    {
        readonly Dictionary<string, StatusEffect> effects = new();

        /* �t�^ -------------------------------------------------- */
        public void Add(StatusDef def)
        {
            if (effects.TryGetValue(def.id, out var eff))
            {
                eff.stack = UnityEngine.Mathf.Min(eff.stack + 1, def.maxStack);
                if (def.refreshOnAdd) eff.remainingTurns = def.baseTurns;
            }
            else
            {
                effects[def.id] = new StatusEffect(def);
            }
        }

        public void Add(string id, int stacks = 1)
        {
            var def = StatusCatalog.Get(id);
            if (!def) return;

            for (int i = 0; i < stacks; i++)
                Add(def);        // �������\�b�h�ė��p
        }


        public void SetStack(string id, int newStack)
        {
            if (effects.TryGetValue(id, out var e))
                e.stack = Mathf.Clamp(newStack, 0, e.def.maxStack);
            else
            {
                Debug.Log(id + ":�ǉ����s");
            }
        }

        public bool Consume(string id)
        {
            return effects.Remove(id);// ����Ύ�������O����true
        }


        /* �^�[��or�C�x���g���Ƃ̒ʒm ---------------------------- */
        public void Notify(ExpireTrigger trig, Unit owner)
        {
            //remove��Ɏ������񂳂Ȃ��悤��x�v�[�����X�g��
            var toRemove = ListPool<string>.Get();
            foreach (var kv in effects)
            {
                if (trig == ExpireTrigger.EndTurn)
                    kv.Value.Tick(owner);

                if (kv.Value.CheckExpire(trig))
                    toRemove.Add(kv.Key);
            }

            foreach (var id in toRemove) effects.Remove(id);
            ListPool<string>.Release(toRemove);
        }

        /* �N�G�� -------------------------------------------------- */
        public bool Has(string id) => effects.ContainsKey(id);
        public int StackOf(string id) => effects.TryGetValue(id, out var e) ? e.stack : 0;
    }
}
