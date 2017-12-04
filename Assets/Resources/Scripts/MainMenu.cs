using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    private Button startButton;
    private Button howToButton;
    private Button exitButton;

	// Use this for initialization
	void Start () {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.GetComponent<Button>().onClick.AddListener(OnStartClick);

        howToButton = GameObject.Find("HowToButton").GetComponent<Button>();
        howToButton.GetComponent<Button>().onClick.AddListener(OnHowToClick);

        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
    }

    void OnStartClick() {
        SceneManager.LoadScene("Level 1");
    }

    void OnHowToClick() {
        SceneManager.LoadScene("How To");
    }

    void OnExitClick() {
        Application.Quit();
    }

}
