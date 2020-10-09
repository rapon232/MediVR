using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeButtonLook : MonoBehaviour
{
    private Button button;
    private ColorBlock buttonColor;
    private Image buttonImage;
    //private Color buttonImageColor;
    private Text buttonText;

    private Color inactiveColor;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        buttonColor = button.colors;

        buttonImage = GetComponent<Image>();   
        buttonText = GetComponentInChildren<Text>();    
        

        inactiveColor = buttonImage.color;

    }

    // Update is called once per frame
    void Update()
    {
        
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
}
