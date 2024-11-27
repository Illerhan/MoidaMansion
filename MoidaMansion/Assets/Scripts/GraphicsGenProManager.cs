using UnityEngine;

public class GraphicsGenProManager : MonoBehaviour
{
    public static GraphicsGenProManager Instance;

    [Header("Parameters")] [SerializeField]
    private DoorListSo doorList;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
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
                    previous = Random.Range(0, doorList.RightDoors.Count);
                    map[x, y].rightDoor = doorList.RightDoors[previous];
                }
                else if (x != 3)
                {
                    map[x, y].leftDoor = doorList.RightDoors[previous];
                    previous = Random.Range(0, doorList.RightDoors.Count);
                    map[x, y].rightDoor = doorList.RightDoors[previous];
                }
                else
                {
                    map[x, y].leftDoor = doorList.RightDoors[previous];
                }
            }
        }
    }
}
