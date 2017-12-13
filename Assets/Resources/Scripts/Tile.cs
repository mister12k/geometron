using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    private GameObject underPlane;
    private bool isPressed;
    private bool isGoalActive;

    // Use this for initialization
    void Start() {
        isGoalActive = false;

        if (name != "Goal Tile" && name != "Pressure Tile") {
            this.name = "Tile";
        }

        if (name == "Pressure Tile") {
            isPressed = false;
        } else {
            isPressed = true;
        }

        this.tag = "Tile";
        transform.localScale = new Vector3(Constants.TILE_WIDTH, 1, Constants.TILE_WIDTH);
        underPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        underPlane.name = "Under";
        underPlane.transform.parent = this.transform;
        underPlane.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.01f, this.transform.position.z);
        underPlane.transform.localScale = new Vector3(1 + 0.005f, 1, 1 + 0.005f);
        underPlane.GetComponent<Renderer>().material = Resources.Load("Materials/Black", typeof(Material)) as Material;
        underPlane.hideFlags = HideFlags.HideInHierarchy;
    }

    // Update is called once per frame
    void Update() {
        bool goalOpened = true;

        if (name == "Goal Tile") {
            foreach (var tile in GameObject.FindGameObjectsWithTag("Tile")) {
                if (!tile.GetComponent<Tile>().GetPressed()) {
                    goalOpened = false;
                }
            }
            if (goalOpened) {
                if(this.GetComponent<Renderer>().material.color != Constants.COLOR_TILE_GOAL_ACTIVE && !isGoalActive) {
                    this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_GOAL_ACTIVE;
                    isGoalActive = true;
                }
                foreach (var unit in GameObject.FindGameObjectsWithTag("Unit")) {
                    if (unit.transform.position == new Vector3(transform.position.x, transform.position.y + Constants.UNIT_TILE_DIFF, transform.position.z)) {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }
            }
        } else if (name == "Pressure Tile") {
            foreach (var unit in GameObject.FindGameObjectsWithTag("Unit")) {
                if (unit.transform.position == new Vector3(transform.position.x, transform.position.y + Constants.UNIT_TILE_DIFF, transform.position.z)) {
                    isPressed = true;
                }
            }
        }

    }

    private void OnMouseOver() {
        if (this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_AREA || this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_OVER) { // Move area highlighted
            this.GetComponent<Renderer>().material.color = Constants.COLOR_MOVE_OVER;
        } else if (this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_AREA || this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_OVER) { //Interact tiles highlighted
            this.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_OVER;
        } else {
            this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_OVER;
        }

        if (Input.GetMouseButtonDown(0) && this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_OVER) {
            if (GameObject.Find("Selected") != null) {
                GameObject.Find("Selected").transform.GetChild(0).GetComponent<PlayerShape>().moveAnimation(transform.position);
            }
        }

        if (Input.GetMouseButtonDown(0) && this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_OVER) {
            if (GameObject.Find("Selected") != null) {
                GameObject.Find("Selected").transform.GetChild(0).GetComponent<PlayerShape>().InteractAnimation(transform.position);
            }
        }
    }

    private void OnMouseExit() {

        if (this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_OVER || this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_AREA) {
            this.GetComponent<Renderer>().material.color = Constants.COLOR_MOVE_AREA;
        } else if (this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_AREA || this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_OVER) {
            this.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_AREA;
        } else {
            if (name == "Goal Tile" && !isGoalActive) {
                this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_GOAL;
            } else if (name == "Goal Tile" && isGoalActive) {
                this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_GOAL_ACTIVE;
            } else if (name == "Pressure Tile") {
                this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_PRESSURE;
            } else {
                this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_NORMAL;
            }
        }
    }

    public bool GetPressed() {
        return isPressed;
    }

}
