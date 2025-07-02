using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNewArts : MonoBehaviour
{
    [SerializeField] GameObject ArtsTilePrefab;
    [SerializeField] Transform ParentTransform;

    public int NumOfOptions = 3;

    List<GameObject> generatedObject = new();

    private void OnEnable()
    {
        //‘I‘ğˆ¶¬
        for (int i = 0; i < NumOfOptions; i++)
        {
            generatedObject.Add(Instantiate(ArtsTilePrefab, ParentTransform));
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        foreach (GameObject obj in generatedObject)
        {
            Destroy(obj);
        }
    }

    public void CloseButton()
    {
        this.gameObject.SetActive(false);
    }
}
