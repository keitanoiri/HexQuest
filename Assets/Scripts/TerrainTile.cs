using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom Tiles/TerrainTile")]
public class TerrainTile : Tile
{
    // ��������������������������������������������������������������������������������������������������������������������������
    #region 1. �񋓌^�� �g�n�`�̎�ށh ���`
    /// <summary>
    ///  �^�C���̒n�`����  
    ///  �����ɐV������ނ�ǉ������Inspector�̑I�����Ɏ������f�����
    /// </summary>
    public enum TerrainType
    {
        Ground,     // �ʏ�n��
        Wall,       // ���S��Q��
        Water,      // ���ʁi���s���j�b�g�s�A�M�E��s�͉� �Ȃǁj
        Mud,        // �ړ��R�X�g��
    }
    #endregion

    #region 2. Inspector �֌��J����t�B�[���h
    [Tooltip("�n�`�̎��")]
    public TerrainType type = TerrainType.Ground;

    [Tooltip("��{�ړ��R�X�g�Btype ��ύX����Ǝ����ōX�V")]
    public int moveCost = 1;

    [Tooltip("���j�b�g�����Ă邩�ǂ���")]
    public bool walkable = true;

    [Tooltip("���j�b�gID: 0 = �����Ȃ� / >0 = ���̃��j�b�g��p�Ȃ�")]
    public int unitID = 0;
    #endregion

}
