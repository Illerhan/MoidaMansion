using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FairiesManager : MonoBehaviour
{
    [Header("Public Infos")] 
    public Vector2Int[] fairiesPositions;
    public bool[] pickedFairies;
    public bool hasFairy;
    public List<Vector2Int> bannedPositions = new List<Vector2Int>();

    [Header("Private infos")] 
    private bool isSetup;
    
    [Header("References")] 
    [SerializeField] private SpriteRenderer[] fairies;


    private void Start()
    {
        hasFairy = false;
        for (int i = 0; i < fairies.Length; i++)
        {
            fairies[i].enabled = false;
        }
    }

    /*private bool doOnce = false;
    private void Update()
    {
        if (!doOnce)
        {
            doOnce = true;
            SetupFairies(GenProManager.Instance.GetCurrentRoom().coord);
        }
    }*/


    public void SetupFairies(Vector2Int playerPos)
    {
        isSetup = true;
        
        int currentFiaryCounter = 0;
        fairiesPositions = new Vector2Int[3];
        pickedFairies = new bool[3];

        while (currentFiaryCounter < 3)
        {
            Vector2Int pickedPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            List<Room> path = GenProManager.Instance.GetPath(pickedPos, playerPos);

            if (bannedPositions.Contains(pickedPos)) continue;
            if (path.Count == 0 || path.Count == 1) continue;
            if (GenProManager.Instance.mansionMap[pickedPos.x, pickedPos.y].roomSo.RoomType == RoomType.Void) continue;
            
            bannedPositions.Add(pickedPos);
            fairiesPositions[currentFiaryCounter] = pickedPos;
            pickedFairies[currentFiaryCounter] = false;
            currentFiaryCounter++;
        }
    }

    public SpriteRenderer GetCurrentFairy()
    {
        if (!isSetup) return null; 
        
        for (int i = 0; i < 3; i++)
        {
            if (!fairies[i].enabled) continue;

            return fairies[i];
        }

        return null;
    }

    public void DisplayFairy(Vector2Int currentPos)
    {
        if (!isSetup) return;
        
        hasFairy = false;
        
        for (int i = 0; i < 3; i++)
        {
            fairies[i].enabled = false;
            
            if (currentPos != fairiesPositions[i]) continue;
            if (pickedFairies[i]) continue;

            fairies[i].enabled = true;
            hasFairy = true;
        }
    }

    public void PickFairy()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!fairies[i].enabled) continue;

            pickedFairies[i] = true;
            fairies[i].enabled = false;
        }
    }
}
