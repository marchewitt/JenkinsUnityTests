using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChangeText : MonoBehaviour
{
    private Text TextRef;
    // Start is called before the first frame update
    void Awake()
    {
        TextRef = this.gameObject.GetComponent<Text>();
        Assert.IsNotNull(TextRef, "TextRef can not be null");
        ChangeTheText("Hello1");
    }

    private void ChangeTheText(string newText)
    {
        TextRef.text = newText;
    }
}
