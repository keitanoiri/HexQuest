using System.Collections.Generic;
using UnityEngine;



public class HexPathFinder : MonoBehaviour
{
    // 作業用データ構造（ヒープ・辞書）―― インスタンス間で共有しない
    private readonly BinaryHeap _open = new();
    private readonly HashSet<Vector3Int> _closed = new();
    private readonly Dictionary<Vector3Int, int> _g = new();
    private readonly Dictionary<Vector3Int, Vector3Int> _parent = new();

    /// <summary>
    ///  A* で最短経路を検索する。
    /// </summary>
    /// <param name="start">スタート座標</param>
    /// <param name="goal">ゴール座標</param>
    /// <returns>
    ///  スタート→ゴールの座標リスト（両端含む）。  
    ///  進行不能なら <c>null</c> を返還。
    /// </returns>
    /// 
    #nullable enable        // これが必須
    public List<Vector3Int>? FindPath(Vector3Int start, Vector3Int goal)
    {
        // 0. 準備 ------------------------------------------------------------
        _open.Clear(); _closed.Clear(); _g.Clear(); _parent.Clear();

        _g[start] = 0;
        _open.Enqueue(start, Heuristic(start, goal));

        // 1. メインループ ----------------------------------------------------
        while (_open.Count > 0)
        {
            var cur = _open.Dequeue();

            // 1-a. ゴール判定
            if (cur.Equals(goal))
                return Reconstruct(cur);

            _closed.Add(cur);

            // 1-b. 隣接 4 方向をチェック
            foreach (Vector3Int nxt in HexUtil.GetEnableMoveCell(cur,1))
            {
                //var nxt = new Point(cur.x + dx, cur.y + dy);

                //ユニットがすでに存在するマスであれば判定できない？
                if (GameManager.UnitPos.TryGetValue(nxt, out var pos)) continue;

                int tentativeG = _g[cur] + 1;          // 基本コスト = 1

                // より短い経路を発見したら情報を更新
                if (!_g.TryGetValue(nxt, out int oldG) || tentativeG < oldG)
                {
                    _parent[nxt] = cur;
                    _g[nxt] = tentativeG;
                    int f = tentativeG + Heuristic(nxt, goal);
                    _open.Enqueue(nxt, f);
                }
            }
        }
        // 2. open が空 → ゴールまで辿り着けない
        return null;
    }

    #region ─── 内部ユーティリティ ────────────────────────────────────

    /// <summary>
    ///  ヒューリスティック距離を使用するのでそれの算出用。  
    /// </summary>
    private static int Heuristic(Vector3Int a, Vector3Int b)
        => (a.x-b.x)^2+(a.y-b.y)^2+(a.z-b.z)^2;

    /// <summary>parent 辞書を遡って経路を復元</summary>
    private List<Vector3Int> Reconstruct(Vector3Int node)
    {
        var list = new List<Vector3Int> { node };
        while (_parent.TryGetValue(node, out node))
            list.Add(node);
        list.Reverse();  // スタート→ゴール順に
        list.RemoveAt(0);//第一要素(スタート位置自分の位置)を除く
        return list;
    }
    #endregion
}
