using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class test : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TMP_InputField username;

    public void demoButton()
    {
        output.text = "Good Luck, "+ username.text;
    }

}
