using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Unit PlayerUnit;
    // Start is called before the first frame update
    void Start()
    {
        PlayerUnit = GameManager.AllUnits[1];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerUnit.transform.position+new Vector3Int(0,0,10);
    }
}
