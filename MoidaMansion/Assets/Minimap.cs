using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Image[] roomsPositions = new Image[12];

    public void EnterRoom(Vector2Int coord)
    {
        int currentCoord = coord.y * 4 + coord.x;

        for (int i = 0; i < roomsPositions.Length; i++)
        {
            if (currentCoord == i)
            {
                roomsPositions[i].enabled = true;
            }
            else
            {
                roomsPositions[i].enabled = false;
            }
        }
    }
}
