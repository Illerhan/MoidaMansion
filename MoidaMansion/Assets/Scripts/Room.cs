using UnityEngine;

public class Room
{
    [Header("Room connections")] 
    public bool connectedLeft;
    public bool connectedRight;
    public bool connectedUp;
    public bool connectedDown;
    public bool isLockedLeft;
    public bool isLockedRight;
    public bool isKeyLocked;
    public bool isCodeLocked;
    public bool isSecretLocked;

    [Header("Room Contained Elements")] 
    public bool hasKey;
    public bool hasFriend;
    public bool hasFullCode;
    public bool hasCodePart;
    
    [Header("Others")]
    public bool isStart;
    public Vector2Int coord;    // Needed for pathfinding
    public Room previous;    // Needed for pathfinding

}

public enum RoomType
{
    None,
    Entrance,
    Laboratory,
    Bedroom,
    Void,
    Study,
    Storage,
    Kitchen,
    Dressing,
    Toilet,
    Boiler,
    Library
}