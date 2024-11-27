using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;
using Vector3 = System.Numerics.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private RoomDisplayManager roomDisplayManager;
    
    Vector2Int _position = new (0, 0);
    private Room _currentRoom;
    private int currentInspectIndex;
    private List<SpriteRenderer> inspectedSpriteRenderers;
    private bool isInspecting;
    private Coroutine currentCoroutine;


    private void Start()
    {
        // We setup the variables
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
        _position = _currentRoom.coord;
        
        roomDisplayManager.Room = _currentRoom;
        roomDisplayManager.DisplayRoom();
    }

    public void SwitchLevel()
    {
        if (_currentRoom.connectedDown)
            _position += new Vector2Int(0, -1);
        if (_currentRoom.connectedUp)
            _position += new Vector2Int(0, 1);
        Debug.Log("New position = " + _position);
        
        // We actualise the variables
        GenProManager.Instance.ChangeCurrentRoom(_position);
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
    }
    
    public void SwitchRoom(int direction)
    {
        if (direction > 0)
        {
            if (_position.x < 3 && _currentRoom.connectedRight && !_currentRoom.isLockedRight)
            {
                _position += new Vector2Int(direction, 0);
            }
        }
        else if(_position.x > 0 && _currentRoom.connectedLeft && !_currentRoom.isLockedLeft)
            _position += new Vector2Int(direction, 0);
        Debug.Log("New position = " + _position);
        
        // Si le joueur était en train de fouiller
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        // We actualise the variables
        GenProManager.Instance.ChangeCurrentRoom(_position);
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
        
        roomDisplayManager.Room = _currentRoom;
        roomDisplayManager.DisplayRoom();
    }


    public void InspectItem()
    {
        if (isInspecting)
        {
            currentInspectIndex++;
            if (currentInspectIndex >= _currentRoom.roomSo.RoomObjects.Count)
                currentInspectIndex = 0;
        }
        else
        {
            currentInspectIndex = 0;
        }
        
        // Missing : get sprites
        
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(InspectObjectCoroutine());
    }

    private IEnumerator InspectObjectCoroutine()
    {
        int currentCounter = 0;
        
        while (true)
        {
            if (currentCounter >= 10)
            {
                break;
            }
            
            for (int i = 0; i < inspectedSpriteRenderers.Count; i++)
            {
                inspectedSpriteRenderers[i].gameObject.SetActive(false);
            }
        
            yield return new WaitForSeconds(0.5f);

            currentCounter++;
        
            for (int i = 0; i < inspectedSpriteRenderers.Count; i++)
            {
                inspectedSpriteRenderers[i].gameObject.SetActive(true);
            }
        
            yield return new WaitForSeconds(0.5f);
            
            currentCounter++;
        }
        
        
        Debug.Log("C'est fouillé");
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
