﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private string shapeSelected;

	private Button moveButton;
    private Button interactButton;
    private Button endButton;

    private bool movePressed;
    private bool interactPressed;

    private GameObject interactTip; 
    private Text interactText;

	private List<Tile> moveArea;
    private List<Tile> interactTiles;

    private bool movedShape;
    private bool interactedShape;

    private Text turnsLeftNumber;

    private GameObject menu;
    private Button restartButton;
    private Button exitButton;

    private GameObject alert;

    // Use this for initialization
    void Start () {

        movePressed = false;
        interactPressed = false;
		moveButton = GameObject.Find ("MoveButton").GetComponent<Button>();
		moveButton.GetComponent<Button>().onClick.AddListener(OnMoveClick);
        moveButton.gameObject.SetActive (false);

        endButton = GameObject.Find("EndButton").GetComponent<Button>();
        endButton.GetComponent<Button>().onClick.AddListener(OnEndClick);
        endButton.gameObject.SetActive(false);

        turnsLeftNumber = GameObject.Find("Turns Number").GetComponentInChildren<Text>();
        switch (SceneManager.GetActiveScene().name){
            case "Level 1":
                turnsLeftNumber.text = Constants.TURNS_LEVEL1.ToString();
                break;
            case "Level 2":
                turnsLeftNumber.text = Constants.TURNS_LEVEL2.ToString();
                break;
            case "Level 3":
                turnsLeftNumber.text = Constants.TURNS_LEVEL3.ToString();
                break;
        }

        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        restartButton.GetComponent<Button>().onClick.AddListener(OnRestartClick);
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
        menu = GameObject.Find("Menu");
        menu.SetActive(false);

        interactButton = GameObject.Find("InteractButton").GetComponent<Button>();
        interactButton.GetComponent<Button>().onClick.AddListener(OnInteractClick);
        interactButton.image.color = Constants.COLOR_BUTTON_CLICKED;
        interactButton.gameObject.SetActive(false);

        interactButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry eventtypeenter = new EventTrigger.Entry();
        eventtypeenter.eventID = EventTriggerType.PointerEnter;
        eventtypeenter.callback.AddListener((eventData) => { OnInteractHoverEnter(); });
        interactButton.GetComponent<EventTrigger>().triggers.Add(eventtypeenter);

        EventTrigger.Entry eventtypeexit = new EventTrigger.Entry();
        eventtypeexit.eventID = EventTriggerType.PointerExit;
        eventtypeexit.callback.AddListener((eventData) => { OnInteractHoverExit(); });
        interactButton.GetComponent<EventTrigger>().triggers.Add(eventtypeexit);

        interactText = GameObject.Find("InteractTip").GetComponentInChildren<Text>();
        interactTip = GameObject.Find("InteractTip");
        interactTip.SetActive(false);

        alert = GameObject.Find("Alert");
        alert.SetActive(false);

        moveArea = new List<Tile>();
        interactTiles = new List<Tile>();
    }


    /**
     *  Method which allows to reset the buttons on the UI depending on the state of the 
     *  booleans given through arguments. It will click or unclick buttons as needed
     *  depending on the current state of the game.
     */
    public void SetButtons(bool hasMoved, bool hasInteracted, string name) {
        shapeSelected = name;
        movedShape = hasMoved;
        interactedShape = hasInteracted;

        Vector3 selectedPosition = new Vector3();
        bool interactableObjects = false;
        moveButton.gameObject.SetActive(true);
        interactButton.gameObject.SetActive(true);
        endButton.gameObject.SetActive(true);

        if (!hasMoved) {
            restoreMoveButton();
        } else {
            SetButtonMoveClicked();
        }
        
        if (!hasInteracted) {

            interactButton.image.color = Constants.COLOR_BUTTON_CLICKED;

            selectedPosition = GameObject.Find("Selected").transform.position;

            if (name != "Plank") {
                if(Pathing.NeighbouringUnits(selectedPosition).Count > 0) {
                    interactableObjects = true;
                }
            } else {
                if(Pathing.NeighbouringSpaces(selectedPosition).Count > 0) {
                    interactableObjects = true;
                }
            }

            if (interactableObjects) {
                restoreInteractButton();
            } else {
                SetButtonInteractClicked();
            }
        } else {
            SetButtonInteractClicked();
        }

    }

    public void SetButtonMoveClicked() {
        moveButton.image.color = Constants.COLOR_BUTTON_CLICKED;
        movePressed = true;
        unhighlightMoveArea();
    }

    public void SetButtonInteractClicked() {
        interactButton.image.color = Constants.COLOR_BUTTON_CLICKED;
        interactPressed = true;
        HideInteractTip();
        UnhighlightInteractable();
    }

    /**
	 * 	Listener active when the move button is pressed, sets the button's state
	 * 	and if it highlights or not the available move area for the currently selected shape
	 */
    void OnMoveClick(){
        if (!(moveButton.image.color == Constants.COLOR_BUTTON_CLICKED && movePressed)) {
            if(!interactedShape) restoreInteractButton();
            movePressed = !movePressed;
            if (movePressed) {
                moveButton.image.color = Constants.COLOR_BUTTON_CLICKED;
                highlightMoveArea();
            } else {
                moveButton.image.color = Constants.COLOR_BUTTON_UNCLICKED;
                unhighlightMoveArea();
            }
        } else {
            if (movedShape) {
                StartCoroutine(ShowMessage("This shape has already moved this turn", 1.5f));
            }
        }
	}

    /**
	 * 	Listener active when the interact button is pressed, sets the button's state
     * 	and if it highlights or not the interactable shapes and hides/shows the interaction tip.
	 */
    void OnInteractClick() {
        if (!(interactButton.image.color == Constants.COLOR_BUTTON_CLICKED && interactPressed)) {
            if(!movedShape) restoreMoveButton();
            interactPressed = !interactPressed;
            if (interactPressed) {
                interactButton.image.color = Constants.COLOR_BUTTON_CLICKED;
                ShowInteractTip();
                HighlightInteractable();
            } else {
                interactButton.image.color = Constants.COLOR_BUTTON_UNCLICKED;
                HideInteractTip();
                UnhighlightInteractable();
            }
        } else {
            if (shapeSelected != "Mini Cube") {
                if (interactedShape) {
                    StartCoroutine(ShowMessage("This shape has already interacted", 1.5f));
                } else {
                    if (interactTiles.Count == 0) {
                        StartCoroutine(ShowMessage("This shape can't interact in this position", 1.5f));
                    }
                }
            } else {
                StartCoroutine(ShowMessage("This shape doesn't have the ability to interact", 1.5f));
            }
        }
    }

    void OnEndClick() {
        foreach(var unit in GameObject.FindGameObjectsWithTag("Unit")) {
            unit.GetComponentInChildren<PlayerShape>().ResetTurnFlags();
        }

        turnsLeftNumber.text = (Int32.Parse(turnsLeftNumber.text) - 1).ToString();

        SetButtons(GameObject.Find("Selected").GetComponentInChildren<PlayerShape>().HasMoved(), 
                   GameObject.Find("Selected").GetComponentInChildren<PlayerShape>().HasInteracted(), 
                   GameObject.Find("Selected").GetComponentInChildren<PlayerShape>().name);

        

        if (turnsLeftNumber.text == "0") {
            menu.SetActive(true);

            moveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            interactButton.GetComponent<Button>().onClick.RemoveAllListeners();
            endButton.GetComponent<Button>().onClick.RemoveAllListeners();
        } else {
            StartCoroutine(ShowMessage("Next turn", 1f));
        }
        
    }

    void OnRestartClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnExitClick() {
        SceneManager.LoadScene("Main Menu");
    }

    void OnInteractHoverEnter() {
        ShowInteractTip();
    }

    void OnInteractHoverExit()  {
        HideInteractTip();
    }

    IEnumerator ShowMessage(string message, float delay) {
        alert.GetComponentInChildren<Text>().text = message;
        alert.SetActive(true);
        yield return new WaitForSeconds(delay);
        alert.SetActive(false);
    }

    /**
	 *  Sets the move button to its default state (colour and flags reset) and
	 * 	unhighlights the move area
	 */
    public void restoreMoveButton() {
		moveButton.image.color = Constants.COLOR_BUTTON_UNCLICKED;
		unhighlightMoveArea ();
		movePressed = false;
	}

    /**
	 *  Sets the interact button to its default state (colour and flags reset), 
	 * 	unhighlights the interacting shapes and hides the interaction tip.
	 */
    public void restoreInteractButton() {
        interactButton.image.color = Constants.COLOR_BUTTON_UNCLICKED;
        HideInteractTip();
        UnhighlightInteractable();
        interactPressed = false;
    }

    /**
	 * Highlights the area where the shape currently selected can move
	 */
    void highlightMoveArea(){
		List<Vector3> lightedArea = new List<Vector3> ();
		List<Vector3> nextStep = new List<Vector3> ();
		List<Vector3> currStep = new List<Vector3> ();

		PlayerShape shape = (PlayerShape)GameObject.Find ("Selected").GetComponentInChildren<PlayerShape> ();

		// First, get the neighbours of the shape's position
		nextStep = Pathing.neighbours (GameObject.Find ("Selected").transform.position);

		// Iterate through found neighbours and save them as long as movement permits it
		for (int i = 0; i < shape.getMovement (); i++) {
			foreach (var tile in nextStep) {
				if (tile != GameObject.Find ("Selected").transform.position && !lightedArea.Contains (new Vector3 (tile.x, tile.y - Constants.UNIT_TILE_DIFF, tile.z))) {
					currStep.AddRange (Pathing.neighbours (new Vector3 (tile.x, tile.y, tile.z)));
				}
			}

			for (int j = 0; j < nextStep.Count; j++) {  // Sets every Y value to the tile level instead of unit level
				nextStep[j] = new Vector3 (nextStep[j].x,nextStep[j].y - Constants.UNIT_TILE_DIFF, nextStep[j].z);
			}

			lightedArea.AddRange (nextStep);	
			nextStep = new List<Vector3>(currStep);
			currStep.Clear ();
		}

		// Highlight the identified tiles which can be moved to
		foreach (var tile in lightedArea) {
			foreach (GameObject current in GameObject.FindGameObjectsWithTag("Tile")) {
				if (tile == current.transform.position) {
					current.GetComponent<Renderer>().material.color = Constants.COLOR_MOVE_AREA;
					moveArea.Add (current.GetComponent<Tile>());
				}
			}
		}
	}

	/**
	 * 	Unhighlights the area where the shape currently selected can move
	 */
	public void unhighlightMoveArea(){
		foreach (var tile in moveArea) {
            if (tile.GetComponent<Tile>().name == "Goal Tile") {
                tile.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_GOAL;
            } else if (tile.GetComponent<Tile>().name == "Pressure Tile") {
                tile.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_PRESSURE;
            } else {
                tile.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_NORMAL;
            }
		}
		moveArea.Clear ();
	}

    /**
     *  Shows the interact tip for the currently selected shape 
     */
    void ShowInteractTip() {
        string nameSelected = GameObject.Find("Selected").transform.GetChild(0).name;
        
        interactTip.SetActive(true);
        interactTip.transform.GetChild(0).gameObject.SetActive(true);

        switch (nameSelected) {
            case "Sphere":
                interactText.text = "The sphere will push a neighbouring shape, moving it up to 3 positions in the direction opposite of the sphere.";
                break;

            case "Cube":
                interactText.text = "The cube will stomp a neighbouring figure, causing different results depending on the stomped shape.";
                break;

            case "Pyramid":
                interactText.text = "The pyramid will fly over the neighbouring figure, and those neighbouring it in a straight line, landing at the nearest space.";
                break;
            case "Plank":
                interactText.text = "The plank will put itself over the neighbouring space, creating a new tile that can be moved to.";
                break;
        }
    }

    /**
     *  Hides the interact tip for the currently selected shape 
     */
    void HideInteractTip() {
        interactTip.SetActive(false);
        interactTip.transform.GetChild(0).gameObject.SetActive(false);
    }

    /**
    * 	Highlights the tiles or spaces which the shape currently selected can interact
    */
    void HighlightInteractable() {
        List<Vector3> lightedArea = new List<Vector3>();

        if (shapeSelected != "Plank") {

            // First, get the neighbouring shapes of the shape's position
            lightedArea = Pathing.NeighbouringUnits(GameObject.Find("Selected").transform.position);

            // Highlight the identified shapes which can be interacted with
            foreach (var tile in lightedArea) {
                foreach (GameObject current in GameObject.FindGameObjectsWithTag("Tile")) {
                    if (tile == current.transform.position) {
                        current.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_AREA;
                        interactTiles.Add(current.GetComponent<Tile>());
                    }
                }
            }
        } else {
            // First, get the neighbouring spaces of the plank's position
            lightedArea = Pathing.NeighbouringSpaces(GameObject.Find("Selected").transform.position);

            // Highlight the identified spaces which can be interacted with
            foreach (var space in lightedArea) {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                tile.transform.position = space;
                tile.AddComponent<Tile>();

                tile.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_AREA;
                interactTiles.Add(tile.GetComponent<Tile>());
            }
        }
    }

    /**
    * 	Unhighlights the tiles which the shape currently selected can interact
    */
    public void UnhighlightInteractable() {
        if (shapeSelected != "Plank") {
            foreach (var tile in interactTiles) {
                tile.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_NORMAL;
            }
        } else {
            foreach (var tile in interactTiles) {
                Destroy(tile.gameObject);
            }
        }

        interactTiles.Clear();
    }

    public void ClearUI() {
        movePressed = false;
        interactPressed = false;
        moveButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
        menu.SetActive(false);
        interactButton.gameObject.SetActive(false);
        interactTip.SetActive(false);
        alert.SetActive(false);
    }
}
