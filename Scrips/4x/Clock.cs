using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clock : MonoBehaviour
{
    public Text clockText;

    private void Awake()
    {
        clockText = GetComponent<Text>();
    }

    private void Update()
    {
        DateTime time = DateTime.Now;
        String hour =  Zero(time.Hour);
        String minute = Zero(time.Minute);
        String seconds = Zero(time.Second);

        clockText.text = hour + ":" + minute + ":" + seconds; 
    }

    string Zero (int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}

