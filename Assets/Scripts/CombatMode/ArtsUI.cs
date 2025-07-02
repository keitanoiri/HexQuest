using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtsUI : MonoBehaviour,IPointerDownHandler,IDragHandler
{
    [SerializeField] int Index;

    [SerializeField] PlayerController pc;

    public Arts Arts;
    Image parentImage;
    [SerializeField]Image baseImage;
    [SerializeField]Image iconimage;
    [SerializeField]GameObject usedImage;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField]TextMeshProUGUI APcost;
    [SerializeField]TextMeshProUGUI Attack;
    [SerializeField] TextMeshProUGUI Guard;

    bool isserected_;
    public bool IsSerected
    {
        get => isserected_;
        set
        {
            if(isserected_==value) return;
            if(value == true)
            {
                Color c = parentImage.color;
                c.a = 1f;
                parentImage.color = c;
            }
            else
            {
                Color c = parentImage.color;
                c.a = 0f;
                parentImage.color = c;
            }

            isserected_ = value;
        }
    }


    void Awake()
    {
        parentImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.CombatIndex == Index)//若干重くなるかも
        {
            IsSerected = true;
        }
        else
        {
            IsSerected = false;
        }
    }

    /// <summary>
    /// 見た目の変更。生成した側で呼び出す
    /// </summary>
    /// <param name="arts"></param>
    public void ChengeCellLock(Arts arts)
    {
        //ベースの色
        switch (arts.type)
        {
            case ArtsType.Special: baseImage.color = Color.black; break;
            case ArtsType.Attack: baseImage.color = Color.red; break;
            case ArtsType.Guard: baseImage.color = Color.blue; break;
            case ArtsType.Action: baseImage.color = Color.gray; break;
        }
        //画像
        Sprite sp = Resources.Load<Sprite>($"Sprites/ArtsSprites/{arts.Icon}");
        iconimage.sprite = sp;
        //各種数字
        Name.text = arts.nameJP;
        APcost.text = arts.apCost.ToString();
        Attack.text = arts.power.ToString();
        Guard.text = arts.power.ToString();

        if(arts.CANUSE)usedImage.SetActive(false);
        else usedImage.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //セレクトされた。
        pc.CombatIndex = Index;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        //セレクトされている。
        pc.CombatIndex = Index;
    }

}
