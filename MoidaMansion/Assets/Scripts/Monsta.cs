using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Monsta : MonoBehaviour
{
    public GameObject _player;
    private Coroutine _coroutine;
    public List<GameObject> _monstaSprite = new List<GameObject>();
    private GameObject _selectedMonsta;
    private bool isRunning;
    private RoomSo _currentRoom;
    private PlayerController _playerController;
    private List<ObjectSo> _objectSos;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Start()
    {
        
        _currentRoom = _playerController.GetCurrentRoom().roomSo;
        _objectSos = _currentRoom.RoomObjects;
        foreach (var objectSo in _objectSos)
        {
            if (objectSo.CanHaveMonsta)
            {
                _selectedMonsta = _monstaSprite[objectSo.MonstaSprite];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerController.isChased)
        {
            StopCoroutine(MonstaCoroutine());
            _selectedMonsta.SetActive(false);
            return;
        }

        if(!isRunning)
            _coroutine  = StartCoroutine(MonstaCoroutine());
            
    }
    
    public IEnumerator MonstaCoroutine()
    {
        isRunning = true;
        int currentCounter = 0;
        _selectedMonsta.SetActive(true);
        while (true)
        {
            if (currentCounter >= 13)
            {
                _selectedMonsta.SetActive(false);
                isRunning = false;
                yield break;
            }

            _selectedMonsta.SetActive(false);
            
            yield return new WaitForSeconds(0.4f);
            currentCounter++;

            _selectedMonsta.SetActive(true);

            yield return new WaitForSeconds(0.4f);
            currentCounter++;
        }

    }

    public void UpdatePosition()
    {
        _selectedMonsta.SetActive(false);
        _currentRoom = _playerController.GetCurrentRoom().roomSo;
        _objectSos = _currentRoom.RoomObjects;
        foreach (var objectSo in _objectSos)
        {
            if (objectSo.CanHaveMonsta)
            {
                _selectedMonsta = _monstaSprite[objectSo.MonstaSprite];
                _selectedMonsta.SetActive(true);
            }
        }
    }
    
}
