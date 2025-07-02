using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    int hp_;
    public int HP
    {
        get => hp_;
        set
        {
            if(hp_ == value) return;
            chengeHPUI(value);
            hp_ = value;
        }
    }
    [SerializeField] TextMeshProUGUI HpText;
    [SerializeField] TextMeshProUGUI NameText;
    [SerializeField] Image parentImage;
    [SerializeField] Image childImage;
    [SerializeField] StatusUI statusUI;

    Unit unit;

    private void chengeHPUI(int h)
    {
        HpText.text = h.ToString();
        float delta = (float)h/unit.MAXHP;
        delta = (1f-delta) * parentImage.rectTransform.rect.width;
        Vector2 ancherPos = new Vector2Int(0,0);
        ancherPos.x -= delta;
        childImage.rectTransform.anchoredPosition = ancherPos;
    }

    private Canvas UICanvas;          // 親 Canvas
    private Camera cam;
    private RectTransform hpRect;          // バーの RectTransform

    private void Start()
    {
        //初期設定
        unit = GetComponentInParent<Unit>();//親からUnitを探す
        NameText.text = unit.NAME;
        chengeHPUI(unit.HP);


        //名前の生成
        cam = Camera.main;
        UICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();//エラー出るかも雑な実装
        transform.SetParent(UICanvas.transform);
        hpRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        HP = unit.HP;
        statusUI.ChengeStatusUI(unit);
        if (unit == null)
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        Vector3 worldPos = HexUtil.CellToWorld(unit.CELL);
        Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);
        Vector2 canvasSize = UICanvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 anchored = new Vector2((viewportPos.x) * canvasSize.x,(viewportPos.y) * canvasSize.y);
        hpRect.anchoredPosition = anchored;
    }
}
