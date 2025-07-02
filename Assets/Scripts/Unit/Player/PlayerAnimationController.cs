using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] Transform PlayerTransform;

    // Update is called once per frame
    void Update()
    {
        //マウスカーソルの方向にプレイヤーの向きを変える
        if (Input.mousePosition.x < Screen.width * 0.5f) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);

        if (PlayerController.IsCombat)
        {
            PlayerAnimator.SetBool("IsCombat", true);
        }
        else
        {
            PlayerAnimator.SetBool("IsCombat", false);
        }

    }


    public void ChengeAnimation(Arts usearts,Vector3Int targetCell, Vector3Int curentCell)
    {
        switch (usearts.animKey)
        {
            case "Slash01": PlaySlashAnimation(targetCell,curentCell); break;
            case "Guard01": PlayGuardAnimation(curentCell); break;


        }
    }

    private async void PlaySlashAnimation(Vector3Int target,Vector3Int curent)
    {
        Vector3 pos = (HexUtil.CellToWorld(target)-HexUtil.CellToWorld(curent))/2+HexUtil.CellToWorld(curent);
        await PlayerTransform.DOMove(pos, 0.1f).SetEase(Ease.InQuad);
        await PlayerTransform.DOMove(HexUtil.CellToWorld(curent), 0.1f).SetEase(Ease.OutQuad);
    }

    private async void PlayGuardAnimation(Vector3Int curent)
    {
        await PlayerTransform.DOJump(HexUtil.CellToWorld(curent), 0.5f, 1, 0.2f).SetEase(Ease.Linear);
    }

}
