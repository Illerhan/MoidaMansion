using System;
using System.Collections;
using UnityEngine;

public class Monsta : MonoBehaviour
{
    public GameObject _player;
    private Coroutine _coroutine;
    public GameObject _monstaSprite;
    private bool isRunning;
    private PlayerController _playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerController.isChased)
        {
            return;
        }
        if(!isRunning)
            _coroutine  = StartCoroutine(MonstaCoroutine());
            
    }
    
    private IEnumerator MonstaCoroutine()
    {
        isRunning = true;
        int currentCounter = 0;
        _monstaSprite.SetActive(true);
        while (true)
        {
            if (currentCounter >= 13)
            {
                break;
            }

            _monstaSprite.SetActive(false);
            
            yield return new WaitForSeconds(0.4f);
            currentCounter++;

            _monstaSprite.SetActive(true);

            yield return new WaitForSeconds(0.4f);
            currentCounter++;
        }
        isRunning = false;
    }
    
}
