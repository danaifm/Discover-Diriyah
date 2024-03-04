using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Discover : MonoBehaviour
{
    public TMP_InputField userName;
    public TextMeshProUGUI output; 

    public void button()
    {
        output.text = "hello there " +
            userName.text;


    }
}
