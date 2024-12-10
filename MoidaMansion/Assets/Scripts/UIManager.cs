using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")] 
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RoomDisplayManager roomDisplayManager;
    [SerializeField] private GhostManager ghostManager;
    [SerializeField] private FairiesManager fairiesManager;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    public void DisplayText(string text)
    {
        if (text.Length > 13)
        {
            StartCoroutine(DisplayLargeTextCoroutine(text));
            return;
        }
        
        mainText.enabled = true;
        mainText.text = text;
    }

    public void HideText()
    {
        mainText.enabled = false;
    }
    
    public void DisplayRescueText(string roomName, string friendName)
    {
        StartCoroutine(DisplayRescueTextCoroutine(roomName, friendName, GenProManager.Instance.GetNextHint()));
    }
    
    
    
    private IEnumerator DisplayRescueTextCoroutine(string roomName, string friendName, Hint hint)
    {
        playerController.StopControl();
        
        DisplayText("Rescued " + friendName + "!");
        yield return new WaitForSeconds(2f);
        DisplayText("\"thank you!\"");
        yield return new WaitForSeconds(2f);

        switch (hint.hintType)
        {
            case HintType.Ghost :
                StartCoroutine(DisplayGhostHintCoroutine(roomName, hint));
                break;
            
            case HintType.Fairy :
                StartCoroutine(DisplayFairiesHintCoroutine(roomName));
                break;
            
            case HintType.Position :
                StartCoroutine(DisplayPositionHintCoroutine(hint, roomName));
                break;
        }
    }
    
    private IEnumerator DisplayLargeTextCoroutine(string text)
    {
        for (int i = 0; i < text.Length - 12; i++)
        {
            DisplayText(text[new Range(i, i+13)]);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DisplayGhostHintCoroutine(string roomName, Hint hint)
    {
        DisplayText("\"Follow ghost for hint\"");

        ghostManager.SetupGhost(GenProManager.Instance.GetCurrentRoom().coord, hint.itemLocation.roomCoord, hint.itemLocation.itemIndex);
        
        yield return new WaitForSeconds(5f);
        
        playerController.GiveControl();
        DisplayText(roomName);
    }

    private IEnumerator DisplayFairiesHintCoroutine(string roomName)
    {
        DisplayText("\"Search fairies for hint\"");
        
        fairiesManager.SetupFairies(GenProManager.Instance.GetCurrentRoom().coord);
        
        yield return new WaitForSeconds(5f);
        
        playerController.GiveControl();
        DisplayText(roomName);
    }

    private IEnumerator DisplayPositionHintCoroutine(Hint hint, string roomName)
    {
        DisplayText("\"Search this !\"");

        List<SpriteRenderer> spriteRenderersToFlicker = new List<SpriteRenderer>();
        
        roomDisplayManager.HideRoom();

        for (int i = 0; i < 10; i++)
        {
            
            yield return new WaitForSeconds(0.2f);
            
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitForSeconds(5f);

        roomDisplayManager.DisplayRoom();
        
        playerController.GiveControl();
        DisplayText(roomName);
    }
}
