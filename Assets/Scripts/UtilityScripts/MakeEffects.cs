using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeEffects : MonoBehaviour
{
    static List<GameObject> InstamceObjects = new List<GameObject>();
    static GameObject _prefab;

    const string EffectPath = "Effects/SelectEffect"; // Resources ����̑��΃p�X
    /// <summary>
    /// �S�ẴG�t�F�N�g���폜����(List���N���A)
    /// </summary>
    public static void DestroyAllEffect()
    {
        foreach (var go in InstamceObjects)�@if (go)Destroy(go); 
        InstamceObjects.Clear();

    }

    /// <summary>
    /// �G�t�F�N�g���쐬��list�ɕۑ�����
    /// </summary>
    /// <param name="Cell"></param>
    public static void MakeSerectEffect(Vector3Int Cell)
    {
        if (_prefab==null) _prefab = Resources.Load<GameObject>(EffectPath);
        InstamceObjects.Add(Instantiate(_prefab, HexUtil.CellToWorld(Cell) + new Vector3(0, 0, 0.4f), Quaternion.identity));
    }
    /// <summary>
    /// ����List����Ȃ�G�t�F�N�g�𐶐�����
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
            Debug.Log("���ɐ����ς�");
        }
    }

}
