using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComandUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] int Index;//アーツのインデックスに相当

    Image parentImage;
    [SerializeField] Image baseImage;
    [SerializeField] Image iconimage;
    [SerializeField] GameObject usedImage;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI APcost;

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

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.ExproreIndex == Index)//若干重くなるかも
        {
            IsSerected = true;
        }else
        {
            IsSerected = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //セレクトされた。
        PlayerController.ExproreIndex = Index;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        //セレクトされている
        PlayerController.ExproreIndex = Index;
    }
}
