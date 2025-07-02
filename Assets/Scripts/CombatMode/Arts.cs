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
    public string[] statusOps;  // "Bleed+2" �Ȃ�
    public string comboRaw;    // "AfterGuard:+2DMG"
    public string animKey;
    public string Icon;
    public bool CANUSE = true;

    // �� �R�s�[�R���X�g���N�^ ��
    public Arts(Arts src)
    {
        id = src.id;
        nameJP = src.nameJP;              // string �̓C�~���[�^�u���Ȃ̂ŋ��L��
        type = src.type;
        apCost = src.apCost;
        rangeKey = src.rangeKey;
        ConditionOps = (string[])src.ConditionOps.Clone();
        power = src.power;

        // �Q�ƌ^ (�z��) �͓��e�𕡐����ĕ���
        statusOps = (string[])src.statusOps?.Clone();

        comboRaw = src.comboRaw;
        animKey = src.animKey;
        Icon = src.Icon;
        CANUSE = src.CANUSE;
    }

    // �����̋�R���X�g���N�^���c���Ȃ疾���I��
    public Arts() { }
}
