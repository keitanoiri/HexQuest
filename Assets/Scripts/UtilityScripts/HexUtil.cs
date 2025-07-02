using System.Collections.Generic;
using UnityEngine;

public static class HexUtil
{



    // 1セルの寸法（px）
    private const float WIDTH = 200f;
    private const float HALF_WIDTH = WIDTH * 0.5f;   // = 100
    private const float VERT_SPACING = WIDTH * 0.75f;  // = 150

   /// <summary>
   /// 二つのセルが隣接するかを判定する
   /// </summary>
   /// <param name="curent"></param>
   /// <param name="target"></param>
   /// <returns></returns>
    public static bool IsAdjacent(Vector3Int curent,Vector3Int target)
    {
        Vector3Int[] NeighborsCell = GetNeighborsCell(curent);

        foreach (var neighbor in NeighborsCell)
        {
            if (neighbor == target) return true;
        }
        return false;

    }

    /// <summary>
    /// 指定したセルインデックスのTerrainTileを取得する。なければnullが帰る
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static TerrainTile GetTerrainTile(Vector3Int cell)
    {
        if(cell.z<0 || cell.z >= GameManager.groundLayers.Length) return null;//範囲外
        if (GameManager.groundLayers[cell.z].HasTile(cell))//移動先のセルがあるか
        {
            return GameManager.groundLayers[cell.z].GetTile<TerrainTile>(cell);
        }
        return null;
    }

    /// <summary>
    /// ワールド座標からセルへの変換を試みる
    /// </summary>
    /// <param name="worldpos"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryWorldToCell(Vector3 worldpos, out Vector3Int result)
    {
        for (int i = GameManager.groundLayers.Length - 1; i >= 0; i--)
        {
            var tm = GameManager.groundLayers[i];
            var cell = tm.WorldToCell(worldpos);
            cell.z = (int)(tm.transform.position.z * 2.0f);

            if (tm.HasTile(cell))
            {
                result = cell;
                return true;
            }
        }
        result = default;
        return false;
    }


   　/// <summary>
    /// セルインデックスからワールド座標へ変換する
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 Pos = new Vector3();

        Pos.y = cell.y * 0.75f+(cell.z/2.0f);
        if(cell.y%2 ==0)
        {
            Pos.x = cell.x;
        }else { Pos.x = cell.x + 0.5f; }
        Pos.z = cell.z/2.0f;

