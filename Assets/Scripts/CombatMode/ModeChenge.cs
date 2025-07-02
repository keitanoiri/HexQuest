using ArtsHandlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeChenge : MonoBehaviour
{

    [SerializeField] GameObject ExproreObject;
    [SerializeField] GameObject CombatObject;




    // Update is called once per frame
    void Update()
    {
        if (PlayerController.IsCombat)
        {
            ExproreObject.SetActive(false); CombatObject.SetActive(true);
        }
        else
        {
            ExproreObject.SetActive(true); CombatObject.SetActive(false);
        }
    }
}
