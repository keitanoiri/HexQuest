using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtsTile : MonoBehaviour
{

    public Arts Arts;
    Image parentImage;
    [SerializeField] Image baseImage;
    [SerializeField]Image iconimage;
    [SerializeField] GameObject usedImage;
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
            if (isserected_ == value) return;
            if (value == true)
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

    }

    public void ChengeCellLock(Arts arts,Vector2Int key)
    {
        Arts = arts;

        this.transform.localPosition = HexUtil.CellToScrean(key);

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

        if (arts.CANUSE) usedImage.SetActive(false);
        else usedImage.SetActive(true);
    }



}
