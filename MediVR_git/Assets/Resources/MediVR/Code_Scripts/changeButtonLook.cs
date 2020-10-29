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
    //private Color buttonImageColor;
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

    public void ToggleColor()
    {
        //buttonColor = button.colors;
        //buttonImageColor = buttonImage.color;

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

        //button.colors = buttonColor;
    }

    public void ToggleColorText(string Text)
    {
        //buttonColor = button.colors;
        //buttonImageColor = buttonImage.color;

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

        //button.colors = buttonColor;
    }

    public void ToggleColorTextOverTime(string timedText)
    {
        //buttonColor = button.colors;
        //buttonImageColor = buttonImage.color;

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

        //button.colors = buttonColor;
    }
}
