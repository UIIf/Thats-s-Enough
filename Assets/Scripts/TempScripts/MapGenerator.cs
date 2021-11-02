using UnityEngine;
using System.Collections.Generic;

public class RoomAndDist:MonoBehaviour
{
    public GameObject gO;
    public float sDistance;

    public RoomAndDist(GameObject g, Vector3 pos)
    {
        gO = g;
        this.ResetDist(pos);
    }
    ~RoomAndDist()
    {
        print("n");
        Destroy(gO);
    }
    public void ResetDist(Vector3 position)
    {
        sDistance = (gO.transform.position - position).sqrMagnitude;
    }
}
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject folowing;
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private float wallSize;
    [SerializeField] private int maxRoomCount;

    [SerializeField] private GameObject[] outerWX;
    [SerializeField] private GameObject[] outerWZ;
    [SerializeField] private GameObject roomBase;
    [SerializeField] private GameObject door;

    private Transform _folowingTransform, _transform;
    private Vector3 temp;
    private float maximilian;

    private GameObject temporaryGameObj;
    private GameObject generatedRoom;
    private List<RoomAndDist> generatedMap = new List<RoomAndDist>();
    private List<GameObject> Door;

    private int outerLayerMask;
    private int floorLayerMask;
    private int doorLayerMask;

    void Awake()
    {
        wallSize /= 2;

        _folowingTransform = folowing.GetComponent<Transform>();
        _transform = GetComponent<Transform>();

        outerLayerMask = LayerMask.GetMask("outerWall");
        floorLayerMask = LayerMask.GetMask("floor");
        doorLayerMask = LayerMask.GetMask("Door");

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
        Collider[] col = Physics.OverlapBox(toDel.gO.transform.position, new Vector3(sizeX,1,sizeZ), _transform.rotation, doorLayerMask);
        for(int i = 0; i < col.Length; i++)
        {
            Destroy(col[i].gameObject.transform.parent.gameObject);
        }
        Destroy(toDel.gO);
        Destroy(toDel);
    }

    //Создает стену и дверь
    GameObject GenerateWall(float x, float z, bool hor = true)
    {
        GameObject to_ret;
        Vector3 centrOfWall = new Vector3(x,0,z);
        Collider[] col = Physics.OverlapBox(centrOfWall, Vector3.one, _transform.rotation, outerLayerMask);

        if (col.Length > 0)
        {
            to_ret = Instantiate(col[0].gameObject.transform.parent.gameObject, centrOfWall, _transform.rotation);
            if (hor)
            {
                if (to_ret.transform.GetChild(0).name == "door")
                {
                    temporaryGameObj = Instantiate(door, new Vector3((x + col[0].transform.position.x)/2, 0 , to_ret.transform.GetChild(0).position.z), Quaternion.Euler(Vector3.up *(90 + 90* Mathf.Sign(Random.value - 0.5f))));
                }
            }
            else
            {
                if (to_ret.transform.GetChild(0).name == "door")
                {
                    
                    temporaryGameObj = Instantiate(door, new Vector3(to_ret.transform.GetChild(0).position.x, 0, (z + col[0].transform.position.z) / 2), Quaternion.Euler(Vector3.up * 90 * Mathf.Sign(Random.value - 0.5f)));
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


    //Создает комнату,пол и тп и тд
    GameObject GenerateRoom(int iX, int iZ)
    {
        float cZ = _transform.position.z + sizeZ * iZ * 2;
        float cX = _transform.position.x + sizeX * iX * 2;
        if (Physics.OverlapBox(new Vector3(cX, 0, cZ), Vector3.one, _transform.rotation, floorLayerMask).Length <= 0)
        {
            generatedRoom = new GameObject("room" + cX.ToString()+ ":" + cZ.ToString());
            generatedRoom.transform.position = new Vector3(cX, 0, cZ);
            GenerateWall(cX - sizeX + wallSize, cZ).transform.parent = generatedRoom.transform;
            GenerateWall(cX + sizeX - wallSize, cZ).transform.parent = generatedRoom.transform;
            GenerateWall(cX, cZ - sizeZ + wallSize, false).transform.parent = generatedRoom.transform;
            GenerateWall(cX, cZ + sizeZ - wallSize, false).transform.parent = generatedRoom.transform;
            Instantiate(roomBase, new Vector3(cX, 0, cZ), _transform.rotation).transform.parent = generatedRoom.transform;
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
}
