using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	private Button moveButton;
    private Button interactButton;

    private bool movePressed;
    private bool interactPressed;

    private GameObject interactTip; 
    private Text interactText;

	private List<Tile> moveArea;

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

        interactText = GameObject.Find("InteractTip").GetComponent<Text>();
        interactTip = GameObject.Find("InteractTip");
        interactText.gameObject.SetActive(false);
        interactTip.SetActive(false);

        moveArea = new List<Tile>();
	}
	
	// Update is called once per frame
	void Update () {
       
    }

    public void SetButtons() {

        Vector3 selectedPosition = new Vector3();
        bool interactableObjects = false;

        moveButton.gameObject.SetActive(true);
        restoreMoveButton();
        interactButton.gameObject.SetActive(true);
        restoreInteractButton();

        selectedPosition = GameObject.Find("Selected").transform.position;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Unit")) {

            if (g.transform.position == new Vector3(selectedPosition.x + Constants.TILE_GAP, selectedPosition.y, selectedPosition.z)) {
                interactButton.image.color = Color.white;
                interactableObjects = true;
            }

            if (g.transform.position == new Vector3(selectedPosition.x - Constants.TILE_GAP, selectedPosition.y, selectedPosition.z)) {
                interactButton.image.color = Color.white;
                interactableObjects = true;
            }

            if (g.transform.position == new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z + Constants.TILE_GAP)) {
                interactButton.image.color = Color.white;
                interactableObjects = true;
            }

            if (g.transform.position == new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z - Constants.TILE_GAP)) {
                interactButton.image.color = Color.white;
                interactableObjects = true;
            }
        }

        if (!interactableObjects) {
            interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
        }

    }

	/**
	 * 	Listener active when the move button is pressed, sets the button's state
	 * 	and if it highlights or not the available move area for the currently selected shape
	 */
	void OnMoveClick(){
		movePressed = !movePressed;
		if (movePressed) {
			moveButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
			highlightMoveArea ();
		} else {
			moveButton.image.color = Color.white;
			unhighlightMoveArea ();
		}
	}

    /**
	 * 	Listener active when the interact button is pressed, sets the button's state
	 */
    void OnInteractClick() {
        if (!(interactButton.image.color == new Color32(0xC8, 0xC8, 0xC8, 0xFF) && !interactPressed)) {
            interactPressed = !interactPressed;
            if (interactPressed) {
                interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
                ShowInteractTip();
            } else {
                interactButton.image.color = Color.white;
                //unhighlightMoveArea();
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

    public void restoreInteractButton() {
        interactButton.image.color = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
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
					current.GetComponent<Renderer>().material.color = new Color32(0x00, 0x00, 0x80, 0xFF);
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

    void ShowInteractTip() {
        string nameSelected = GameObject.Find("Selected").transform.GetChild(0).name;

        interactText.gameObject.SetActive(true);
        interactTip.SetActive(true);

        switch (nameSelected) {
            case "Sphere":
                interactText.text = "The sphere will push a neighbouring shape, moving it up to 3 positions in the direction opposite of the sphere.";
                break;

            case "Cube":
                interactText.text = "";
                break;
        }
    }
}
