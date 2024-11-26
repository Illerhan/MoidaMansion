using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenProManager : MonoBehaviour
{
    public static GenProManager Instance;

    [Header("Public Infos")]
    public Room[,] mansionMap = new Room[4, 3];
    public Vector2Int[] friendPositions = new Vector2Int[3];

    [Header("Private Infos")] 
    private Vector2Int startPos;
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateMansion();
    }

    private void VerifyRoomAccessibility(Vector2Int startPos, Vector2Int wantedPos)
    {
        
    }


    public void GenerateMansion()
    {
        startPos = new Vector2Int(Random.Range(0, 4), Random.Range(0, 2));
        
        
    }
    
}
