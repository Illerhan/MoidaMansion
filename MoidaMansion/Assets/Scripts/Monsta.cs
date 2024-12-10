using System.Collections.Generic;
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
    [SerializeField] private GameObject _Minimap;
    private bool _isRunning;
    private RoomSo _currentRoomSo;
    private Room _currentRoom;
    private PlayerController _playerController;
    [SerializeField] private GameObject roomSpriteRenderer;
    private List<ObjectSo> _objectSos;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Start()
    {
        //UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_selectedMonsta)
            UpdatePosition();
        if (!_playerController.isChased && _selectedMonsta)
        {
            _selectedMonsta?.SetActive(false);
        }
        if(!_selectedMonsta.activeSelf && _playerController.isChased)
            ShowMonsta();
    }

    public void ShowMonsta()
    {
        _selectedMonsta.SetActive(true);
    }
    public void HideMonsta()
    {
        _selectedMonsta.SetActive(false);
    }

    public void UpdatePosition()
    {
        _possibleMonsta.Clear();
        _currentRoom = _playerController.GetCurrentRoom();
        _currentRoomSo = _currentRoom.roomSo;
        _objectSos = _currentRoomSo.RoomObjects;
        if (_currentRoomSo.RoomType == RoomType.Void)
        {
            return;
        }
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
        _selectedMonsta = _possibleMonsta[Random.Range(0, _possibleMonsta.Count - 1)];
    }

    public GameObject GetSelectedMonsta()
    {
        return _selectedMonsta;
    }

    public void MonstaAttack()
    { 
        GenProManager.Instance.buttonsManager.HideButtons();
        _playerController.roomDisplayManager.HideRoom();
        _playerController.isChased = false;
        //roomSpriteRenderer.SetActive(false);
        HideMonsta();
        _Minimap.SetActive(false);
        _selectedMonsta.SetActive(false);
       _monstaBlood.SetActive(true);
       _monstaHand.SetActive(true);
       _playerController.StopControl();
    }
}
