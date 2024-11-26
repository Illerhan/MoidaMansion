using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSo", menuName = "Scriptable Objects/RoomSo")]
public class RoomSo : ScriptableObject
{
    [field: SerializeField] public RoomType RoomType { get; private set; }
    [field: SerializeField] public List<Sprite> RoomSprites { get; private set; } = new();
}
