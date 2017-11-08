using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	private Button moveButton;

	private bool movePressed;

	private List<Tile> moveArea;

	// Use this for initialization
	void Start () {
		movePressed = false;
		moveButton = GameObject.Find ("MoveButton").GetComponent<Button>();
		moveButton.GetComponent<Button>().onClick.AddListener(onMoveClick);
		moveButton.gameObject.SetActive (false);
		moveArea = new List<Tile>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.Find ("Selected") != null) {
			moveButton.gameObject.SetActive (true);
		}
	}

	/**
	 * 	Listener active when the move button is pressed, sets the button's state
	 * 	and if it highlights or not the available move area for the currently selected shape
	 */
	void onMoveClick(){
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
	 *  Sets the move button to its default state (colour and flags reset) and
	 * 	unhighlights the move area
	 */ 
	public void restoreMoveButton(){
		moveButton.image.color = Color.white;
		unhighlightMoveArea ();
		movePressed = false;
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

}
