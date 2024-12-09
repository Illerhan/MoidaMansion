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
    private int friendCount = 0;
    public GameObject Cal;
    public GameObject Ace;
    public GameObject Bek;
    public GameObject Dot;
    public GameObject Code1;
    public GameObject Code2;
    public GameObject Code3;
    public GameObject Key;
    
    
    
    // Call this function to add friend to inventory and display it.
    public void FoundFriend()
    {
        foundFriends[name] = true;
        switch (friendCount)
        {
            case 0:
                Ace.SetActive(true);
                foundFriends["Ace"] = true;
                break;
            case 1:
                Dot.SetActive(true);
                foundFriends["Dot"] = true;
                break;
            case 2:
                Cal.SetActive(true);
                foundFriends["Cal"] = true;
                break;
            case 3:
                Bek.SetActive(true);
                foundFriends["Bek"] = true;
                break;
            default:
                break;
        }
        
        friendCount++;
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

    public void FoundFullCode()
    {
        _code = 3;
        Code1.SetActive(true);
        Code2.SetActive(true);
        Code3.SetActive(true);
    }

    public bool HasFullCode()
    {
        return _code == 3;
    }

    public void UseCode()
    {
        _code = 0;
        Code1.SetActive(false);
        Code2.SetActive(false);
        Code3.SetActive(false);
    }
    
    public void FoundKey()
    {
        _key = true;
        Key.SetActive(true);
    }

    public void UseKey()
    {
        _key = false;
        Key.SetActive(false);
    }
    
    public bool HasKey()
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
            FoundFriend();
        }
    }
}
