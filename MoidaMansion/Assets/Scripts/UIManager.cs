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
        mainText.enabled = true;
        mainText.text = text;
    }

    public void HideText()
    {
        mainText.enabled = false;
    }
    
    public void DisplayRescueText(string roomName, string friendName)
    {
        StartCoroutine(DisplayRescueTextCoroutine(roomName, friendName));
    }
    
    private IEnumerator DisplayRescueTextCoroutine(string roomName, string friendName)
    {
        DisplayText("Rescued " + friendName + "!");
        yield return new WaitForSeconds(2f);
        DisplayText("\"thank you!\"");
        yield return new WaitForSeconds(2f);
        DisplayText(roomName);
    }
}
