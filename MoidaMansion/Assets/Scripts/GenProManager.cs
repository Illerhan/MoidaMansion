using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum LockType
{
    Key,
    Code,
    Secret
}

public enum ItemType
{
    Friend,
    Key,
    Code,
    SecretPassage,
    None
}

public enum HintType
{
    Ghost,
    Fairy,
    Position
}

[Serializable]
public struct ItemLocation
{
    public ItemLocation(ItemType itemType, Vector2Int location, int itemIndex)
    {
        this.itemType = itemType;
        roomCoord = location;
        this.itemIndex = itemIndex;
    }
    public ItemType itemType;
    public Vector2Int roomCoord;
    public int itemIndex;
}

public struct Hint
{
    public Hint(HintType hintType, ItemLocation location)
    {
        this.hintType = hintType;
        itemLocation = location;
    }
    
    public HintType hintType;
    public ItemLocation itemLocation;
}


public class GenProManager : MonoBehaviour
{
    public static GenProManager Instance;

    [Header("Parameters")] 
    [SerializeField] private bool debugPrint;
    
    [Header("Public Infos")]
    public ItemLocation[] friendPositions = new ItemLocation[3];
    public List<ItemLocation> keyItems = new List<ItemLocation>();
    public Vector2Int codeLockedLocation = new Vector2Int(-1, -1);
    
    [Header("Private Infos")]
    public Room[,] mansionMap = new Room[4, 3];
    private Vector2Int currentPos;
    private List<Vector2Int> bannedPositions;
    private List<int> bannedInts;
    private LockType[] lockTypes = new LockType[2];
    private Room[] lockedRooms = new Room[4];
    private Hint[] hints = new Hint[2];
    public List<int> foundKeyItemsIndexes = new List<int>();
    public List<int> foundFriendsIndexes = new List<int>();
    
