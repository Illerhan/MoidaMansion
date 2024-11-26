using System;
using System.Collections.Generic;
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
                    GameObject path = Instantiate(pathPrefabDebug, new Vector3(x - 0.5f, y, 0), Quaternion.Euler(0, 0, 0));
                    
                    if (mansionMap[x, y].isLockedLeft)
                    {
                        path.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                }
                if (mansionMap[x, y].connectedRight)
                {
                    GameObject path = Instantiate(pathPrefabDebug, new Vector3(x + 0.5f, y, 0), Quaternion.Euler(0, 0, 0));
                    
                    if (mansionMap[x, y].isLockedRight)
                    {
                        path.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
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
                mansionMap[x, y].coord = new Vector2Int(x, y);
            }
        }
        
        
        Vector2Int startPos = new Vector2Int(Random.Range(0, 4), 1);
        currentPos = startPos;
        mansionMap[startPos.x, startPos.y].isStart = true;
        
        GenerateDoorConnections();
        GenerateStairs();

        GenerateLockedDoors(4);
        GenerateFriends();
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
                if (mansionMap[stairsIndex, y + 1].isStart) continue;

                break;
            }

            mansionMap[stairsIndex, y].connectedUp = true;
            mansionMap[stairsIndex, y + 1].connectedDown = true;
        }
    }

    private void GenerateLockedDoors(int minDist)
    {
        int lockedPathCount = 0;
        int delay = 0;
        
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (lockedPathCount >= 2) return;

                if (delay >= 0)
                {
                    delay--;
                    continue;
                }
                
                List<Room> pathToStart = GetPath(new Vector2Int(x, y), currentPos);

                if (pathToStart.Count < minDist) continue;
                if (pathToStart[1].coord.x == x) continue;

                lockedPathCount++;
                delay = 4;

                if (pathToStart[1].coord.x < x)
                {
                    mansionMap[x, y].isLockedLeft = true;
                    mansionMap[x - 1, y].isLockedRight = true;
                }
                else
                {
                    mansionMap[x, y].isLockedRight = true;
                    mansionMap[x + 1, y].isLockedLeft = true;
                }
            }
        }
    }


    private void GenerateFriends()
    {
        int friendCount = 0;
        int delay = 0;
        
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (delay >= 0)
                {
                    delay--;
                    continue;
                }
                
                List<Room> pathToStart = GetPath(new Vector2Int(x, y), currentPos);
            }
        }
    }
    
    
    #region Pathfinding

    public List<Room> GetPath(Vector2Int start, Vector2Int end)
    {
        List<Room> openList = new List<Room>();
        List<Room> closedList = new List<Room>();

        openList.Add(mansionMap[start.x, start.y]);
        
        while (openList.Count != 0)
        {
            Room currentRoom = openList[0];
            
            if (currentRoom.coord == end)
                return GetFinalPath(start, currentRoom);

            if (currentRoom.coord.x > 0 && currentRoom.connectedLeft && !currentRoom.isLockedLeft)
            {
                if (!openList.Contains(mansionMap[currentRoom.coord.x - 1, currentRoom.coord.y]) &&
                    !closedList.Contains(mansionMap[currentRoom.coord.x - 1, currentRoom.coord.y]))
                {
                    openList.Add(mansionMap[currentRoom.coord.x - 1, currentRoom.coord.y]);
                    mansionMap[currentRoom.coord.x - 1, currentRoom.coord.y].previous = currentRoom;
                }
            }
            if (currentRoom.coord.x < 3 && currentRoom.connectedRight && !currentRoom.isLockedRight)
            {
                if (!openList.Contains(mansionMap[currentRoom.coord.x + 1, currentRoom.coord.y]) &&
                    !closedList.Contains(mansionMap[currentRoom.coord.x + 1, currentRoom.coord.y]))
                {
                    openList.Add(mansionMap[currentRoom.coord.x + 1, currentRoom.coord.y]);
                    mansionMap[currentRoom.coord.x + 1, currentRoom.coord.y].previous = currentRoom;
                }
            }
            if (currentRoom.coord.y > 0 && currentRoom.connectedDown)
            {
                if (!openList.Contains(mansionMap[currentRoom.coord.x, currentRoom.coord.y - 1]) &&
                    !closedList.Contains(mansionMap[currentRoom.coord.x, currentRoom.coord.y - 1]))
                {
                    openList.Add(mansionMap[currentRoom.coord.x, currentRoom.coord.y - 1]);
                    mansionMap[currentRoom.coord.x, currentRoom.coord.y - 1].previous = currentRoom;
                }
            }
            if (currentRoom.coord.y < 2 && currentRoom.connectedUp)
            {
                if (!openList.Contains(mansionMap[currentRoom.coord.x, currentRoom.coord.y + 1]) &&
                    !closedList.Contains(mansionMap[currentRoom.coord.x, currentRoom.coord.y + 1]))
                {
                    openList.Add(mansionMap[currentRoom.coord.x, currentRoom.coord.y + 1]);
                    mansionMap[currentRoom.coord.x, currentRoom.coord.y + 1].previous = currentRoom;
                }
            }
            
            openList.RemoveAt(0);
            closedList.Add(currentRoom);
        }

        return new List<Room>();
    }

    private List<Room> GetFinalPath(Vector2Int start, Room endRoom)
    {
        List<Room> path = new List<Room>();
        Room currentRoom = endRoom;

        while (true)
        {
            path.Add(currentRoom);

            if (currentRoom.coord == start)
                break;

            currentRoom = currentRoom.previous;
        }
        
        path.Reverse();
        return path;
    }
    
    #endregion
}
