using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeMap : MonoBehaviour
{
    [SerializeField] private Tilemap[] grids;// �� �ŉ��i���珇�ɕ��ׂ�

    // Start is called before the first frame update
    void Awake()
    {
        
        foreach (var tm in grids)
        {
            // 1) �����^�C�����ꎞ�o�b�t�@�Ɏ��W
            BoundsInt bounds = tm.cellBounds;
            var list = new List<(Vector3Int pos, TileBase tile, Matrix4x4 mat, Color col)>();
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (!tm.HasTile(pos)) continue;

                list.Add((pos,tm.GetTile(pos),tm.GetTransformMatrix(pos),tm.GetColor(pos)));
            }
            // 2) �ꊇ�N���A
            tm.ClearAllTiles();
            
            int chengeZ = (int)(tm.transform.position.z*2);
            foreach (var e in list)
            {
                Vector3Int dst = new Vector3Int(e.pos.x,e.pos.y,chengeZ); //z �I�t�Z�b�g

                tm.SetTile(dst, e.tile);
                tm.SetTransformMatrix(dst, e.mat);
                tm.SetTileFlags(dst, TileFlags.None);
                tm.SetColor(dst, e.col);
            }
            
        }
        //�ŏI�I�ɃQ�[���}�l�[�W���[�Ɋi�[
        GameManager.groundLayers = grids;
    }
}
