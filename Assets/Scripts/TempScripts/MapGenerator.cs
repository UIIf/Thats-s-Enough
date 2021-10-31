using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject folowing;
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private float wallSize;

    [SerializeField] private GameObject[] outerWX;
    [SerializeField] private GameObject[] outerWZ;
    [SerializeField] private GameObject corners;
    [SerializeField] private GameObject floor;

    private Transform _folowingTransform, _transform;
    private Vector3 temp;
    private float maximilian;

    private GameObject temporaryGameObj;
    private GameObject[] generatedRoom;
    private GameObject[][] generatedMap;

    private int outerLayerMask;
    private int floorLayerMask;

    void Awake()
    {
        wallSize /= 2;
        _folowingTransform = folowing.GetComponent<Transform>();
        _transform = GetComponent<Transform>();
        outerLayerMask = LayerMask.GetMask("outerWall");
        floorLayerMask = LayerMask.GetMask("floor");
        FirstGenerateRoom();
    }

    void FixedUpdate()
    {
        MoveGenerator();
    }


    //Make discrete movements
    void MoveGenerator()
    {
        temp = (_folowingTransform.position - _transform.position);

        if (Mathf.Abs(temp.x) > sizeX)
        {
            maximilian = (int)_folowingTransform.position.x / sizeX;
            if (maximilian%2 != 0)
                maximilian += Mathf.Sign(temp.x);

            _transform.position = new Vector3(
                maximilian * sizeX,
                _transform.position.y,
                _transform.position.z
                );
            generatedRoom = GenerateRoom(-1, 0);
            generatedRoom = GenerateRoom(1, 0);
            generatedRoom = GenerateRoom(0, 0);
            generatedRoom = GenerateRoom(0, 1);
            generatedRoom = GenerateRoom(0, -1);
        }

        if (Mathf.Abs(temp.z) > sizeZ)
        {
            maximilian = (int)_folowingTransform.position.z / sizeZ;
            if (maximilian%2 != 0)
                maximilian += Mathf.Sign(temp.z);

            _transform.position = new Vector3(
                _transform.position.x,
                _transform.position.y,
                maximilian * sizeZ
                );
            generatedRoom = GenerateRoom(-1, 0);
            generatedRoom = GenerateRoom(1, 0);
            generatedRoom = GenerateRoom(0, 0);
            generatedRoom = GenerateRoom(0, 1);
            generatedRoom = GenerateRoom(0, -1);
        }
    }

    void FirstGenerateRoom()
    {
        generatedRoom = GenerateRoom(-1, 0);
        generatedRoom = GenerateRoom(1, 0);
        generatedRoom = GenerateRoom(0, 0);
        generatedRoom = GenerateRoom(0, 1);
        generatedRoom = GenerateRoom(0, -1);
    }

    GameObject GenerateWall(float x, float z, bool hor = true)
    {
        GameObject to_ret;
        Collider[] col = Physics.OverlapBox(new Vector3(x, 0, z), Vector3.one, _transform.rotation, outerLayerMask);

        print(col.Length);
        if (col.Length > 0)
        {
            to_ret = Instantiate(col[0].gameObject.transform.parent.gameObject, new Vector3(x, 0, z), _transform.rotation);
        }
        else
        {
            if(hor)
                to_ret = Instantiate(RandomXWall(), new Vector3(x, 0, z), _transform.rotation);
            else
                to_ret = Instantiate(RandomZWall(), new Vector3(x, 0, z), _transform.rotation);
        }
        return to_ret;
    }

    GameObject[] GenerateRoom(int iX, int iZ)
    {
        float cZ = _transform.position.z + sizeZ * iZ * 2;
        float cX = _transform.position.x + sizeX * iX * 2;
        if (Physics.OverlapBox(new Vector3(cX, 0, cZ), Vector3.one, _transform.rotation, floorLayerMask).Length <= 0)
        {
            generatedRoom = new GameObject[6];
            generatedRoom[0] = GenerateWall(cX - sizeX + wallSize, cZ);
            generatedRoom[1] = GenerateWall(cX + sizeX - wallSize, cZ);
            generatedRoom[2] = GenerateWall(cX, cZ - sizeZ + wallSize, false);
            generatedRoom[3] = GenerateWall(cX, cZ + sizeZ - wallSize, false);
            generatedRoom[4] = Instantiate(floor, new Vector3(cX, 0, cZ), floor.transform.rotation);
            generatedRoom[5] = Instantiate(corners, new Vector3(cX, 0, cZ), _transform.rotation);
        }
        else
            generatedRoom = null;
        return generatedRoom;
    }
    

    GameObject RandomXWall()
    {
        return outerWX[Random.Range(0, outerWX.Length)];
    }
    GameObject RandomZWall()
    {
        return outerWZ[Random.Range(0, outerWZ.Length)];
    }
}
