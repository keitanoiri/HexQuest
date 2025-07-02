using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

namespace StatusSystem
{
    public class StatusContainer
    {
        readonly Dictionary<string, StatusEffect> effects = new();

        /* 付与 -------------------------------------------------- */
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
                Add(def);        // 既存メソッド再利用
        }


        public void SetStack(string id, int newStack)
        {
            if (effects.TryGetValue(id, out var e))
                e.stack = Mathf.Clamp(newStack, 0, e.def.maxStack);
            else
            {
                Debug.Log(id + ":追加失敗");
            }
        }

        public bool Consume(string id)
        {
            return effects.Remove(id);// あれば辞書から外してtrue
        }


        /* ターンorイベントごとの通知 ---------------------------- */
        public void Notify(ExpireTrigger trig, Unit owner)
        {
            //remove後に辞書を回さないよう一度プールリストへ
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

        /* クエリ -------------------------------------------------- */
        public bool Has(string id) => effects.ContainsKey(id);
        public int StackOf(string id) => effects.TryGetValue(id, out var e) ? e.stack : 0;
    }
}
