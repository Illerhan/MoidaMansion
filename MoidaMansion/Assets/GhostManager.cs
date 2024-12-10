using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostManager : MonoBehaviour
{
    [Header("Public Infos")] 
    public Vector2Int wantedPos;
    public Vector2Int startPos;
    public List<Room> ghostPath;
    public List<Vector2Int> validatedPos = new List<Vector2Int>();
    public bool ghostActive;

    [Header("Private Infos")]
    private Coroutine currentCoroutine;
    private int objIndex;

    [Header("References")] 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpriteRenderer[] leftUpPath;
    [SerializeField] private SpriteRenderer[] rightUpPath;
    [SerializeField] private SpriteRenderer[] leftRightPath;
    [SerializeField] private SpriteRenderer[] startLeftPath;
    [SerializeField] private SpriteRenderer[] startRightPath;
    [SerializeField] private SpriteRenderer[] startUpPath;
    [SerializeField] private SpriteRenderer[] everySteps;
    
    
    /*private bool doOnce = false;
    private void Update()
    {
        if (!doOnce)
        {
            doOnce = true;
            SetupGhost(GenProManager.Instance.GetCurrentRoom().coord, GenProManager.Instance.keyItems[0].roomCoord, GenProManager.Instance.keyItems[0].itemIndex);
        }
    }*/
    
    
    public void SetupGhost(Vector2Int playerPos, Vector2Int keyPos, int objIndex)
    {
        ghostActive = true;
        this.objIndex = objIndex;

        int antiCrashCounter = 0;
        
        while (true)
        {
            antiCrashCounter++;
            if (antiCrashCounter > 1000)
            {
                Debug.LogError("Can't Spawn ghost");
                break;
            }
            
            Vector2Int pickedPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            List<Room> path = GenProManager.Instance.GetPath(pickedPos, playerPos);

            if (path.Count <= 1) continue;
            
            ghostPath = GenProManager.Instance.GetPath(pickedPos, keyPos);
            
            if (ghostPath.Count <= 1) continue;
            
            startPos = pickedPos;
            break;
        }

        validatedPos.Add(startPos);
        wantedPos = keyPos;
    }

    
    public void VerifyGhost(Vector2Int currentRoomCoord)
    {
        if (!ghostActive) return;
        
        for (int i = 0; i < everySteps.Length; i++)
        {
            everySteps[i].enabled = false;
            if(currentCoroutine != null)
                StopCoroutine(currentCoroutine);
        }
        
        if (!validatedPos.Contains(currentRoomCoord)) return;

        Room comingFromRoom = null;
        Room currentRoom = GenProManager.Instance.mansionMap[currentRoomCoord.x, currentRoomCoord.y];
        Room nextRoom = null;

        for (int i = 0; i < ghostPath.Count; i++)
        {
            if (ghostPath[i] == currentRoom)
            {
                if (i != 0)
                {
                    comingFromRoom = ghostPath[i - 1];
                }
                if (i != ghostPath.Count - 1)
                {
                    nextRoom = ghostPath[i + 1];
                }
            }
        }
        
        if (comingFromRoom == null)
        {
            Vector2Int dirExit = nextRoom.coord - currentRoom.coord;
            dirExit.y = Mathf.Abs(dirExit.y);
            
            if (dirExit == new Vector2Int(1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startRightPath, false));
            }
            else if (dirExit == new Vector2Int(-1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startLeftPath, false));
            }
            else if (dirExit == new Vector2Int(0, 1))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startUpPath, false));
            }
            
            validatedPos.Add(nextRoom.coord);
        } 
        else if (nextRoom == null)
        {
            ghostActive = false;
            playerController.StopControl();
            
            Vector2Int dirEnter = currentRoom.coord - comingFromRoom.coord;
            dirEnter.y = Mathf.Abs(dirEnter.y);
            
            if (dirEnter == new Vector2Int(1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startLeftPath, true));
            }
            else if (dirEnter == new Vector2Int(-1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startRightPath, true));
            }
            else if (dirEnter == new Vector2Int(0, 1))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(startUpPath, true));
            }
        }
        else
        {
            Vector2Int dirEnter = currentRoom.coord - comingFromRoom.coord;
            Vector2Int dirExit = nextRoom.coord - currentRoom.coord;

            dirEnter.y = Mathf.Abs(dirEnter.y);
            dirExit.y = Mathf.Abs(dirExit.y);

            if (dirEnter == new Vector2Int(1, 0) && dirExit == new Vector2Int(1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(leftRightPath, false));
            }
            else if (dirEnter == new Vector2Int(-1, 0) && dirExit == new Vector2Int(-1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(leftRightPath, true));
            }
            else if (dirEnter == new Vector2Int(1, 0) && dirExit == new Vector2Int(0, 1))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(leftUpPath, false));
            }
            else if (dirEnter == new Vector2Int(0, 1) && dirExit == new Vector2Int(-1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(leftUpPath, true));
            }
            else if (dirEnter == new Vector2Int(-1, 0) && dirExit == new Vector2Int(0, 1))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(rightUpPath, false));
            }
            else if (dirEnter == new Vector2Int(0, 1) && dirExit == new Vector2Int(1, 0))
            {
                currentCoroutine = StartCoroutine(PlayGhostPath(rightUpPath, true));
            }
            
            validatedPos.Add(nextRoom.coord);
        }

    }

    private IEnumerator PlayGhostPath(SpriteRenderer[] path, bool reverse)
    {
        if (reverse)
        {
            for (int i = path.Length - 1; i >= 0; i--)
            {
                path[i].enabled = true;
            
                yield return new WaitForSeconds(0.5f);
            
                path[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < path.Length; i++)
            {
                path[i].enabled = true;
            
                yield return new WaitForSeconds(0.5f);
            
                path[i].enabled = false;
            }
        }

        if (!ghostActive)
        {
            StartCoroutine(EndGhostCoroutine());
        }
    }

    private IEnumerator EndGhostCoroutine()
    {
        Room room = GenProManager.Instance.GetCurrentRoom();
        
        for (int i = 0; i < 10; i++)
        {
            int pickedIndex = Random.Range(0, GenProManager.Instance.roomDisplayManager.packedSpriteRenderers.Count);

            for (int j = 0; j < GenProManager.Instance.roomDisplayManager.packedSpriteRenderers[pickedIndex].Count; j++)
            {
                GenProManager.Instance.roomDisplayManager.packedSpriteRenderers[pickedIndex][j].enabled = false;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            for (int j = 0; j < GenProManager.Instance.roomDisplayManager.packedSpriteRenderers[pickedIndex].Count; j++)
            {
                GenProManager.Instance.roomDisplayManager.packedSpriteRenderers[pickedIndex][j].enabled = true;
            } 
        }

        yield return new WaitForSeconds(0.1f);
        List<SpriteRenderer> sprites = GenProManager.Instance.roomDisplayManager.packedSpriteRenderers[objIndex];
        
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < sprites.Count; j++)
            {
                sprites[j].enabled = false;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            for (int j = 0; j <sprites.Count; j++)
            {
                sprites[j].enabled = true;
            } 
            
            yield return new WaitForSeconds(0.1f);
        }
        
        playerController.GiveControl();
    }
}
