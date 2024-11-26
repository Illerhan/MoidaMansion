using System.Collections.Generic;
using UnityEngine;

public class RoomDisplayManager : MonoBehaviour
{
    public RoomSo RoomSo { get; set; }
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    
    public void DisplayRoom()
    {
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            if (i < RoomSo.RoomSprites.Count)
            {
                spriteRenderers[i].sprite = RoomSo.RoomSprites[i];
                spriteRenderers[i].gameObject.SetActive(true);
            }
            else
            {
                spriteRenderers[i].gameObject.SetActive(false);
            }
        }
    }
}
