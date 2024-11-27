using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<String, bool> foundFriends = new Dictionary<string, bool>();
    private int _code = 0;
    private bool _key;
    
    public void FoundingFriend(String name)
    {
        foundFriends[name] = true;
    }

    bool HasFullCode()
    {
        if (_code == 0)
            return true;
        return false;
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
        
    }
}
