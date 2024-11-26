using UnityEngine;

public class Room
{
    [Header("Public Infos")] 
    public bool connectedLeft;
    public bool connectedRight;
    public bool connectedUp;
    public bool connectedDown;
    public bool isStart;
    public bool isLockedLeft;
    public bool isLockedRight;
    public Vector2Int coord;
    public Room previous;

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