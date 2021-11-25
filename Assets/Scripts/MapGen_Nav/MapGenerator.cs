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
    [SerializeField] private float MaxDist;


    [SerializeField] private GameObject[] outerWX;
    [SerializeField] private GameObject blockWX;
    [SerializeField] private GameObject[] outerWZ;
    [SerializeField] private GameObject blockWZ;
    [SerializeField] private GameObject[] roomInner;
    [SerializeField] private GameObject[] door;
    [SerializeField] private GameObject floor;

    [Space]

    [SerializeField] private Material[] wallpapers;
    [SerializeField] private Material[] floorMaterial;

    [Space]

    [SerializeField] private GameObject link;

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


    //Генерирует комнаты вокруг объекта
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
    

    //Добавляет комнату в массив карты
    void AddRoom()
    {
        while(generatedMap.Count >= maxRoomCount)
        {
            DeleteRoom(generatedMap[0]);
            generatedMap.RemoveAt(0);
        }
        generatedMap.Add(new RoomAndDist(generatedRoom, _transform.position));
    }


    //Удаляет комнату и двери ведущие в нее
    void DeleteRoom(RoomAndDist toDel)
    {
        Collider[] col = Physics.OverlapBox(toDel.gO.transform.position, new Vector3(sizeX,1,sizeZ), _transform.rotation, LayerMask.GetMask("door", "weapon", "enemy"));
        for(int i = 0; i < col.Length; i++)
        {
            Destroy(col[i].gameObject.transform.parent.gameObject);
        }
        Destroy(toDel.gO);
    }

    //Создает стену и дверь
    GameObject GenerateWall(float x, float z, bool hor = true)
    {
        GameObject to_ret;
        Vector3 centrOfWall = new Vector3(x,0,z);
        if (Mathf.Abs(x) > MaxDist || Mathf.Abs(z) > MaxDist)
        {
            if (hor)
                to_ret = Instantiate(blockWX, centrOfWall, _transform.rotation);
            else
                to_ret = Instantiate(blockWZ, centrOfWall, _transform.rotation);
        }
        else
        {
            Collider[] col = Physics.OverlapBox(centrOfWall, Vector3.one, _transform.rotation, outerLayerMask);

            if (col.Length > 0)
            {
                to_ret = Instantiate(col[0].gameObject.transform.parent.gameObject, centrOfWall, _transform.rotation);

                for (int i = 0; i < to_ret.transform.childCount; i++)
                {
                    if (to_ret.transform.GetChild(i).name == "door")
                    {

                        to_ret.transform.GetChild(i).Rotate(Vector3.up * (180 * Random.Range(0, 2)));

                        Instantiate(link, to_ret.transform.GetChild(i).transform).transform.parent = Instantiate(RandomDoor(), to_ret.transform.GetChild(i).transform).transform; ;
                    }

                }
            }
            else
            {
                if (hor)
                    to_ret = Instantiate(RandomXWall(), centrOfWall, _transform.rotation);
                else
                    to_ret = Instantiate(RandomZWall(), centrOfWall, _transform.rotation);
            }
        }
        return to_ret;
    }


    void AddInnerDoor(GameObject wall)
    {
        for (int i = 0; i < wall.transform.childCount; i++)
        {
            if (wall.transform.GetChild(i).name == "door")
            {
                wall.transform.GetChild(i).Rotate(Vector3.up * (180 * Random.Range(0, 2)));
                
                Instantiate(RandomDoor(), wall.transform.GetChild(i).transform);
            }

        }
    }

    //Создает комнату,пол и тп и тд
    GameObject GenerateRoom(int iX, int iZ)
    {
        float cZ = _transform.position.z + sizeZ * iZ * 2;
        float cX = _transform.position.x + sizeX * iX * 2;
        if (Physics.OverlapBox(new Vector3(cX, 0, cZ), Vector3.one, _transform.rotation, floorLayerMask).Length <= 0)
        {
            generatedRoom = new GameObject("room" + cX.ToString()+ ":" + cZ.ToString());
            generatedRoom.transform.position = new Vector3(cX, 0, cZ);
            Material roomMat = RandomWallpaper();
            GameObject[] generatedWalls = new GameObject[5];
            generatedWalls[0] = GenerateWall(cX - sizeX + wallSize, cZ);
            generatedWalls[1] = GenerateWall(cX + sizeX - wallSize, cZ);
            generatedWalls[2] = GenerateWall(cX, cZ - sizeZ + wallSize, false);
            generatedWalls[3] = GenerateWall(cX, cZ + sizeZ - wallSize, false);
            generatedWalls[4] = Instantiate(RandomRoomInner(), new Vector3(cX, 0, cZ), Quaternion.Euler(new Vector3(0, Random.Range(0,2) * 180 ,0)));
            for (int i = 0; i < generatedWalls.Length; i++)
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

            AddInnerDoor(generatedWalls[4]);

            temporaryGameObj = Instantiate(floor, new Vector3(cX, 0, cZ), _transform.rotation);
            temporaryGameObj.transform.parent = generatedRoom.transform;
            temporaryGameObj.GetComponent<MeshRenderer>().material = RandomFloorMaterial();
        }
        else
            generatedRoom = null;
        return generatedRoom;
    }
    
    //Возвращает случайную стену вертикальную
    GameObject RandomXWall()
    {
        return outerWX[Random.Range(0, outerWX.Length)];
    }

    //Возвращает случайную стену горизонтальную
    GameObject RandomZWall()
    {
        return outerWZ[Random.Range(0, outerWZ.Length)];
    }

    GameObject RandomDoor()
    {
        return door[Random.Range(0, door.Length)];
    }

    GameObject RandomRoomInner()
    {
        return roomInner[Random.Range(0, roomInner.Length)];
    }

    Material RandomWallpaper()
    {
        return wallpapers[Random.Range(0, wallpapers.Length)];
    }
    Material RandomFloorMaterial()
    {
        return floorMaterial[Random.Range(0, floorMaterial.Length)];
    }
}
