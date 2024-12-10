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
}
