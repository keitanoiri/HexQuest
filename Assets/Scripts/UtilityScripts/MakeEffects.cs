using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeEffects : MonoBehaviour
{
    static List<GameObject> InstamceObjects = new List<GameObject>();
    static GameObject _prefab;

    const string EffectPath = "Effects/SelectEffect"; // Resources からの相対パス
    /// <summary>
    /// 全てのエフェクトを削除する(Listもクリア)
    /// </summary>
    public static void DestroyAllEffect()
    {
        foreach (var go in InstamceObjects)　if (go)Destroy(go); 
        InstamceObjects.Clear();

    }

    /// <summary>
    /// エフェクトを作成しlistに保存する
    /// </summary>
    /// <param name="Cell"></param>
    public static void MakeSerectEffect(Vector3Int Cell)
    {
        if (_prefab==null) _prefab = Resources.Load<GameObject>(EffectPath);
        InstamceObjects.Add(Instantiate(_prefab, HexUtil.CellToWorld(Cell) + new Vector3(0, 0, 0.4f), Quaternion.identity));
    }
    /// <summary>
    /// もしListが空ならエフェクトを生成する
    /// </summary>
    /// <param name="Cell"></param>
    public static void MakeSerectEffectOnes(Vector3Int Cell)
    {
        if (_prefab == null) _prefab = Resources.Load<GameObject>(EffectPath);
        if (InstamceObjects.Count == 0)
        {
            InstamceObjects.Add(Instantiate(_prefab, HexUtil.CellToWorld(Cell) + new Vector3(0, 0, 0.4f), Quaternion.identity));
        }
        else
        {
            Debug.Log("既に生成済み");
        }
    }

}
