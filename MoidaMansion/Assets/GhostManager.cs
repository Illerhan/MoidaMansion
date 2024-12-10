using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [Header("Public Infos")] 
    public Vector2Int wantedPos;
    public Vector2Int startPos;
    public List<Room> ghostPath;
    public List<Vector2Int> validatedPos = new List<Vector2Int>();

    [Header("References")] 
    [SerializeField] private SpriteRenderer[] leftUpPath;
    [SerializeField] private SpriteRenderer[] rightUpPath;
    [SerializeField] private SpriteRenderer[] leftRightPath;
    [SerializeField] private SpriteRenderer[] startLeftPath;
    [SerializeField] private SpriteRenderer[] startRightPath;
    [SerializeField] private SpriteRenderer[] startUpPath;
    
    
    public void SetupGhost(Vector2Int playerPos, Vector2Int keyPos)
    {
        while (true)
        {
            Vector2Int pickedPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            List<Room> path = GenProManager.Instance.GetPath(pickedPos, playerPos);

            if (path.Count == 0 || path.Count == 1) continue;
            
            ghostPath = path;
            startPos = pickedPos;
            break;
        }

        validatedPos.Add(startPos);
        wantedPos = keyPos;
    }

    public void VerifyGhost(Vector2Int currentRoomCoord)
    {
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
                else if (i != ghostPath.Count - 1)
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
                StartCoroutine(PlayGhostPath(startRightPath, false));
            }
            else if (dirExit == new Vector2Int(-1, 0))
            {
                StartCoroutine(PlayGhostPath(startLeftPath, false));
            }
            else if (dirExit == new Vector2Int(0, 1))
            {
                StartCoroutine(PlayGhostPath(startUpPath, false));
            }
            
            validatedPos.Add(nextRoom.coord);
        } 
        else if (nextRoom == null)
        {
            Vector2Int dirEnter = currentRoom.coord - comingFromRoom.coord;
            dirEnter.y = Mathf.Abs(dirEnter.y);
            
            if (dirEnter == new Vector2Int(1, 0))
            {
                StartCoroutine(PlayGhostPath(startLeftPath, true));
            }
            else if (dirEnter == new Vector2Int(-1, 0))
            {
                StartCoroutine(PlayGhostPath(startRightPath, true));
            }
            else if (dirEnter == new Vector2Int(0, 1))
            {
                StartCoroutine(PlayGhostPath(startUpPath, true));
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
                StartCoroutine(PlayGhostPath(leftRightPath, false));
            }
            else if (dirEnter == new Vector2Int(-1, 0) && dirExit == new Vector2Int(-1, 0))
            {
                StartCoroutine(PlayGhostPath(leftRightPath, true));
            }
            else if (dirEnter == new Vector2Int(1, 0) && dirExit == new Vector2Int(0, 1))
            {
                StartCoroutine(PlayGhostPath(leftUpPath, false));
            }
            else if (dirEnter == new Vector2Int(0, 1) && dirExit == new Vector2Int(-1, 0))
            {
                StartCoroutine(PlayGhostPath(leftUpPath, true));
            }
            else if (dirEnter == new Vector2Int(-1, 0) && dirExit == new Vector2Int(0, 1))
            {
                StartCoroutine(PlayGhostPath(rightUpPath, false));
            }
            else if (dirEnter == new Vector2Int(0, 1) && dirExit == new Vector2Int(1, 0))
            {
                StartCoroutine(PlayGhostPath(rightUpPath, true));
            }
            
            validatedPos.Add(nextRoom.coord);
        }

    }

    private IEnumerator PlayGhostPath(SpriteRenderer[] path, bool reverse)
    {
        yield return new WaitForSeconds(1);
    }
}
