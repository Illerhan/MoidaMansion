using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomDisplayManager : MonoBehaviour
{
    public RoomSo RoomSo;
    [SerializeField] private List<SpriteRenderer> spriteRenderers;

    private void Start()
    {
        DisplayRoom();
    }

    public void DisplayRoom()
    {
        int objectSoIndex = 0;
        int spriteIndex = 0;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (objectSoIndex < RoomSo.RoomObjects.Count)
            {
                spriteRenderer.sprite = RoomSo.RoomObjects[objectSoIndex].RoomSprites[spriteIndex];
                spriteRenderer.gameObject.SetActive(true);
                spriteIndex++;
                
                if (spriteIndex >= RoomSo.RoomObjects[objectSoIndex].RoomSprites.Count)
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