        return Pos;
    }

    public static Vector3 CellToScrean(Vector2Int cell)
    {

        Vector3 Pos = new Vector3();

        Pos.y = cell.y * 0.75f*120;
        if (cell.y % 2 == 0)
        {
            Pos.x = cell.x*120;
        }
        else { Pos.x = cell.x*120 + 60f; }

        return Pos;
    }

    /// <summary>
    /// 画面（ワールド）座標から“奇数行オフセット”方式のセル座標へ変換する<br/>
    /// <paramref name="pos"/> は <see cref="CellToScrean"/> が返す値と同じスケールを想定。
    /// </summary>
    /// <remarks>
    /// * 1 セル高さ：<c>rowStep = 0.75f * 120 = 90</c><br/>
    /// * 1 セル幅　：<c>colStep = 120</c><br/>
    /// * 奇数行は X が +60 オフセット
    /// </remarks>
    public static Vector2Int ScreenToCell(Vector3 pos)
    {
        const float rowStep = 0.75f * 120f; // 90
        const float colStep = 120f;         // 幅
        const float oddOffset = 60f;        // 奇数行ずれ

        //Y から行番号を推定
        int cy = Mathf.RoundToInt(pos.y / rowStep);

        //行の偶奇で X オフセットを補正し列番号を計算
        float correctedX = (cy % 2 == 0)
            ? pos.x
            : pos.x - oddOffset;

        int cx = Mathf.RoundToInt(correctedX / colStep);

        return new Vector2Int(cx, cy);
    }

    /// <summary>
    /// <paramref name="searchcell"/>が<paramref name="goalpos"/に>近づく為にどの方向に進めばよいかを返す
    /// </summary>
    /// <param name="searchcell"></param>
    /// <param name="goalpos"></param>
    /// <returns></returns>
    public static Vector3Int GetAngle(Vector3Int searchcell, Vector3 goalpos)
    {
        Vector3 nowPos = HexUtil.CellToWorld(searchcell);

        float angle = Mathf.Atan2((goalpos.y-nowPos.y),(goalpos.x-nowPos.x))*Mathf.Rad2Deg;
        switch(angle)
        {

            case <= 150 and > 90:
                if (searchcell.y % 2 == 0) return new Vector3Int(-1, 1, 0);
                else return new Vector3Int(0, 1, 0);
            case <= 90 and > 30:
                if (searchcell.y % 2 == 0) return new Vector3Int(0, 1, 0);
                else return new Vector3Int(1, 1, 0);
            case <= 30 and > -30:
                return new Vector3Int(1, 0, 0);
                
            case <= -30 and > -90:
                if (searchcell.y % 2 == 0) return new Vector3Int(0, -1, 0);
                else return new Vector3Int(1, -1, 0);
            case <= -90 and > -150:
                if (searchcell.y % 2 == 0) return new Vector3Int(-1, -1, 0);
                else return new Vector3Int(0, -1, 0);
            default:
                return new Vector3Int(-1, 0, 0);
        }
    }

    /// <summary>
    ///指定したセルと隣接するセルインデックスの集合を返す 
    /// </summary>
    /// <param name="cellpos"></param>
    /// <returns>隣接するセルインデックスの集合</returns>
    public static Vector3Int[] GetNeighborsCell(Vector3Int cellpos)//同一平面上のみ
    {
        Vector3Int[] NeighborsCell = new Vector3Int[6];
        if (cellpos.y % 2 == 0)
        {
            NeighborsCell[0] = cellpos + new Vector3Int(0, 1, 0);
            NeighborsCell[1] = cellpos + new Vector3Int(-1, 1, 0);
            NeighborsCell[2] = cellpos + new Vector3Int(1, 0, 0);
            NeighborsCell[3] = cellpos + new Vector3Int(-1, 0, 0);
            NeighborsCell[4] = cellpos + new Vector3Int(0, -1, 0);
            NeighborsCell[5] = cellpos + new Vector3Int(-1, -1, 0);
        }else
        {
            NeighborsCell[0] = cellpos + new Vector3Int(1, 1, 0);
            NeighborsCell[1] = cellpos + new Vector3Int(0, 1, 0);
            NeighborsCell[2] = cellpos + new Vector3Int(1, 0, 0);
            NeighborsCell[3] = cellpos + new Vector3Int(-1, 0, 0);
            NeighborsCell[4] = cellpos + new Vector3Int(1, -1, 0);
            NeighborsCell[5] = cellpos + new Vector3Int(0, -1, 0);
        }
        return NeighborsCell;
    }

    /// <summary>
    /// <param name="cellpos">から高さ<param name="h"></param>まで高低差を許容して移動可能なセルのListを返す
    /// </summary>
    /// <param name="cellpos"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public static Vector3Int[] GetEnableMoveCell(Vector3Int cellpos,int h)
    {
        List<Vector3Int> ReturnCellList = new List<Vector3Int>(); 
        foreach (Vector3Int cell in GetNeighborsCell(cellpos))
        {
            TerrainTile target = GetTerrainTile(cell);
            if(target == null)
            {
                for (int i = 1; i <= h; i++)
                {

                    Vector3Int searchcell = cell - new Vector3Int(0, 0, i);
                    target = GetTerrainTile(searchcell);
                    if (target !=null && target.type == TerrainTile.TerrainType.Ground) ReturnCellList.Add(searchcell);
                }
            }
            else if(target.type==TerrainTile.TerrainType.Ground) ReturnCellList.Add(cell);
            else if(target.type == TerrainTile.TerrainType.Wall)
            {
                for (int i = 1; i <= h; i++)
                {
                    
                    Vector3Int searchcell = cell + new Vector3Int(0, 0, i);
                    target = GetTerrainTile(searchcell);
                    if (target != null && target.type == TerrainTile.TerrainType.Ground) ReturnCellList.Add(searchcell);
                }
            }
        }
        return ReturnCellList.ToArray();
    }

    /// <summary>
    /// <param name="cellpos">の周囲のセル(平面上)のリストを得る
    /// </summary>
    /// <param name="cellpos"></param>
    /// <returns></returns>
    public static Vector2Int[] GetNeighborsCell(Vector2Int cellpos)//同一平面上のみ 右上から時計回りに取得
    {
        Vector2Int[] NeighborsCell = new Vector2Int[6];
        if (cellpos.y % 2 == 0)
        {
            NeighborsCell[0] = cellpos + new Vector2Int(0, 1);
            NeighborsCell[1] = cellpos + new Vector2Int(1, 0);
            NeighborsCell[2] = cellpos + new Vector2Int(0, -1);
            NeighborsCell[3] = cellpos + new Vector2Int(-1, -1);
            NeighborsCell[4] = cellpos + new Vector2Int(-1, 0);
            NeighborsCell[5] = cellpos + new Vector2Int(-1, 1);
        }
        else
        {
            NeighborsCell[0] = cellpos + new Vector2Int(1, 1);
            NeighborsCell[1] = cellpos + new Vector2Int(1, 0);
            NeighborsCell[2] = cellpos + new Vector2Int(1, -1);
            NeighborsCell[3] = cellpos + new Vector2Int(0, -1);
            NeighborsCell[4] = cellpos + new Vector2Int(-1, 0);
            NeighborsCell[5] = cellpos + new Vector2Int(0, 1);
        }
        return NeighborsCell;
    }

    /*A*アルゴリズムを別で実装したので不要になったが一応残しておく
    public static List<Vector3Int> GetRoot(Vector3Int startindex,Vector3 goalpos)
    {
        List<Vector3Int> root = new List<Vector3Int>();
        Vector3Int curent = startindex;
        Vector3 curentpos = HexUtil.CellToWorld(curent);
        int guard = 100;//無限ループ防止

        while (Mathf.Sqrt(Mathf.Pow(curentpos.x-goalpos.x,2)+ Mathf.Pow(curentpos.y - goalpos.y, 2))>0.6)
        {
            guard--;
            if (guard < 0) break;
            curent += GetAngle(curent, goalpos);
            if (HexUtil.HasTile(curent))
            {
                if(GetTerrainTile(curent).walkable == true && !GameManager.UnitPos.TryGetValue(curent,out int x)) root.Add(curent);
                else
                {
                    curent += new Vector3Int(0, 0, 1);
                    if(HexUtil.HasTile(curent)&& GetTerrainTile(curent).walkable == true && !GameManager.UnitPos.TryGetValue(curent, out int xx))
                    {
                        root.Add(curent);
                    }else break;
                }
            }
            else
            {
                curent += new Vector3Int(0, 0, -1);
                if (HexUtil.HasTile(curent) && GetTerrainTile(curent).walkable == true&& !GameManager.UnitPos.TryGetValue(curent, out int x))
                {
                    root.Add(curent);
                }
                else break;
            }
            curentpos = HexUtil.CellToWorld(curent);
        }


        return root;
    }
    */

    /// <summary>
    /// タイルが存在するか？
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static bool HasTile(Vector3Int cell)//範囲内かのチェックも行う
    {
        int tileindex = GameManager.groundLayers.Length - 1 - cell.z;
        if(tileindex < 0 || tileindex >= GameManager.groundLayers.Length) return false;
        return GameManager.groundLayers[tileindex].HasTile(cell);
    }
}
