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

    void Awake()
    {
        wallSize /= 2;
        _folowingTransform = folowing.GetComponent<Transform>();
        _transform = GetComponent<Transform>();
        //FirstGenerateRoom();
        FirstGenerateRoom();
        //ClearRoom(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveGenerator();
    }

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
        }
    }

    void FirstGenerateRoom()
    {
        generatedMap = new GameObject[25][];
        generatedRoom = new GameObject[6];
        generatedRoom[0] = Instantiate(RandomXWall(), new Vector3(_transform.position.x - sizeX + wallSize, 0, _transform.position.z), _transform.rotation);
        generatedRoom[1] = Instantiate(RandomXWall(), new Vector3(_transform.position.x + sizeX - wallSize, 0, _transform.position.z), _transform.rotation);
        generatedRoom[2] = Instantiate(RandomZWall(), new Vector3(_transform.position.x, 0, _transform.position.z - sizeZ + wallSize), _transform.rotation);
        generatedRoom[3] = Instantiate(RandomZWall(), new Vector3(_transform.position.x, 0, _transform.position.z + sizeZ - wallSize), _transform.rotation);
        generatedRoom[4] = Instantiate(floor, new Vector3(_transform.position.x, 0, _transform.position.z), floor.transform.rotation);
        //generatedRoom[4] = Instantiate(corners, new Vector3(_transform.position.x, 0, _transform.position.z), _transform.rotation);
        generatedMap[12] = generatedRoom;
        GenerateLeft(1, 2);
        GenerateRight(3, 2);
    }

    void GenerateLeft(int indexX,int indexZ)
    {
        if (indexX >= 0 && indexX < 5)
        {
            generatedRoom = new GameObject[6];
            generatedRoom[0] = Instantiate(RandomXWall(), new Vector3(_transform.position.x - sizeX + wallSize - (2 - indexX) * sizeX * 2, 0, _transform.position.z), _transform.rotation);
            generatedRoom[1] = Instantiate(generatedMap[indexZ * 5 + indexX+1][0], new Vector3(_transform.position.x + sizeX - wallSize - (2 - indexX) * sizeX * 2, 0, _transform.position.z), _transform.rotation);
            generatedRoom[2] = Instantiate(RandomZWall(), new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z - sizeZ + wallSize), _transform.rotation);
            generatedRoom[3] = Instantiate(RandomZWall(), new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z + sizeZ - wallSize), _transform.rotation);
            generatedRoom[4] = Instantiate(floor, new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z), floor.transform.rotation);
            generatedMap[indexZ * 5 + indexX] = generatedRoom;
            GenerateLeft(indexX-1, indexZ);
        }
    }

    void GenerateRight(int indexX, int indexZ)
    {
        if (indexX >= 0 && indexX < 5)
        {
            generatedRoom = new GameObject[6];
            generatedRoom[0] = Instantiate(generatedMap[indexZ * 5 + indexX - 1][1], new Vector3(_transform.position.x - sizeX + wallSize - (2 - indexX) * sizeX * 2, 0, _transform.position.z), _transform.rotation);
            generatedRoom[1] = Instantiate(RandomXWall(), new Vector3(_transform.position.x + sizeX - wallSize - (2 - indexX) * sizeX * 2, 0, _transform.position.z), _transform.rotation);
            generatedRoom[2] = Instantiate(RandomZWall(), new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z - sizeZ + wallSize), _transform.rotation);
            generatedRoom[3] = Instantiate(RandomZWall(), new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z + sizeZ - wallSize), _transform.rotation);
            generatedRoom[4] = Instantiate(floor, new Vector3(_transform.position.x - (2 - indexX) * sizeX * 2, 0, _transform.position.z), floor.transform.rotation);
            generatedMap[indexZ * 5 + indexX] = generatedRoom;
            GenerateRight(indexX + 1, indexZ);
        }
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
