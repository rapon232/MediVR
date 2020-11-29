/*

    MediVR, a medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

    Copyright (C) 2020  Dimitar Tahov

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    This script serves to momentarily change button text and color after pressing of said button.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeButtonLook : MonoBehaviour
{
    public float countdownSeconds = 5.0f;

    private Button button = null;
    private ColorBlock buttonColor = ColorBlock.defaultColorBlock;
    private Image buttonImage = null;
    private Text buttonText = null;

    private Color inactiveColor = Color.clear;
    private string inactiveText = "";

    private float countdownTime = 0f;
    private bool countdown = false;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        buttonColor = button.colors;

        buttonImage = GetComponent<Image>();   
        buttonText = GetComponentInChildren<Text>();    
        
        inactiveColor = buttonImage.color;
        inactiveText = buttonText.text;

        countdownTime  = countdownSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if(countdown)
        {
            countdownTime -= Time.deltaTime;
            if(countdownTime < 0)
            {
                countdown = false;
                countdownTime = countdownSeconds;
                buttonText.text = inactiveText;
                ToggleColor();
            }
        }
        
    }

    //TOGGLE ACTIVENESS OF GAME OBJECT
    public void ToggleActive(GameObject GO)
    {
        if(GO.activeSelf)
        {
            GO.SetActive(false);
        }
        else
        {
            GO.SetActive(true);
        }
    }

    //TOGGLE BUTTON COLOR
    public void ToggleColor()
    {
        if(buttonImage.color == Color.black)
        {
            buttonImage.color = inactiveColor;
            buttonText.color = Color.black;
        }
        else
        {
            buttonImage.color = Color.black;
            buttonText.color = Color.white;
        }
    }

    //TOGGLE BUTTON TEXT COLOR
    public void ToggleColorText(string Text)
    {
        if(buttonImage.color == Color.black)
        {
            buttonImage.color = inactiveColor;
            buttonText.text = inactiveText;
            buttonText.color = Color.black;
        }
        else
        {
            buttonImage.color = Color.black;
            buttonText.color = Color.white;
            buttonText.text = Text;
        }
    }

    //TOGGLE BUTTON COLOR OVER SPECIFIED PERIOD OF TIME
    public void ToggleColorTextOverTime(string timedText)
    {
        if(countdown)
        {
            // do nothing
        }
        else
        {
            countdown = true;
            buttonText.text = timedText;
            ToggleColor();
        }
    }
}
