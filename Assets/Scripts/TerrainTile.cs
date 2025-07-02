using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom Tiles/TerrainTile")]
public class TerrainTile : Tile
{
    // ─────────────────────────────────────────────────────────────
    #region 1. 列挙型で “地形の種類” を定義
    /// <summary>
    ///  タイルの地形属性  
    ///  ここに新しい種類を追加するとInspectorの選択肢に自動反映される
    /// </summary>
    public enum TerrainType
    {
        Ground,     // 通常地面
        Wall,       // 完全障害物
        Water,      // 水面（歩行ユニット不可、舟・飛行は可 など）
        Mud,        // 移動コスト増
    }
    #endregion

    #region 2. Inspector へ公開するフィールド
    [Tooltip("地形の種類")]
    public TerrainType type = TerrainType.Ground;

    [Tooltip("基本移動コスト。type を変更すると自動で更新")]
    public int moveCost = 1;

    [Tooltip("ユニットが立てるかどうか")]
    public bool walkable = true;

    [Tooltip("ユニットID: 0 = 何もない / >0 = そのユニット専用など")]
    public int unitID = 0;
    #endregion

}
