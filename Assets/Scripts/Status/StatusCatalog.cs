using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StatusSystem
{
    /// <summary>�S StatusDef ���ꌳ�Ǘ����郊�|�W�g�� (Resources ��)�ύX�\��</summary>
    public static class StatusCatalog
    {
        static readonly Dictionary<string, StatusDef> map;

        static StatusCatalog()
        {
            //Resources/StatusDefs�z���ɒu����SD��S���ǂݍ���
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
