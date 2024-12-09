using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDisplayManager : MonoBehaviour
{
    public Room Room { get; set; }
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    public List<List<SpriteRenderer>> packedSpriteRenderers { get; set; }

    [HideInInspector] public SpriteRenderer displayedCode;

    private Coroutine noiseCoroutine;
    
    [Header("RemovableItems")] 
    [SerializeField] private SpriteRenderer leftKeyLock;
    [SerializeField] private SpriteRenderer rightKeyLock;
    [SerializeField] private SpriteRenderer code1;
    [SerializeField] private SpriteRenderer code2;
    [SerializeField] private SpriteRenderer code3;
    [SerializeField] private SpriteRenderer noise1;
    [SerializeField] private SpriteRenderer noise2;

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
                if (Room.isLockedLeft && Room.isSecretLocked)
                {
                    objectSoIndex++;
                    continue;
                }
                
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
                if (Room.isLockedRight && Room.isSecretLocked)
                {
                    objectSoIndex++;
                    continue;
                }
                
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
        leftKeyLock.enabled = false;
        rightKeyLock.enabled = false;
        code1.enabled = false;
        code2.enabled = false;
        code3.enabled = false;
        displayedCode = null;
        noise1.enabled = false;
        noise2.enabled = false;

        if (noiseCoroutine != null)
        {
            StopCoroutine(noiseCoroutine);
        }
        
        if (Room.isKeyLocked)
        {
            leftKeyLock.enabled = Room.isLockedLeft;
            rightKeyLock.enabled = Room.isLockedRight;
        }
        else if (Room.hasSecretPath)
        {
            
        }

        if (Room.coord == GenProManager.Instance.codeLockedLocation)
        {
            ObjectSo lockedObject = null;
            
            for (int i = 0; i < GenProManager.Instance.friendPositions.Length; i++)
            {
                if(GenProManager.Instance.friendPositions[i].roomCoord == Room.coord)
                {
                    lockedObject = Room.roomSo.RoomObjects[GenProManager.Instance.friendPositions[i].itemIndex];
                }
            }

            if (lockedObject.name == "LeftCloset1")
            {
                code3.enabled = true;
                displayedCode = code3;
            } 
            else if (lockedObject.name == "ToiletFurniture1")
            {
                code3.enabled = true;
                displayedCode = code3;
            }
            else if (lockedObject.name == "RightFurniture1")
            {
                code1.enabled = true;
                displayedCode = code1;
            } 
            else if (lockedObject.name == "LeftMirrorHandleFurniture1")
            {
                code2.enabled = true;
                displayedCode = code2;
            }
        } 
        
        // Noise
        for (int i = 0; i < GenProManager.Instance.friendPositions.Length; i++)
        {
            if(GenProManager.Instance.friendPositions[i].roomCoord == Room.coord)
            {
                if (GenProManager.Instance.foundFriendsIndexes.Contains(i)) continue;
                
                ObjectSo lockedObject = Room.roomSo.RoomObjects[GenProManager.Instance.friendPositions[i].itemIndex];
                
                if (lockedObject.name == "LeftCloset1")
                {
                    noiseCoroutine = StartCoroutine(PlayNoiseLeft());
                } 
                else if (lockedObject.name == "ToiletFurniture1")
                {
                    noiseCoroutine = StartCoroutine(PlayNoiseLeft());
                }
                else if (lockedObject.name == "RightFurniture1")
                {
                    noiseCoroutine = StartCoroutine(PlayNoiseRight());
                }
                else if (lockedObject.name == "LeftMirrorHandleFurniture1")
                {
                    noiseCoroutine = StartCoroutine(PlayNoiseLeft());
                }
            }
        }
        
    }

    private IEnumerator PlayNoiseLeft()
    {    
        while (true)
        {
            noise1.enabled = false;
            
            yield return new WaitForSeconds(Random.Range(0.4f, 2f));
            
            noise1.enabled = true;
            
            yield return new WaitForSeconds(0.1f);
            
            noise1.enabled = false;
        }
    }

    private IEnumerator PlayNoiseRight()
    {
        while (true)
        {
            noise2.enabled = false;
        
            yield return new WaitForSeconds(Random.Range(0.4f, 2f));
        
            noise2.enabled = true;
        
            yield return new WaitForSeconds(0.1f);
        
            noise2.enabled = false;
        }
    }
}
