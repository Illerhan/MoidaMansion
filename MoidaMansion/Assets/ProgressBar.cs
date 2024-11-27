using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private List<Image> images;

    private void Start()
    {
        DisplayProgress(0);
    }

    public void DisplayProgress(int currentIndex)
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (currentIndex > i)
            {
                images[i].enabled = true;
            }
            else
            {
                images[i].enabled = false;
            }
        }
    }
}
