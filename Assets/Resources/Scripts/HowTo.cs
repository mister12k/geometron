using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowTo : MonoBehaviour {
    
    private Button exitButton;

    // Use this for initialization
    void Start()
    {
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
    }

    void OnExitClick()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
