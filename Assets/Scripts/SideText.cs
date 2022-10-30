using System;
using UnityEngine;
using UnityEngine.UI;

public class SideText : MonoBehaviour
{
    // Start is called before the first frame update
    private readonly string[] texts1 = new string[5] { "GG :-)", "Level99", "Mind games", "5 stars", "e2-e4" };
    private readonly string[] texts2 = new string[5] { "Your turn, baby", "Another game maybe?", "The best of the best", "Is it Deep Blue?", "Sicilian Defense" };

    void Start()
    {
        switch (gameObject.name)
        {
            case "SideText1":
                gameObject.GetComponent<Text>().text = texts1[UnityEngine.Random.Range(0, 5)];
                break;
            case "SideText2":
                gameObject.GetComponent<Text>().text = texts2[UnityEngine.Random.Range(0, 5)];
                break;
        }
        
    }
}
