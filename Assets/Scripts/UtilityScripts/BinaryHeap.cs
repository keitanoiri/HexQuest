using System.Collections.Generic;
using UnityEngine;


#region ─── 2. バイナリヒープ (open リスト) ────────────────────────────────
/// <summary>
///  シンプルな <b>最小ヒープ</b> 実装。要素は <see cref="Point"/>、
///  優先度は <c>int</c>（f = g + h）で比較します。  
///  実プロジェクトでは .NET 6+ の <c>PriorityQueue&lt;TElement,TPrio&gt;</c>
///  や独自 BinaryHeap/N-aryHeap に差し替えても OK です。
/// </summary>
internal class BinaryHeap
{
    private readonly List<(Vector3Int p, int pri)> _heap = new();

    /// <summary>ヒープに格納されている要素数</summary>
    public int Count => _heap.Count;

    /// <summary>ヒープを空に</summary>
    public void Clear() => _heap.Clear();

    /// <summary>指定された優先度で要素を追加</summary>
    public void Enqueue(Vector3Int p, int priority)
    {
        _heap.Add((p, priority));
        BubbleUp(_heap.Count - 1);
    }

    /// <summary>最小優先度（f値が最も小さい）要素を取り出す/// </summary>
    public Vector3Int Dequeue()
    {
        var root = _heap[0].p;
        var last = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);

        if (_heap.Count > 0)
        {
            _heap[0] = last;
            BubbleDown(0);
        }
        return root;
    }

    #region ヒープ内部操作
    private void BubbleUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) >> 1;
            if (_heap[parent].pri <= _heap[i].pri) break;
            (_heap[parent], _heap[i]) = (_heap[i], _heap[parent]);//スワップ
            i = parent;
        }
    }
    private void BubbleDown(int i)
    {
        int n = _heap.Count;
        while (true)
        {
            int l = i * 2 + 1;
            if (l >= n) break;
            int r = l + 1;
            int smallest = (r < n && _heap[r].pri < _heap[l].pri) ? r : l;
            if (_heap[i].pri <= _heap[smallest].pri) break;
            (_heap[i], _heap[smallest]) = (_heap[smallest], _heap[i]);
            i = smallest;
        }
    }
    #endregion
}
#endregion