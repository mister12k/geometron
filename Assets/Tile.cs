using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    private GameObject underPlane;
	private 

    // Use this for initialization
    void Start () {

        this.name = "Tile";
		this.tag = "Tile";
        transform.localScale = new Vector3(Constants.TILE_WIDTH, 1, Constants.TILE_WIDTH);
        underPlane  = GameObject.CreatePrimitive(PrimitiveType.Plane);
        underPlane.name = "Under";
        underPlane.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.01f, this.transform.position.z);
        underPlane.transform.localScale = new Vector3(Constants.TILE_WIDTH + 0.005f, 1 , Constants.TILE_WIDTH + 0.005f);
        underPlane.GetComponent<Renderer>().material = Resources.Load("Materials/Black", typeof(Material)) as Material;
        underPlane.hideFlags = HideFlags.HideInHierarchy;
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnMouseOver() {
        if (this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_AREA || this.GetComponent<Renderer>().material.color == Constants.COLOR_MOVE_OVER) { // Move area highlighted
            this.GetComponent<Renderer>().material.color = Constants.COLOR_MOVE_OVER;
        } else if (this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_AREA || this.GetComponent<Renderer>().material.color == Constants.COLOR_INTERACT_OVER) { //Interact tiles highlighted
            this.GetComponent<Renderer>().material.color = Constants.COLOR_INTERACT_OVER;
        } else {
            this.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_OVER;
        }
        
		if (Input.GetMouseButtonDown(0) && this.GetComponent<Renderer> ().material.color == Constants.COLOR_MOVE_OVER) {
			if (GameObject.Find ("Selected") != null) {
				GameObject.Find ("Selected").transform.GetChild (0).GetComponent<PlayerShape> ().moveAnimation (transform.position);
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
            this.GetComponent<Renderer>().material.color = Color.white;
        }
    }

}
