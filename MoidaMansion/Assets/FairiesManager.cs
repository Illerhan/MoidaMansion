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

    [Header("Private infos")] 
    private bool isSetup;
    
    [Header("References")] 
    [SerializeField] private SpriteRenderer[] fairies;


    private void Start()
    {
        for (int i = 0; i < fairies.Length; i++)
        {
            fairies[i].enabled = false;
        }
    }


    public void SetupFairies(Vector2Int playerPos, Room[,] map)
    {
        isSetup = true;
        
        int currentFiaryCounter = 0;
        fairiesPositions = new Vector2Int[3];
        pickedFairies = new bool[3];

        while (currentFiaryCounter < 3)
        {
            Vector2Int pickedPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));
            List<Room> path = GenProManager.Instance.GetPath(pickedPos, playerPos);

            if (path.Count == 0 || path.Count == 1) continue;

            fairiesPositions[currentFiaryCounter] = pickedPos;
            pickedFairies[currentFiaryCounter] = false;
            currentFiaryCounter++;
        }
    }

    public SpriteRenderer GetCurrentFairy()
    {
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

            fairies[i].enabled = false;
        }
    }
}
