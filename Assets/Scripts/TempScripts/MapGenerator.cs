using UnityEngine;
using System.Collections.Generic;

public class RoomAndDist
{
    public GameObject gO;
    public float sDistance;

    public RoomAndDist(GameObject g, Vector3 pos)
    {
        gO = g;
        this.ResetDist(pos);
    }
    public void ResetDist(Vector3 position)
    {
        sDistance = (gO.transform.position - position).sqrMagnitude;
    }
}
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject following;
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private float wallSize;
    [SerializeField] private int maxRoomCount;

    [SerializeField] private GameObject[] outerWX;
    [SerializeField] private GameObject[] outerWZ;
    [SerializeField] private GameObject roomBase;
    [SerializeField] private GameObject[] door;
    [SerializeField] private Material[] wallpapers;

    private Transform _folowingTransform, _transform;
    private Vector3 temp;
    private float maximilian;

    private GameObject temporaryGameObj;
    private GameObject generatedRoom;
    private List<RoomAndDist> generatedMap = new List<RoomAndDist>();

    private int outerLayerMask;
    private int floorLayerMask;
    private int doorLayerMask;

    void Awake()
    {
        wallSize /= 2;

        _folowingTransform = following.GetComponent<Transform>();
        _transform = GetComponent<Transform>();

        outerLayerMask = LayerMask.GetMask("outerWall");
        floorLayerMask = LayerMask.GetMask("floor");
        doorLayerMask = LayerMask.GetMask("door");

        GenerateRooms();
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
            GenerateRooms();
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
            GenerateRooms();
        }
    }


    //���������� ������� ������ �������
    void GenerateRooms()
    {
        generatedMap.ForEach((el1) => el1.ResetDist(_transform.position));
        generatedMap.Sort((emp2,emp1) => emp1.sDistance.CompareTo(emp2.sDistance));
        
        //Change size of ....
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                generatedRoom = GenerateRoom(i, j);
                if (!(generatedRoom is null))
                {
                    AddRoom();
                }
            }
        }
    }
    

    //��������� ������� � ������ �����
    void AddRoom()
    {
        while(generatedMap.Count >= maxRoomCount)
        {
            DeleteRoom(generatedMap[0]);
            generatedMap.RemoveAt(0);
        }
        generatedMap.Add(new RoomAndDist(generatedRoom, _transform.position));
    }


    //������� ������� � ����� ������� � ���
    void DeleteRoom(RoomAndDist toDel)
    {
        Collider[] col = Physics.OverlapBox(toDel.gO.transform.position, new Vector3(sizeX,1,sizeZ), _transform.rotation, doorLayerMask);
        for(int i = 0; i < col.Length; i++)
        {
            Destroy(col[i].gameObject.transform.parent.gameObject);
        }
        Destroy(toDel.gO);
    }

    //������� ����� � �����
    GameObject GenerateWall(float x, float z, bool hor = true)
    {
        GameObject to_ret;
        Vector3 centrOfWall = new Vector3(x,0,z);
        Collider[] col = Physics.OverlapBox(centrOfWall, Vector3.one, _transform.rotation, outerLayerMask);

        if (col.Length > 0)
        {
            to_ret = Instantiate(col[0].gameObject.transform.parent.gameObject, centrOfWall, _transform.rotation);

            for(int i = 0; i < to_ret.transform.childCount; i++)
            {
                if(to_ret.transform.GetChild(i).name == "door")
                {

                    float dx = (x + col[0].transform.position.x) / 2,
                        dz = to_ret.transform.GetChild(i).position.z;
                    Quaternion rot = Quaternion.Euler(Vector3.up * (90 + 90 * Mathf.Sign(Random.value - 0.5f)));

                    if (!hor)
                    {
                        dx = to_ret.transform.GetChild(i).position.x;
                        dz = (z + col[0].transform.position.z) / 2;
                        rot = Quaternion.Euler(Vector3.up * 90 * Mathf.Sign(Random.value - 0.5f));
                    }

                    Instantiate(RandomDoor(), new Vector3(dx, 0, dz), rot);
                }

            }
        }
        else
        {
            if(hor)
                to_ret = Instantiate(RandomXWall(), centrOfWall, _transform.rotation);
            else
                to_ret = Instantiate(RandomZWall(), centrOfWall, _transform.rotation);
        }
        return to_ret;
    }


    //������� �������,��� � �� � ��
    GameObject GenerateRoom(int iX, int iZ)
    {
        float cZ = _transform.position.z + sizeZ * iZ * 2;
        float cX = _transform.position.x + sizeX * iX * 2;
        if (Physics.OverlapBox(new Vector3(cX, 0, cZ), Vector3.one, _transform.rotation, floorLayerMask).Length <= 0)
        {
            generatedRoom = new GameObject("room" + cX.ToString()+ ":" + cZ.ToString());
            generatedRoom.transform.position = new Vector3(cX, 0, cZ);
            Material roomMat = RandomWallpaper();
            GameObject[] generatedWalls = new GameObject[4];
            generatedWalls[0] = GenerateWall(cX - sizeX + wallSize, cZ);
            generatedWalls[1] = GenerateWall(cX + sizeX - wallSize, cZ);
            generatedWalls[2] = GenerateWall(cX, cZ - sizeZ + wallSize, false);
            generatedWalls[3] = GenerateWall(cX, cZ + sizeZ - wallSize, false);
            for(int i = 0; i < generatedWalls.Length; i++)
            {
                generatedWalls[i].transform.parent = generatedRoom.transform;
                for (int j = 0; j < generatedWalls[i].transform.childCount; j++)
                {
                    if (generatedWalls[i].transform.GetChild(j).GetComponent<MeshRenderer>())
                    {
                        generatedWalls[i].transform.GetChild(j).GetComponent<MeshRenderer>().material = roomMat;
                    }
                }
            }
            
            Instantiate(roomBase, new Vector3(cX, 0, cZ), _transform.rotation).transform.parent = generatedRoom.transform;
        }
        else
            generatedRoom = null;
        return generatedRoom;
    }
    
    //���������� ��������� ����� ������������
    GameObject RandomXWall()
    {
        return outerWX[Random.Range(0, outerWX.Length)];
    }

    //���������� ��������� ����� ��������������
    GameObject RandomZWall()
    {
        return outerWZ[Random.Range(0, outerWZ.Length)];
    }

    GameObject RandomDoor()
    {
        return door[Random.Range(0, door.Length)];
    }

    Material RandomWallpaper()
    {
        return wallpapers[Random.Range(0, wallpapers.Length)];
    }
}
