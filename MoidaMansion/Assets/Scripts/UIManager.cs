using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")] 
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RoomDisplayManager roomDisplayManager;
    [SerializeField] private GhostManager ghostManager;
    [SerializeField] private FairiesManager fairiesManager;
    [SerializeField] private GameObject MiniMap;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject upStairs;
    [SerializeField] private GameObject downStairs;

    [Header("Title Screen")]
    [SerializeField] private GameObject atlas;

    [SerializeField] private GameObject titleScene;
    private Coroutine _titleScreenCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        upStairs.SetActive(false);
        downStairs.SetActive(false);
        MiniMap.SetActive(false);
        roomDisplayManager.HideRoom();
        playerController.StopControl();
        
        ShowTitleScreen();
    }
    
    
    public void ShowTitleScreen()
    {
        _titleScreenCoroutine = StartCoroutine(ShowTitleScreenCoroutine());
    }
    
    private IEnumerator ShowTitleScreenCoroutine()
    {
        atlas.SetActive(true);
        yield return new WaitForSeconds(1f);
        atlas.SetActive(false);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(StartGame);
        }
        yield return new WaitForSeconds(0.5f);
        DisplayText(Random.Range(0, 2) == 0 ? "welcome to" : "beware of");
        yield return new WaitForSeconds(2f);
        titleScene.SetActive(true);
        DisplayText("Moida Mansion");
        yield return new WaitForSeconds(3.5f);
        DisplayText("by");
        yield return new WaitForSeconds(1f);
        DisplayText("Lucas Pope");
        yield return new WaitForSeconds(3.5f);
        DisplayText("(C) 3909 LLC");
        yield return new WaitForSeconds(3.5f);
        DisplayText("1.0.8");
        yield return new WaitForSeconds(1f);
        while (true)
        {
            DisplayText("Rescue your friends!    Rescue your ");
            yield return new WaitForSeconds(12f);
        }
    }
    
    private void StartGame()
    {
        StopCoroutine(_titleScreenCoroutine);
        titleScene.SetActive(false);
        playerController.GiveControl();
        MiniMap.SetActive(true);
        roomDisplayManager.DisplayRoom();
        
        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(StartGame);
        }
    }
    
    
    public void ShowMoveHud(bool left, bool right, bool up, bool down)
    {
        leftArrow.SetActive(left);
        rightArrow.SetActive(right);
        upStairs.SetActive(up);
        downStairs.SetActive(down);
    }


    public IEnumerator DisplayTextCoroutine(string newText, string endText, float duration)
    {
        if (endText == null)
        {
            endText = mainText.text;
        }
        
        DisplayText(newText);
        playerController.StopControl();
        
        yield return new WaitForSeconds(duration);
        
        playerController.GiveControl();
        DisplayText(endText);
    }

    public void DisplayText(string text)
    {
        if (text != null)
        {
            if (text.Length > 13)
            {
                StartCoroutine(DisplayLargeTextCoroutine(text));
                return;
            }
        
            mainText.enabled = true;
            mainText.text = text;
        }
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

        List<SpriteRenderer> spriteRenderersToFlicker = roomDisplayManager.GetSpriteRenderersToFlicker(GenProManager.Instance.mansionMap[hint.itemLocation.roomCoord.x, hint.itemLocation.roomCoord.y], 
            hint.itemLocation.itemIndex);
        
        //roomDisplayManager.HideRoom();

        for (int i = 0; i < 10; i++)
        {
            foreach (var spriteRenderer in spriteRenderersToFlicker)
            {
                spriteRenderer.enabled = false;
            }
            
            yield return new WaitForSeconds(0.25f);
            
            foreach (var spriteRenderer in spriteRenderersToFlicker)
            {
                spriteRenderer.enabled = true;
            }
            
            yield return new WaitForSeconds(0.25f);
        }
        
        foreach (var spriteRenderer in spriteRenderersToFlicker)
        {
            spriteRenderer.enabled = false;
        }

        roomDisplayManager.DisplayRoom();
        
        playerController.GiveControl();
        DisplayText(roomName);
    }
}
