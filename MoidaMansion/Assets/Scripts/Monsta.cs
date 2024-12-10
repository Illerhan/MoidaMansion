using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monsta : MonoBehaviour
{
    public GameObject _player;
    private Coroutine _coroutine;
    public List<GameObject> _monstaSprite = new List<GameObject>();
    private List<GameObject> _possibleMonsta = new List<GameObject>();
    private int _MonstaIndex;
    private GameObject _selectedMonsta;
    [SerializeField] private GameObject _monstaHand;
    [SerializeField] private GameObject _monstaBlood;
    private bool _isRunning;
    private RoomSo _currentRoomSo;
    private Room _currentRoom;
    private PlayerController _playerController;
    private List<ObjectSo> _objectSos;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Start()
    {
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_selectedMonsta)
            UpdatePosition();
        if (!_playerController.isChased)
        {
            StopCoroutine(MonstaCoroutine());
            _selectedMonsta?.SetActive(false);
            return;
        }

        if(!_isRunning)
            _coroutine  = StartCoroutine(MonstaCoroutine());
            
    }
    
    public IEnumerator MonstaCoroutine()
    {
        _isRunning = true;
        _selectedMonsta.SetActive(true);
        while (true)
        {
            _selectedMonsta.SetActive(false);
            
            yield return new WaitForSeconds(0.4f);

            _selectedMonsta.SetActive(true);

            yield return new WaitForSeconds(0.4f);
        }

    }

    public void UpdatePosition()
    {
        _possibleMonsta.Clear();
        if(_selectedMonsta)
            _selectedMonsta?.SetActive(false);
        _currentRoom = _playerController.GetCurrentRoom();
        _currentRoomSo = _currentRoom.roomSo;
        _objectSos = _currentRoomSo.RoomObjects;
        foreach (var objectSo in _objectSos)
        {
            if (objectSo.CanHaveMonsta)
            {
                _MonstaIndex = objectSo.MonstaSprite;
                _possibleMonsta.Add(_monstaSprite[_MonstaIndex]);
            }

            if (_currentRoom.connectedDown && _currentRoom.connectedLeft || _currentRoom.connectedDown && _currentRoom.connectedRight 
                || _currentRoom.connectedUp && _currentRoom.connectedLeft || _currentRoom.connectedUp && _currentRoom.connectedRight
                || _currentRoom.connectedRight && _currentRoom.connectedLeft)
            {
                if (_currentRoom.connectedRight && _currentRoom.rightDoor.CanHaveMonsta)
                {
                    _MonstaIndex = _currentRoom.rightDoor.MonstaSprite;
                    _possibleMonsta.Add(_monstaSprite[_MonstaIndex]);
                }
                if (_currentRoom.connectedLeft && _currentRoom.leftDoor.CanHaveMonsta)
                {
                    _MonstaIndex = _currentRoom.leftDoor.MonstaSprite;
                    _possibleMonsta.Add(_monstaSprite[_MonstaIndex]);
                }
                if (_currentRoom.connectedUp ||_currentRoom.connectedDown)
                {
                    _MonstaIndex = _currentRoom.stairs.MonstaSprite;
                    _possibleMonsta.Add(_monstaSprite[_MonstaIndex]);
                }
            }
        }
        //_selectedMonsta = _possibleMonsta[Random.Range(0, _possibleMonsta.Count - 1)];
    }

    public void MonstaAttack()
    {
       StopCoroutine(MonstaCoroutine());
       _selectedMonsta.SetActive(false);
       _monstaBlood.SetActive(true);
       _monstaHand.SetActive(true);
    }
}
