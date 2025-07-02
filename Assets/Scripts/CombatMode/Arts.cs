using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArtsType { Attack, Guard, Action,Move, Buff, Special }

public class Arts 
{
    public ushort id;
    public string nameJP;
    public ArtsType type;
    public byte apCost;
    public string rangeKey;
    public string[] ConditionOps;
    public byte power;
    public string[] statusOps;  // "Bleed+2" など
    public string comboRaw;    // "AfterGuard:+2DMG"
    public string animKey;
    public string Icon;
    public bool CANUSE = true;

    // ★ コピーコンストラクタ ★
    public Arts(Arts src)
    {
        id = src.id;
        nameJP = src.nameJP;              // string はイミュータブルなので共有可
        type = src.type;
        apCost = src.apCost;
        rangeKey = src.rangeKey;
        ConditionOps = (string[])src.ConditionOps.Clone();
        power = src.power;

        // 参照型 (配列) は内容を複製して分離
        statusOps = (string[])src.statusOps?.Clone();

        comboRaw = src.comboRaw;
        animKey = src.animKey;
        Icon = src.Icon;
        CANUSE = src.CANUSE;
    }

    // 既存の空コンストラクタを残すなら明示的に
    public Arts() { }
}
