using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorListSo", menuName = "Scriptable Objects/DoorListSo")]
public class DoorListSo : ScriptableObject
{
    [field: SerializeField] public List<ObjectSo> LeftDoors { get; private set; } = new();
    [field: SerializeField] public List<ObjectSo> RightDoors { get; private set; } = new();
}
