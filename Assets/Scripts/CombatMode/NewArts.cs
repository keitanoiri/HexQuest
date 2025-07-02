using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewArts : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ArtsTile ArtsTile;
    // Start is called before the first frame update
    void Start()
    {
        //idéÊìæ
        ushort ID = (ushort)Random.Range(3, 5);//ÇRÅ`ÇSÇ‹Ç≈
        ArtsTile.Arts = ArtsDatabase.Get(ID);
        ArtsTile.ChengeCellLock(ArtsTile.Arts,new Vector2Int(0,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //senntaku
        ArtsPanel ap = FindFirstObjectByType<ArtsPanel>(FindObjectsInactive.Include);
        ap.id = ArtsTile.Arts.id;
        ap.IsGetNewArts = true;
        ap.gameObject.SetActive(true);

        GetNewArts gna = FindFirstObjectByType<GetNewArts>(FindObjectsInactive.Include);
        gna.CloseButton();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ArtsTile.IsSerected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ArtsTile.IsSerected = false;
    }
}
