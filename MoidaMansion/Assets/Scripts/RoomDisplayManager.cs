using System.Collections.Generic;
using UnityEngine;

public class RoomDisplayManager : MonoBehaviour
{
    public Room Room { get; set; }
    [SerializeField] private List<SpriteRenderer> spriteRenderers;

    private void Start()
    {
        DisplayRoom();
    }

    public void DisplayRoom()
    {
        int objectSoIndex = 0;
        int spriteIndex = 0;
        
        Debug.Log(Room.leftDoor.name);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (objectSoIndex < Room.roomSo.RoomObjects.Count)
            {
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
            else
            {
                spriteRenderer.gameObject.SetActive(false);
            }
        }
    }
}
