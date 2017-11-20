using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	private Button moveButton;
    private Button interactButton;

    private bool movePressed;
    private bool interactPressed;

    private GameObject interactTip; 
    private Text interactText;

	private List<Tile> moveArea;
    private List<Tile> interactTiles;

    // Use this for initialization
    void Start () {
		movePressed = false;
        interactPressed = false;
		moveButton = GameObject.Find ("MoveButton").GetComponent<Button>();
		moveButton.GetComponent<Button>().onClick.AddListener(OnMoveClick);
        moveButton.gameObject.SetActive (false);

        interactButton = GameObject.Find("InteractButton").GetComponent<Button>();
        interactButton.GetComponent<Button>().onClick.AddListener(OnInteractClick);
        interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
        interactButton.gameObject.SetActive(false);

        interactText = GameObject.Find("InteractTip").GetComponentInChildren<Text>();
        interactTip = GameObject.Find("InteractTip");
        interactTip.SetActive(false);

        moveArea = new List<Tile>();
        interactTiles = new List<Tile>();
    }

    /**
     *  Method which allows to reset the buttons on the UI depending on the state of the 
     *  booleans given through arguments. It will click or unclick buttons as needed
     *  depending on the current state of the game.
     */
    public void SetButtons(bool hasMoved, bool hasInteracted) {

        Vector3 selectedPosition = new Vector3();
        bool interactableObjects = false;
        moveButton.gameObject.SetActive(true);
        interactButton.gameObject.SetActive(true);

        if (!hasMoved) {
            restoreMoveButton();
        } else {
            SetButtonMoveClicked();
        }


        if (!hasInteracted) {

            interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);

            selectedPosition = GameObject.Find("Selected").transform.position;

            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Unit")) {

                if (g.transform.position == new Vector3(selectedPosition.x + Constants.TILE_GAP, selectedPosition.y, selectedPosition.z)) {
                    interactableObjects = true;
                }

                if (g.transform.position == new Vector3(selectedPosition.x - Constants.TILE_GAP, selectedPosition.y, selectedPosition.z)) {
                    interactableObjects = true;
                }

                if (g.transform.position == new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z + Constants.TILE_GAP)) {
                    interactableObjects = true;
                }

                if (g.transform.position == new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z - Constants.TILE_GAP)) {
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
        moveButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
        movePressed = true;
        unhighlightMoveArea();
    }

    public void SetButtonInteractClicked() {
        interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
        interactPressed = true;
        HideTip();
        UnhighlightInteractable();
    }

    /**
	 * 	Listener active when the move button is pressed, sets the button's state
	 * 	and if it highlights or not the available move area for the currently selected shape
	 */
    void OnMoveClick(){
        if (!(moveButton.image.color == new Color32(0xC8, 0xC8, 0xC8, 0xFF) && movePressed)) {
            restoreInteractButton();
            movePressed = !movePressed;
            if (movePressed) {
                moveButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
                highlightMoveArea();
            } else {
                moveButton.image.color = Color.white;
                unhighlightMoveArea();
            }
        }
	}

    /**
	 * 	Listener active when the interact button is pressed, sets the button's state
     * 	and if it highlights or not the interactable shapes and hides/shows the interaction tip.
	 */
    void OnInteractClick() {
        if (!(interactButton.image.color == new Color32(0xC8, 0xC8, 0xC8, 0xFF) && interactPressed)) {
            restoreMoveButton();
            interactPressed = !interactPressed;
            if (interactPressed) {
                interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
                ShowInteractTip();
                HighlightInteractable();
            } else {
                interactButton.image.color = Color.white;
                HideTip();
                UnhighlightInteractable();
            }
        }
    }

	/**
	 *  Sets the move button to its default state (colour and flags reset) and
	 * 	unhighlights the move area
	 */ 
	public void restoreMoveButton() {
		moveButton.image.color = Color.white;
		unhighlightMoveArea ();
		movePressed = false;
	}

    /**
	 *  Sets the interact button to its default state (colour and flags reset), 
	 * 	unhighlights the interacting shapes and hides the interaction tip.
	 */
    public void restoreInteractButton() {
        interactButton.image.color = Color.white;
        HideTip();
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
			tile.GetComponent<Renderer> ().material.color = Color.white;
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
                interactText.text = "The cube will stomp a neighbouring figure, causing unexpected results depending on the stomped shape.";
                break;

            case "Pyramid":
                interactText.text = "The pyramid will fly over the neighbouring figure, and those neighbouring it in a straight line, landing at the nearest space.";
                break;
        }
    }

    /**
     *  Hides the interact tip for the currently selected shape 
     */
    void HideTip() {
        interactTip.SetActive(false);
        interactTip.transform.GetChild(0).gameObject.SetActive(false);
    }

    /**
    * 	Highlights the tiles which the shape currently selected can interact
    */
    void HighlightInteractable() {
        List<Vector3> lightedArea = new List<Vector3>();

        // First, get the neighbouring shapes of the shape's position
        lightedArea = Pathing.NeighbouringUnits(GameObject.Find("Selected").transform.position);

        // Highlight the identified tiles which can be interacted with
        foreach (var tile in lightedArea) {
            foreach (GameObject current in GameObject.FindGameObjectsWithTag("Tile")) {
                if (tile == current.transform.position) {
                    current.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_AREA;
                    interactTiles.Add(current.GetComponent<Tile>());
                }
            }
        }
    }

    /**
    * 	Unhighlights the tiles which the shape currently selected can interact
    */
    public void UnhighlightInteractable() {
        foreach (var tile in interactTiles) {
            tile.GetComponent<Renderer>().material.color = Color.white;
        }
        interactTiles.Clear();
    }
}
