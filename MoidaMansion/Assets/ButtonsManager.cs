using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ButtonsManager : MonoBehaviour
{
    [Header("Public infos")] 
    public bool hasButtonDisplayed;
    
    [Header("Private infos")]
    private bool[] activatedButtons;
    private Vector2Int[] buttonPositions;
    private List<Vector2Int> bannedPositions = new List<Vector2Int>();
    
    [Header("References")]
    [SerializeField] private SpriteRenderer[] buttonSpriteRenderers;


    private void Start()
    {
        SetupButtons();
    }

    public void SetupButtons()
    {
        buttonPositions = new Vector2Int[4];
        activatedButtons = new bool[4];
        
        for (int i = 0; i < 4; i++)
        {
            activatedButtons[i] = false;

            while (true)
            {
                Vector2Int roomPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 3));

                if (bannedPositions.Contains(roomPos)) continue;
                
                bannedPositions.Add(roomPos);
                buttonPositions[i] = roomPos;

                break;
            }
        }
    }

    public void DisplayButton(Vector2Int roomPos)
    {
        hasButtonDisplayed = false;
        
        for (int i = 0; i < buttonPositions.Length; i++)
        {
            buttonSpriteRenderers[i].enabled = false;
            
            if (buttonPositions[i] != roomPos) continue;

            hasButtonDisplayed = true;
            buttonSpriteRenderers[i].enabled = true;
        }
    }

    public SpriteRenderer GetRoomButton(Vector2Int roomPos)
    {
        for (int i = 0; i < buttonPositions.Length; i++)
        {
            if (buttonPositions[i] != roomPos) continue;

            return buttonSpriteRenderers[i];
        }

        return null;
    }

    public void ActivateButton(Vector2Int roomPos)
    {
        for (int i = 0; i < buttonPositions.Length; i++)
        {
            if (buttonPositions[i] != roomPos) continue;

            activatedButtons[i] = true;
        }
    }
}
