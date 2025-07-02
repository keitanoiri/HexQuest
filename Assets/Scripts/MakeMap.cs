using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeMap : MonoBehaviour
{
    [SerializeField] private Tilemap[] grids;// ← 最下段から順に並べる

    // Start is called before the first frame update
    void Awake()
    {
        
        foreach (var tm in grids)
        {
            // 1) 既存タイルを一時バッファに収集
            BoundsInt bounds = tm.cellBounds;
            var list = new List<(Vector3Int pos, TileBase tile, Matrix4x4 mat, Color col)>();
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (!tm.HasTile(pos)) continue;

                list.Add((pos,tm.GetTile(pos),tm.GetTransformMatrix(pos),tm.GetColor(pos)));
            }
            // 2) 一括クリア
            tm.ClearAllTiles();
            
            int chengeZ = (int)(tm.transform.position.z*2);
            foreach (var e in list)
            {
                Vector3Int dst = new Vector3Int(e.pos.x,e.pos.y,chengeZ); //z オフセット

                tm.SetTile(dst, e.tile);
                tm.SetTransformMatrix(dst, e.mat);
                tm.SetTileFlags(dst, TileFlags.None);
                tm.SetColor(dst, e.col);
            }
            
        }
        //最終的にゲームマネージャーに格納
        GameManager.groundLayers = grids;
    }
}
