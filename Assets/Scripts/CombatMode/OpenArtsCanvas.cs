using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenArtsCanvas : MonoBehaviour
{
    [SerializeField] GameObject MenuObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenArtsMenu()
    {
        MenuObject.SetActive(true);
    }
    public void CloseArtsMenu()
    {
        MenuObject.SetActive(false);
    }

}
