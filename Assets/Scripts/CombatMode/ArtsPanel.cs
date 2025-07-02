using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtsPanel : MonoBehaviour,IPointerMoveHandler
{
    [SerializeField] GameObject InstanceTile;
    [SerializeField] RectTransform parentrect;
    GameObject cleatedTile;

    [SerializeField]Canvas UIcanvas;
    Vector2Int cellIndex_;
    Vector2Int? CellIndex
    {
        get => cellIndex_;
        set { 
            if (cellIndex_ == value || value == null) return;
            //hyouzi
            Vector2Int cell = value.Value;
            ShowInstantTile(cell);
            cellIndex_ = cell;
        }

    }

    public ushort id = 3;

    public bool IsGetNewArts;


    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //�����̃A�[�c�𐶐�
        foreach(KeyValuePair<Vector2Int,Arts> arts in GameManager.AllArts)
        {
            GameObject�@Tile = Instantiate(InstanceTile, parentrect);
            ArtsTile at;
            if (Tile.TryGetComponent<ArtsTile>(out at))
            {
                at.ChengeCellLock(arts.Value,arts.Key);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGetNewArts)return;

        //�A�[�c�ǉ�
        if (CellIndex != null && Input.GetMouseButtonDown(0))//�E�N���b�N��
        {
            if (GameManager.AllArts.TryAdd(CellIndex.Value, new Arts(ArtsDatabase.Get(id)))){
                IsGetNewArts = false;//�ǉ�����

                Destroy(cleatedTile);

                //���������Ȃ猩���ڂ����
                GameObject Tile = Instantiate(InstanceTile, parentrect);
                ArtsTile at;
                if (Tile.TryGetComponent<ArtsTile>(out at))
                {
                    at.ChengeCellLock(GameManager.AllArts[CellIndex.Value],CellIndex.Value);
                }

            }
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2 pos = eventData.position - parentrect.anchoredPosition;

        CellIndex = HexUtil.ScreenToCell(pos);
        if (Input.GetMouseButton(2))
        {
            parentrect.anchoredPosition += eventData.delta;
        }
    }

    public void ShowInstantTile(Vector2Int cell)
    {
        if(!IsGetNewArts) return;

        Arts arts;
        if(GameManager.AllArts.TryGetValue(cell, out arts)) { 
            return;//���ɑ��݂���Ȃ�ŏI�I�Ƀ��^�[��
        }
        Destroy(cleatedTile);
        cleatedTile = Instantiate(InstanceTile,parentrect);
        ArtsTile at;
        if (cleatedTile.TryGetComponent<ArtsTile>(out at))
        {
            at.ChengeCellLock(ArtsDatabase.Get(id), cell);
        }
    }
}
