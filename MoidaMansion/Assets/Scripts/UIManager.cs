using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")] 
    [SerializeField] private TextMeshProUGUI mainText;
    
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
        DisplayText("Rescued " + friendName + "!");
        yield return new WaitForSeconds(2f);
        DisplayText("\"thank you!\"");
        yield return new WaitForSeconds(2f);
        DisplayText(roomName);
    }
    
    private IEnumerator DisplayLargeTextCoroutine(string text)
    {
        for (int i = 0; i < text.Length - 12; i++)
        {
            DisplayText(text[new Range(i, i+13)]);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DisplayGhostHintCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator DisplayFairiesHintCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator DisplayPositionHintCoroutine(Hint hint)
    {
        yield return new WaitForSeconds(2f);
    }
}
