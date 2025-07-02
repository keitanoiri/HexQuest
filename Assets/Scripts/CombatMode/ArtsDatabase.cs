using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEngine;

public static class ArtsDatabase
{


    // ★ ここが検索用の電話帳
    private static readonly Dictionary<ushort, Arts> AllArtsDatabase = new();

    // ゲーム開始時に呼び出す
    public static void Load(string csvPath)
    {
        string[] lines = File.ReadAllLines(csvPath);   // StreamingAssets 内なら Path.Combine

        for (int i = 1; i < lines.Length; i++)         // 1行目はヘッダ
        {
            string[] t = lines[i].Split(',');

            var arts = new Arts
            {
                id = ushort.Parse(t[0]),
                nameJP = t[1],
                // t[2] が NameEN ならここでは省略
                type = Enum.Parse<ArtsType>(t[3]),
                apCost = byte.Parse(t[4]),
                rangeKey = t[5],
                ConditionOps = t[6].Split(';', System.StringSplitOptions.RemoveEmptyEntries),
                power = byte.Parse(t[7]),
                statusOps = t[8].Split(';', System.StringSplitOptions.RemoveEmptyEntries),
                comboRaw = t[9],
                animKey = t[10],
                Icon = t[11]
            };
            AllArtsDatabase[arts.id] = arts;                        // 重複なら上書き
        }

        Debug.Log($"Loaded {AllArtsDatabase.Count} Arts");
    }

    // ID で取得   例:  var slash = ArtDatabase.Get(1);
    public static Arts Get(ushort id)
    {
        if (AllArtsDatabase.TryGetValue(id, out var art))
            return art;

        Debug.LogError($"Art ID {id} not found");
        return null;
    }
}

