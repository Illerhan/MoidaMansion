using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectSo", menuName = "Scriptable Objects/ObjectSo")]
public class ObjectSo : ScriptableObject
{
    [field: SerializeField] public bool CanBeSearched { get; private set; }
    [field: SerializeField] public bool CanHaveFriend { get; private set; }
    [field: SerializeField] public List<Sprite> RoomSprites { get; private set; } = new();
}
