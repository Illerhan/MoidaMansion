using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<String, bool> foundFriends = new Dictionary<string, bool>();
    private int _code = 0;
    private bool _key = false;
    public GameObject Cal;
    public GameObject Ace;
    public GameObject Bek;
    public GameObject Dot;
    public GameObject Code1;
    public GameObject Code2;
    public GameObject Code3;
    public GameObject Key;
    
    
    
    // Call this function to add friend to inventory and display it.
    public void FoundingFriend(String name)
    {
        foundFriends[name] = true;
        switch (name)
        {
            case "Ace":
                Ace.SetActive(true);
                break;
            case "Dot":
                Dot.SetActive(true);
                break;
            case "Cal":
                Cal.SetActive(true);
                break;
            case "Bek":
                Bek.SetActive(true);
                break;
            default:
                break;
        }
        
    }

    bool HasFullCode()
    {
        if (_code == 3)
            return true;
        return false;
    }

    // Used when picking up a code piece or full code to display
    void codeDisplay()
    {
        if (_code >= 1)
            Code1.SetActive(true);
        if (_code >= 2)
            Code2.SetActive(true);
        if (_code == 3)
            Code3.SetActive(true);
    }

    void displayKey()
    {
        Key.SetActive(true);
    }

    bool HasKey()
    {
        return _key;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foundFriends.Add("Ace",false);
        foundFriends.Add("Cal",false);
        foundFriends.Add("Bek",false);
        foundFriends.Add("Dot",false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FoundingFriend("Ace");
        }
    }
}
