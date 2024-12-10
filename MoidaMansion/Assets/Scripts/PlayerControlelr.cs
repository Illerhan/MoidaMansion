using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private RoomDisplayManager roomDisplayManager;
    [SerializeField] private ProgressBar searchProgressBar;
    [SerializeField] private Minimap minimap;
    [SerializeField] private Monsta monsta;
    
    Vector2Int _position = new (0, 0);
    private Room _currentRoom;
    public int currentInspectIndex;
    private List<SpriteRenderer> inspectedSpriteRenderers;
    private bool isInspecting;
    private bool isSearchingFairy;
    private Coroutine currentCoroutine;
    public bool isChased;
    private int roomSwitchCount = 0;


    private void Start()
    {
        // We setup the variables
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
        _position = _currentRoom.coord;

        inspectedSpriteRenderers = new List<SpriteRenderer>();
            
        roomDisplayManager.Room = _currentRoom;
        roomDisplayManager.DisplayRoom();
        
        minimap.EnterRoom(_position);
    }

    public Room GetCurrentRoom()
    {
        return _currentRoom;
    }

    public void SwitchLevel()
    {
        if (_currentRoom.connectedDown)
            _position += new Vector2Int(0, -1);
        if (_currentRoom.connectedUp)
            _position += new Vector2Int(0, 1);
        
        // We actualise the variables
        GenProManager.Instance.ChangeCurrentRoom(_position);
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
        
        roomDisplayManager.Room = _currentRoom;
        roomDisplayManager.DisplayRoom();
        currentInspectIndex = 0;
        minimap.EnterRoom(_position);
        if (isChased)
        {
            roomSwitchCount += 1;
            if (roomSwitchCount <= 2)
                isChased = false;
        }
    }
    
    public void SwitchRoom(int direction)
    {
        if (direction > 0)
        {
            if (_position.x < 3 && _currentRoom.connectedRight && (!_currentRoom.isLockedRight || _currentRoom.isCodeLocked))
            {
                _position += new Vector2Int(direction, 0);
            }
            else if (_currentRoom.isLockedRight)
            {
                if (_currentRoom.isKeyLocked && inventoryManager.HasKey())
                {
                    _currentRoom.isKeyLocked = false;
                    _currentRoom.isLockedRight = false;
                    GenProManager.Instance.GetRoom(_position + new Vector2Int(1, 0)).isKeyLocked = false;
                    GenProManager.Instance.GetRoom(_position + new Vector2Int(1, 0)).isLockedLeft = false;
                    inventoryManager.UseKey();
                }
            }
        }
        else if (_position.x > 0 && _currentRoom.connectedLeft && (!_currentRoom.isLockedLeft || _currentRoom.isCodeLocked))
        {
            _position += new Vector2Int(direction, 0);
        }
        else if (_currentRoom.isLockedLeft)
        {
            if (_currentRoom.isKeyLocked && inventoryManager.HasKey())
            {
                _currentRoom.isKeyLocked = false;
                _currentRoom.isLockedLeft = false;
                GenProManager.Instance.GetRoom(_position + new Vector2Int(-1, 0)).isKeyLocked = false;
                GenProManager.Instance.GetRoom(_position + new Vector2Int(-1, 0)).isLockedRight = false;
                inventoryManager.UseKey();
            }
        }
        
        // Si le joueur était en train de fouiller
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        // We actualise the variables
        GenProManager.Instance.ChangeCurrentRoom(_position);
        _currentRoom = GenProManager.Instance.GetCurrentRoom();
        monsta.UpdatePosition();
        roomDisplayManager.Room = _currentRoom;
        roomDisplayManager.DisplayRoom();
        currentInspectIndex = 0;
        minimap.EnterRoom(_position);
        if (roomSwitchCount >= 3)
        {
            isChased = false;
            roomSwitchCount = 0;
        }
        roomSwitchCount++;
    }
    
    public void InspectItem()
    {
        isSearchingFairy = false;
        bool hasSearchableObjects = false;
        for (int i = 0; i < _currentRoom.roomSo.RoomObjects.Count; i++)
        {
            if (_currentRoom.roomSo.RoomObjects[i].CanBeSearched)
            {
                hasSearchableObjects = true;
            }
        }

        if (!hasSearchableObjects) return;
        
        if (isInspecting)
        {
            for(int i = 0; i < inspectedSpriteRenderers.Count; i++){
                inspectedSpriteRenderers[i].gameObject.SetActive(true);
            }
            
            currentInspectIndex++;
            inspectedSpriteRenderers.Clear();

            if (currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count)
            {
                if (GenProManager.Instance.fairiesManager.hasFairy)
                {
                    inspectedSpriteRenderers.Clear();
                    inspectedSpriteRenderers.Add(GenProManager.Instance.fairiesManager.GetCurrentFairy());
                    isSearchingFairy = true;
                }
                else
                {
                    currentInspectIndex++;
                } 
            }
            
            if(currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count + 1)
                currentInspectIndex = 0;

            if (currentInspectIndex < _currentRoom.roomSo.RoomObjects.Count)
            {
                while (!_currentRoom.roomSo.RoomObjects[currentInspectIndex].CanBeSearched)
                {
                    currentInspectIndex++;
                    // Fairies search
                    if (currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count)
                    {
                        if (GenProManager.Instance.fairiesManager.hasFairy)
                        {
                            inspectedSpriteRenderers.Clear();
                            inspectedSpriteRenderers.Add(GenProManager.Instance.fairiesManager.GetCurrentFairy());
                            isSearchingFairy = true;
                            break;
                        }
                        else
                        {
                            currentInspectIndex++;
                        }   
                    }
                
                    // Restart
                    if(currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count + 1)
                        currentInspectIndex = 0;
                }
            }
        }
        else
        {
            currentInspectIndex = 0;
            isInspecting = true;
            
            inspectedSpriteRenderers.Clear();
            
            while (!_currentRoom.roomSo.RoomObjects[currentInspectIndex].CanBeSearched)
            {
                currentInspectIndex++;
                if (currentInspectIndex >= _currentRoom.roomSo.RoomObjects.Count)
                {
                    // Fairies search
                    if (currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count)
                    {
                        if (GenProManager.Instance.fairiesManager.hasFairy)
                        {
                            inspectedSpriteRenderers.Clear();
                            inspectedSpriteRenderers.Add(GenProManager.Instance.fairiesManager.GetCurrentFairy());
                            isSearchingFairy = true;
                            break;
                        }
                        else
                        {
                            currentInspectIndex++;
                        }   
                    }
                    // Restart
                    if(currentInspectIndex == _currentRoom.roomSo.RoomObjects.Count + 1)
                        currentInspectIndex = 0;
                }
            }
        }
        
        if(inspectedSpriteRenderers.Count == 0)
            inspectedSpriteRenderers = roomDisplayManager.packedSpriteRenderers[currentInspectIndex];
        
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(InspectObjectCoroutine());
    }

    private IEnumerator InspectObjectCoroutine()
    {
        int currentCounter = 0;
        searchProgressBar.DisplayProgress(0);
        
        while (true)
        {
            if (currentCounter >= 13)
            {
                break;
            }
            
            for (int i = 0; i < inspectedSpriteRenderers.Count; i++)
            {
                inspectedSpriteRenderers[i].gameObject.SetActive(false);
            }
        
            yield return new WaitForSeconds(0.4f);

            currentCounter++;
            searchProgressBar.DisplayProgress(currentCounter);
        
            for (int i = 0; i < inspectedSpriteRenderers.Count; i++)
            {
                inspectedSpriteRenderers[i].gameObject.SetActive(true);
            }
        
            yield return new WaitForSeconds(0.4f);
            
            currentCounter++;
            searchProgressBar.DisplayProgress(currentCounter);
        }
        
        searchProgressBar.DisplayProgress(0);
        isChased = true;

        currentInspectIndex = 0;

        if (isSearchingFairy)
        {
            inventoryManager.FoundCodePart();
            GenProManager.Instance.fairiesManager.PickFairy();
            yield break;
        }
        

        ItemType foundItem = GenProManager.Instance.VerifySearch(_position, currentInspectIndex);
        
        switch (foundItem)
        {
            case ItemType.None :
                Debug.LogError("Rien trouvé");
                break;
            
            case ItemType.Friend :
                if (GenProManager.Instance.codeLockedLocation == _position)
                {
                    if (!inventoryManager.HasFullCode())
                    {
                        break;
                    }
                    inventoryManager.UseCode();
                    GenProManager.Instance.codeLockedLocation = new Vector2Int(-1, -1);
                }
                
                GenProManager.Instance.foundFriendsIndexes.Add(GenProManager.Instance.foundFriendsIndexes.Count);
                roomDisplayManager.DisplayRoom();
                inventoryManager.FoundFriend();
                break;
            
            case ItemType.Key :
                inventoryManager.FoundKey();
                break;
            
            case ItemType.Code :
                inventoryManager.FoundFullCode();
                break;
            
            case ItemType.SecretPassage :
                if (_currentRoom.isSecretLocked)
                {
                    if (_currentRoom.isLockedLeft)
                    {
                        _currentRoom.isLockedLeft = false;
                        GenProManager.Instance.GetRoom(_position + new Vector2Int(-1, 0)).isLockedRight = false;
                    }
                    else
                    {
                        GenProManager.Instance.GetRoom(_position + new Vector2Int(1, 0)).isLockedLeft = false;
                        _currentRoom.isLockedRight = false;
                    }
                    
                    _currentRoom.isSecretLocked = false;
                    
                    roomDisplayManager.DisplayRoom();
                }

                if (isChased)
                {
                    monsta.MonstaAttack();
                }
                Debug.Log("J'ai trouvé un passage secret !");
                break;
        }
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
