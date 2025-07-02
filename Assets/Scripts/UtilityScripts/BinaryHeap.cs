using System.Collections.Generic;
using UnityEngine;


#region ������ 2. �o�C�i���q�[�v (open ���X�g) ����������������������������������������������������������������
/// <summary>
///  �V���v���� <b>�ŏ��q�[�v</b> �����B�v�f�� <see cref="Point"/>�A
///  �D��x�� <c>int</c>�if = g + h�j�Ŕ�r���܂��B  
///  ���v���W�F�N�g�ł� .NET 6+ �� <c>PriorityQueue&lt;TElement,TPrio&gt;</c>
///  ��Ǝ� BinaryHeap/N-aryHeap �ɍ����ւ��Ă� OK �ł��B
/// </summary>
internal class BinaryHeap
{
    private readonly List<(Vector3Int p, int pri)> _heap = new();

    /// <summary>�q�[�v�Ɋi�[����Ă���v�f��</summary>
    public int Count => _heap.Count;

    /// <summary>�q�[�v�����</summary>
    public void Clear() => _heap.Clear();

    /// <summary>�w�肳�ꂽ�D��x�ŗv�f��ǉ�</summary>
    public void Enqueue(Vector3Int p, int priority)
    {
        _heap.Add((p, priority));
        BubbleUp(_heap.Count - 1);
    }

    /// <summary>�ŏ��D��x�if�l���ł��������j�v�f�����o��/// </summary>
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

    #region �q�[�v��������
    private void BubbleUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) >> 1;
            if (_heap[parent].pri <= _heap[i].pri) break;
            (_heap[parent], _heap[i]) = (_heap[i], _heap[parent]);//�X���b�v
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