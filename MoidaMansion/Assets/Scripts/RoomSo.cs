using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSo", menuName = "Scriptable Objects/RoomSo")]
public class RoomSo : ScriptableObject
{
    [field: SerializeField] public string RoomName { get; private set; }
    [field: SerializeField] public RoomType RoomType { get; private set; }
    [field: SerializeField] public List<ObjectSo> RoomObjects { get; private set; } = new();
}