    [Header("References")] 
    [SerializeField] private GraphicsGenProManager graphicsGenProManager;
    public FairiesManager fairiesManager;
    public GhostManager ghostManager;
    public RoomDisplayManager roomDisplayManager;
    [SerializeField] private GameObject roomPrefabDebug;
    [SerializeField] private GameObject pathPrefabDebug;
    [SerializeField] private GameObject keyPrefabDebug;
    [SerializeField] private GameObject codePrefabDebug;
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);

        codeLockedLocation = new Vector2Int(-1, -1);
        
        GenerateMansion();
        
        graphicsGenProManager.GenerateRoomTypes(mansionMap);
        graphicsGenProManager.GenerateRoomDoors(mansionMap);
        graphicsGenProManager.GenerateStairs(mansionMap);

        GenerateItems();
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
                if (mansionMap[x, y].hasFriend)
                {
                    room.GetComponent<SpriteRenderer>().color = Color.green;
                }

                if (mansionMap[x, y].hasKey)
                {
                    Instantiate(keyPrefabDebug, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
                }
                if (mansionMap[x, y].hasFullCode)
                {
                    Instantiate(codePrefabDebug, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
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
                        if (mansionMap[x - 1, y].isKeyLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        
                        if (mansionMap[x - 1, y].isCodeLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.magenta;
                        }
                        
                        if (mansionMap[x - 1, y].isSecretLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.blue;
                        }
                    }
                }
                if (mansionMap[x, y].connectedRight)
                {
                    GameObject path = Instantiate(pathPrefabDebug, new Vector3(x + 0.5f, y, 0), Quaternion.Euler(0, 0, 0));

                    if (mansionMap[x, y].isLockedRight)
                    {
                        if (mansionMap[x + 1, y].isKeyLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                    
                        if (mansionMap[x + 1, y].isCodeLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.magenta;
                        }
                    
                        if (mansionMap[x + 1, y].isSecretLocked)
                        {
                            path.GetComponent<SpriteRenderer>().color = Color.blue;
                        }
                    }
                }
            }
        }
    }


    #region Public Functions

    /// <summary>
    /// Gets the data of a room at the given coordinates
    /// </summary>
    public Room GetRoom(Vector2Int coordinates)
    {
        if (coordinates.x < 0 || coordinates.x > 3 || coordinates.y < 0 || coordinates.y > 2)
        {
            Debug.LogWarning("Coordinates Out of Range");
            return null;
        }
        
        return mansionMap[coordinates.x, coordinates.y];
    }

    /// <summary>
    /// Returns true if the room at the given coordinates is a dead end 
    /// </summary>
    public bool VerifyIsInDeadEnd(Vector2Int coordinates)
    {
        int connectionCount = 0;

        if (mansionMap[coordinates.x, coordinates.y].connectedLeft)
            connectionCount++;
        if (mansionMap[coordinates.x, coordinates.y].connectedRight)
            connectionCount++;
        if (mansionMap[coordinates.x, coordinates.y].connectedUp)
            connectionCount++;
        if (mansionMap[coordinates.x, coordinates.y].connectedDown)
            connectionCount++;

        return connectionCount == 1;
    }

    /// <summary>
    /// Changes the coordinates of the room where the player is
    /// </summary>
    public void ChangeCurrentRoom(Vector2Int newRoomCoordinates)
    {
        if (newRoomCoordinates.x < 0 || newRoomCoordinates.x > 3 || newRoomCoordinates.y < 0 ||
            newRoomCoordinates.y > 2)
        {
            Debug.LogWarning("New room coordinates out of range");
            return;
        }
        
        currentPos = newRoomCoordinates;
    }

    /// <summary>
    /// Returns the data of the room where the player is
    /// </summary>
    public Room GetCurrentRoom()
    {
        return mansionMap[currentPos.x, currentPos.y];
    }

    /// <summary>
    /// Returns the current hint to display
    /// </summary>
    /// <param name="friendIndex"> Either 0 or 1 </param>
    public Hint GetNextHint(int friendIndex)
    {
        return hints[friendIndex];
    }

    /// <summary>
    /// Verify at the end of the search if something was at the searched location
    /// </summary>
    public ItemType VerifySearch(Vector2Int roomPos, int itemIndex)
    {
        for (int i = 0; i < keyItems.Count; i++)
        {
            if (roomPos != keyItems[i].roomCoord) continue;
            if (itemIndex != keyItems[i].itemIndex) continue;
            if (foundKeyItemsIndexes.Contains(i)) continue;

            foundKeyItemsIndexes.Add(i);
            return keyItems[i].itemType;
        }
        for (int i = 0; i < friendPositions.Length; i++)
        {
            if (roomPos != friendPositions[i].roomCoord) continue;
            if (itemIndex != friendPositions[i].itemIndex) continue;
            
            return friendPositions[i].itemType;
        }
        

        return ItemType.None;
    }

    public ObjectSo GetCrossedRoom(bool left)
    {
        Room room = GetCurrentRoom();
        return left ? room.leftDoor : room.rightDoor;
    }

    #endregion
    
    
    #region Mansion Generation Functions
    
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
        int antiCrashCounter = 0;
        
        while (lockedPathCount < 2)
        {
            antiCrashCounter++;
            if (antiCrashCounter > 5000)
            {
                Debug.Log("AAARRGGG");
                break;
            }
            
            Vector2Int roomPosition = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            List<Room> pathToStart = GetPath(new Vector2Int(roomPosition.x, roomPosition.y), currentPos);
            
            if (pathToStart.Count < minDist)
            {
                if (lockedPathCount == 0) continue;
                if (pathToStart.Count != 0) continue;
                pathToStart = GetPath(lockedRooms[1].coord, currentPos);
                
                List<Room> path1 = GetPath(lockedRooms[1].coord, roomPosition);
                List<Room> path2 = GetPath(lockedRooms[0].coord, roomPosition);

                if (path1.Count < 2 && path2.Count < 2)
                {
                    continue;
                }

            }
            else
            {
                if (lockedPathCount != 0)
                {
                    List<Room> pathPreviousLock = GetPath(lockedRooms[1].coord, currentPos);

                    if (pathPreviousLock.Contains(mansionMap[roomPosition.x, roomPosition.y]))
                    {
                        continue;
                    }
                    
                    List<Room> path1 = GetPath(lockedRooms[1].coord, roomPosition);
                    List<Room> path2 = GetPath(lockedRooms[0].coord, roomPosition);

                    if (path1.Count < 3 && path2.Count < 3)
                    {
                        continue;
                    }
                }
            }

            if (pathToStart.Count != 0)
            {
                if (pathToStart[1].coord.x == roomPosition.x) continue;
            }

            lockedRooms[lockedPathCount * 2] = mansionMap[roomPosition.x, roomPosition.y];
                
            if (pathToStart[1].coord.x < roomPosition.x)
            {
                mansionMap[roomPosition.x, roomPosition.y].isLockedLeft = true;
                mansionMap[roomPosition.x - 1, roomPosition.y].isLockedRight = true;
                    
                lockedRooms[lockedPathCount * 2 + 1] = mansionMap[roomPosition.x - 1, roomPosition.y];
            }
            else
            {
                mansionMap[roomPosition.x, roomPosition.y].isLockedRight = true;
                mansionMap[roomPosition.x + 1, roomPosition.y].isLockedLeft = true;
                    
                lockedRooms[lockedPathCount * 2 + 1] = mansionMap[roomPosition.x + 1, roomPosition.y];
            }
            
            lockedPathCount++;
        }
    }

    private void GenerateItems()
    {
        GenerateFriends();
        ChooseLocks();
        GenerateKeys();
        
        if(debugPrint)
            DebugDisplayMap();
    }

    private void GenerateFriends()
    {
        int friendCount = 0;
        int antiCrashCounter = 0;

        while (friendCount != 3)
        {
            antiCrashCounter++;
            if (antiCrashCounter > 150)
            {
                Debug.Log("Not Enough friends");
                break;
            }
            
            Vector2Int location = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            bool locationValidated = true;

            if (mansionMap[location.x, location.y].isStart || mansionMap[location.x, location.y].hasFriend)
                continue;

            for (int i = 0; i < friendCount; i++)
            {
                List<Room> path = GetPath(location, friendPositions[i].roomCoord);

                if (path.Count != 0)
                {
                    locationValidated = false;
                    break;
                }
            }

            if (locationValidated)
            {
                AddKeyItem(location, ItemType.Friend, friendCount);
                //friendPositions[friendCount] = new ItemLocation(ItemType.Friend, location, 0);
                mansionMap[location.x, location.y].hasFriend = true;
                friendCount++;
            }
        }
    }
    
    private void ChooseLocks()
    {
        int bannedIndex = -1;
        
        for (int i = 0; i < 2; i++)
        {
            while (true)
            {
                int lockIndex = Random.Range(0, 3);

                if (lockIndex != bannedIndex)
                {
                    bannedIndex = lockIndex;
                    switch (lockIndex)
                    {
                        case 0 :
                            lockTypes[i] = LockType.Code;
                            lockedRooms[i * 2].isCodeLocked = true;
                            lockedRooms[i * 2 + 1].isCodeLocked = true;
                            codeLockedLocation = friendPositions[i + 1].roomCoord;
                            break;
                        case 1 :
                            lockTypes[i] = LockType.Key;
                            lockedRooms[i * 2].isKeyLocked = true;
                            lockedRooms[i * 2 + 1].isKeyLocked = true;
                            break;
                        case 2 :
                            lockTypes[i] = LockType.Secret;
                            lockedRooms[i * 2].isSecretLocked = true;
                            lockedRooms[i * 2 + 1].isSecretLocked = true;
                            break;
                    }

                    break;
                }
            }
        }
    }

    private void GenerateKeys()
    {
        Vector2Int previousKeyPos = new Vector2Int(-1, -1);
        
        for (int i = 0; i < 2; i++)
        {
            int antiCrashCounter = 0;
            
            while (true)
            {
                antiCrashCounter ++;
                if (antiCrashCounter > 150) break;
                
                Vector2Int roomPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));

                if (previousKeyPos != new Vector2Int(-1, -1))
                {
                    List<Room> pathToPrevious = GetPath(previousKeyPos, roomPos);
                    List<Room> pathToUnlockedLock = GetPath(lockedRooms[0].coord, roomPos);

                    if (pathToPrevious.Count != 0) continue;
                    if (pathToUnlockedLock.Count == 0) continue;

                    switch (lockTypes[i])
                    {
                        case LockType.Code :
                            mansionMap[roomPos.x, roomPos.y].hasFullCode = true;
                            AddKeyItem(roomPos, ItemType.Code);
                            break;
                            
                        case LockType.Key :
                            mansionMap[roomPos.x, roomPos.y].hasKey = true;
                            AddKeyItem(roomPos, ItemType.Key);
                            break;
                            
                        case LockType.Secret :
                            mansionMap[roomPos.x, roomPos.y].hasSecretPath = true;
                            AddKeyItem(lockedRooms[i * 2 + 1].coord, ItemType.SecretPassage);
                            break;
                    }
                        
                    break;
                }
                else
                {
                    List<Room> pathToStart = GetPath(currentPos, roomPos);
                    List<Room> pathToLockedRoom = GetPath(lockedRooms[1].coord, roomPos);
                    
                    if (pathToStart.Count <= 1) continue;
                    if (pathToLockedRoom.Count == 0) continue;
                    
                    previousKeyPos = roomPos;

                    switch (lockTypes[i])
                    {
                        case LockType.Code :
                            mansionMap[roomPos.x, roomPos.y].hasFullCode = true;
                            AddKeyItem(roomPos, ItemType.Code);
                            break;
                        
                        case LockType.Key :
                            mansionMap[roomPos.x, roomPos.y].hasKey = true;
                            AddKeyItem(roomPos, ItemType.Key);
                            break;
                        
                        case LockType.Secret :
                            mansionMap[roomPos.x, roomPos.y].hasSecretPath = true;
                            AddKeyItem(lockedRooms[i * 2 + 1].coord, ItemType.SecretPassage);
                            break;
                    }
                    
                    break;
                    
                }
            }
        }
    }
    
    # endregion
    
    
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


    #region Others

    private void AddKeyItem(Vector2Int itemPos, ItemType itemType, int friendIndex = 0)
    {
        RoomSo room = mansionMap[itemPos.x, itemPos.y].roomSo;

        // Friend Spawn
        if (itemType == ItemType.Friend)
        {
            for (int i = 0; i < room.RoomObjects.Count; i++)
            {
                if (!room.RoomObjects[i].CanHaveFriend) continue;

                friendPositions[friendIndex] = new ItemLocation(itemType, itemPos, i);
                return;
            }

            int antiCrashCounter = 0;
            while (true)
            {
                antiCrashCounter++;
                if (antiCrashCounter > 2000)
                {
                    Debug.LogError("Je trouve pas d'objets où spawn des amis");
                    return;
                };
                
                room = GraphicsGenProManager.Instance.possibleRooms[Random.Range(0, GraphicsGenProManager.Instance.possibleRooms.Count)];
                bool found = false;
                
                for (int i = 0; i < room.RoomObjects.Count; i++)
                {
                    if (!room.RoomObjects[i].CanHaveFriend) continue;
                    found = true;
                    
                    mansionMap[itemPos.x, itemPos.y].roomSo = room;
                    break;
                }

                if (found) break;
            }
            
            for (int i = 0; i < room.RoomObjects.Count; i++)
            {
                if (!room.RoomObjects[i].CanHaveFriend) continue;
                
                friendPositions[friendIndex] = new ItemLocation(itemType, itemPos, i);
                return;
            }
        }
        
        
        // Objets plus classiques
        int debugCounter = 0;
        while (true)
        {
            debugCounter++;
            if (debugCounter > 100)
                break;
            
            int pickedIndex = Random.Range(0, room.RoomObjects.Count);
            
            if (!room.RoomObjects[pickedIndex].CanBeSearched) continue;
            if (room.RoomObjects[pickedIndex].CanHaveFriend) continue;
            
            keyItems.Add(new ItemLocation(itemType, itemPos, pickedIndex));
            hints[keyItems.Count - 1] = new Hint(HintType.Position, keyItems[keyItems.Count - 1]);

            return;
        }

        int antiCrashCounter2 = 0;
        while (true)
        {
            antiCrashCounter2++;
            if (antiCrashCounter2 > 2000)
            {
                Debug.LogError("Je trouve pas d'objets où spawn des objects");
                return;
            };
                
            room = GraphicsGenProManager.Instance.possibleRooms[Random.Range(0, GraphicsGenProManager.Instance.possibleRooms.Count)];
            bool found = false;
                
            for (int i = 0; i < room.RoomObjects.Count; i++)
            {
                if (!room.RoomObjects[i].CanBeSearched) continue;
                if (room.RoomObjects[i].CanHaveFriend) continue;
                if (room.RoomType == RoomType.Entrance) continue;
                found = true;
                
                mansionMap[itemPos.x, itemPos.y].roomSo = room;
                break;
            }

            if (found) break;
        }
            

        while (true)
        {
            int pickedIndex = Random.Range(0, room.RoomObjects.Count);
            
            if (!room.RoomObjects[pickedIndex].CanBeSearched) continue;
            if (room.RoomObjects[pickedIndex].CanHaveFriend) continue;
            
            keyItems.Add(new ItemLocation(itemType, itemPos, pickedIndex));
            hints[keyItems.Count - 1] = new Hint(HintType.Position, keyItems[keyItems.Count - 1]);

            break;
        }
    }

    #endregion
}
