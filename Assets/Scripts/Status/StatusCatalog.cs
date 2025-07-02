using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StatusSystem
{
    /// <summary>全 StatusDef を一元管理するリポジトリ (Resources 版)変更予定</summary>
    public static class StatusCatalog
    {
        static readonly Dictionary<string, StatusDef> map;

        static StatusCatalog()
        {
            //Resources/StatusDefs配下に置いたSDを全部読み込む
            map = Resources.LoadAll<StatusDef>("StatusDefs")
                           .ToDictionary(d => d.id);
        }

        public static StatusDef Get(string id)
        {
            if (map.TryGetValue(id, out var def)) return def;
            Debug.LogError($"StatusDef not found: {id}");
            return null;
        }
    }
}
