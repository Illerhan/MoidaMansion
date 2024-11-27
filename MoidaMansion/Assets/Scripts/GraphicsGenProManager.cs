using UnityEngine;

public class GraphicsGenProManager : MonoBehaviour
{
    public static GraphicsGenProManager Instance;

    [Header("Parameters")] 
    [SerializeField] private DoorListSo doorList;
    [SerializeField] private ObjectSo stairsUp;
    [SerializeField] private ObjectSo stairsDown;
    [SerializeField] private RoomSo entranceSo;
    [SerializeField] private RoomSo librarySo;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    public void GenerateRoomTypes(Room[,] map)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if(Random.Range(0, 2) == 0)
                    map[x, y].roomSo = entranceSo;
                else
                    map[x, y].roomSo = librarySo;
            }
        }
    }
    
    public void GenerateRoomDoors(Room[,] map)
    {
        int previous = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (x == 0)
                {
                    map[x, y].leftDoor = doorList.LeftDoors[doorList.LeftDoors.Count - 1];
                    previous = Random.Range(0, doorList.RightDoors.Count - 1);
                    map[x, y].rightDoor = doorList.RightDoors[previous];
                }
                else if (x != 3)
                {
                    map[x, y].leftDoor = doorList.LeftDoors[previous];
                    previous = Random.Range(0, doorList.RightDoors.Count - 1);
                    map[x, y].rightDoor = doorList.RightDoors[previous];
                }
                else
                {
                    map[x, y].leftDoor = doorList.LeftDoors[previous];
                    map[x, y].rightDoor = doorList.RightDoors[doorList.RightDoors.Count - 1];
                }
            }
        }
    }
    
    public void GenerateStairs(Room[,] map)
    {
        int previous = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (map[x, y].connectedUp)
                {
                    map[x, y].stairs = stairsUp;
                }
                if (map[x, y].connectedDown)
                {
                    map[x, y].stairs = stairsDown;
                }
            }
        }
    }
}
