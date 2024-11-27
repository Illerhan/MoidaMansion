using System.Collections.Generic;
using UnityEngine;

public class RoomDisplayManager : MonoBehaviour
{
    public Room Room { get; set; }
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    public List<List<SpriteRenderer>> packedSpriteRenderers { get; set; }

    [Header("RemovableItems")] 
    [SerializeField] private SpriteRenderer leftKeyLock;
    [SerializeField] private SpriteRenderer rightKeyLock;

    private void Start()
    {
        DisplayRoom();
    }

    public void DisplayRoom()
    {
        int objectSoIndex = 0;
        int spriteIndex = 0;
        packedSpriteRenderers = new List<List<SpriteRenderer>>();
        
        ActualiseRemovableItems();
        UIManager.Instance.DisplayText(Room.roomSo.name);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.gameObject.SetActive(false);
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (objectSoIndex < Room.roomSo.RoomObjects.Count)
            {
                // We pack the sprite renderers for la fouille
                if (objectSoIndex >= packedSpriteRenderers.Count)
                {
                    packedSpriteRenderers.Add(new List<SpriteRenderer>());
                }
                packedSpriteRenderers[packedSpriteRenderers.Count - 1].Add(spriteRenderer);
                
                
                spriteRenderer.sprite = Room.roomSo.RoomObjects[objectSoIndex].RoomSprites[spriteIndex];
                spriteRenderer.gameObject.SetActive(true);
                spriteIndex++;
                
                if (spriteIndex >= Room.roomSo.RoomObjects[objectSoIndex].RoomSprites.Count)
                {
                    spriteIndex = 0;
                    objectSoIndex++;
                }
            }
            else if (objectSoIndex == Room.roomSo.RoomObjects.Count)
            {
                spriteRenderer.sprite = Room.leftDoor.RoomSprites[spriteIndex];
                spriteRenderer.gameObject.SetActive(true);
                spriteIndex++;
                
                if (spriteIndex >= Room.leftDoor.RoomSprites.Count)
                {
                    spriteIndex = 0;
                    objectSoIndex++;
                }
            }
            else if (objectSoIndex == Room.roomSo.RoomObjects.Count + 1)
            {
                spriteRenderer.sprite = Room.rightDoor.RoomSprites[spriteIndex];
                spriteRenderer.gameObject.SetActive(true);
                spriteIndex++;
                
                if (spriteIndex >= Room.rightDoor.RoomSprites.Count)
                {
                    spriteIndex = 0;
                    objectSoIndex++;
                }
            }
            else if (objectSoIndex == Room.roomSo.RoomObjects.Count + 2)
            {
                if (Room.stairs == null) continue;
                
                spriteRenderer.sprite = Room.stairs.RoomSprites[spriteIndex];
                spriteRenderer.gameObject.SetActive(true);
                spriteIndex++;
                
                if (spriteIndex >= Room.stairs.RoomSprites.Count)
                {
                    spriteIndex = 0;
                    objectSoIndex++;
                }
            }
            else
            {
                spriteRenderer.gameObject.SetActive(false);
            }
        }
    }

    private void ActualiseRemovableItems()
    {
        leftKeyLock.enabled = Room.isLockedLeft;
        rightKeyLock.enabled = Room.isLockedRight;
    }
}
