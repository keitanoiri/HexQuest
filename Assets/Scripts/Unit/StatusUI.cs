using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Image Image;
    //Unit unit;

    public void ChengeStatusUI(Unit unit)
    {
        if (unit.status.Has(key))
        {
            gameObject.SetActive(true);
            textMeshProUGUI.text = unit.status.StackOf(key).ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
        

    }
}
