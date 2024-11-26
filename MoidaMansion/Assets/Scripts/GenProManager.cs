using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenProManager : MonoBehaviour
{
    public static GenProManager Instance;

    [Header("Public Infos")]
    public Room[,] mansionMap = new Room[4, 3];
    public Vector2Int[] friendPositions = new Vector2Int[3];
    public Vector2Int currentPos;

    [Header("References")] 
    [SerializeField] private GameObject roomPrefabDebug;
    [SerializeField] private GameObject pathPrefabDebug;
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateMansion();
    }


    private void DebugDisplayMap()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GameObject room = Instantiate(roomPrefabDebug, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
                if (mansionMap[x, y].isStart)
                {
                    room.GetComponent<SpriteRenderer>().color = Color.red;
                }

                if (mansionMap[x, y].connectedUp)
                {
                    Instantiate(pathPrefabDebug, new Vector3(x, y + 0.5f, 0), Quaternion.Euler(0, 0, 90));
                }
                if (mansionMap[x, y].connectedDown)
                {
                    Instantiate(pathPrefabDebug, new Vector3(x, y - 0.5f, 0), Quaternion.Euler(0, 0, 90));
                }
                if (mansionMap[x, y].connectedLeft)
                {
                    Instantiate(pathPrefabDebug, new Vector3(x - 0.5f, y, 0), Quaternion.Euler(0, 0, 0));
                }
                if (mansionMap[x, y].connectedRight)
                {
                    Instantiate(pathPrefabDebug, new Vector3(x + 0.5f, y, 0), Quaternion.Euler(0, 0, 0));
                }
            }
        }
    }
    

    private void GenerateMansion()
    {
        mansionMap = new Room[4, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                mansionMap[x, y] = new Room();
            }
        }
        
        
        Vector2Int startPos = new Vector2Int(Random.Range(0, 4), 1);
        currentPos = startPos;
        mansionMap[startPos.x, startPos.y].isStart = true;
        
        GenerateDoorConnections();
        GenerateStairs();

        DebugDisplayMap();
    }

    private void GenerateDoorConnections()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (x != 3) mansionMap[x, y].connectedRight = true;
                if (x != 0) mansionMap[x, y].connectedLeft = true;
            }
        }
    }

    private void GenerateStairs()
    {
        for (int y = 0; y < 2; y++)
        {
            int stairsIndex;
            while (true)
            {
                stairsIndex = Random.Range(0, 4);

                if (mansionMap[stairsIndex, y].isStart) continue;
                if (mansionMap[stairsIndex, y].connectedUp) continue;
                if (mansionMap[stairsIndex, y].connectedDown) continue;

                break;
            }

            mansionMap[stairsIndex, y].connectedUp = true;
            mansionMap[stairsIndex, y + 1].connectedDown = true;
        }
    }
}
