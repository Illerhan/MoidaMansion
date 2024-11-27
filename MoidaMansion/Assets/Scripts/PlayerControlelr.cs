using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;
using Vector3 = System.Numerics.Vector3;

public class PlayerController : MonoBehaviour

{
    [SerializeField]
    Button rightButton;

    [SerializeField]
    private InventoryManager inventoryManager;
    Vector2 _position = new (0, 0);
    
    public void SwitchLevel(int doorDirection)
    {
        _position += new Vector2(0, doorDirection);
        Debug.Log("New position = " + _position);
    }
    
    public void SwitchRoom(int direction)
    {
        if (direction > 0)
        {
            if (_position.x < 3)
            {
                _position += new Vector2(direction, 0);
            }else return;
        }else
        if(_position.x > 0)
            _position += new Vector2(direction, 0);
        
        Debug.Log("New position = " + _position);
       
    }

    // void inspectItem(itemToInspect)
    // {
    //     if (itemToInspect.isButton) ;
    //         if(inventory.friends.count>0)
    //             // Take a friend to push the button
    //     if (itemToInspect.isFalse) ;
    //         // Dialogue nothing here.
    //         // wrongSearch ++ ( for the Monsta spawn systeme)
    //     if (itemToInspect.isFriend) ;
    //         // Display friend found + dialogue
    //         inventoryManager.FoundingFriend(friendName);
    //     if (itemToInspect.isLockedKey)
    //          if(inventory.key == true).
    //              open
    //     if (itemToInspect.isLockedCode)
    //          if(inventory.code == 3).
    //              open
    // }
}
