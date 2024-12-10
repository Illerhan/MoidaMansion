using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [Header("Public Infos")] 
    public Vector2Int wantedPos;
    public Vector2Int startPos;
    public List<Room> ghostPath;

    [Header("References")] 
    [SerializeField] private SpriteRenderer[] leftUpPath;
    [SerializeField] private SpriteRenderer[] rightUpPath;
    [SerializeField] private SpriteRenderer[] leftRightPath;
    
    
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

        wantedPos = keyPos;
    }

    public void VerifyGhost()
    {
        
    }
}
