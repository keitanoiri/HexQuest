using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEffect : MonoBehaviour
{
    [SerializeField]SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        SpriteRenderer.color = Color.yellow;
    }

    void OnMouseEnter()
    {
        SpriteRenderer.color = Color.red;
    }

    void OnMouseExit()
    {
        SpriteRenderer.color = Color.yellow;
    }
}
