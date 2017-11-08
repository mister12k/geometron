﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShape : MonoBehaviour {

    private Vector3 target;
	private List<Vector3> targetPath;

	private int movement;

    // Use this for initialization
    void Start() {
		targetPath = new List<Vector3> ();
		if (this.name.Equals ("Cube")) {
			movement = 2;
		} else {
			movement = 3;
		}
    }

    /**
     * 	On update, the figure unpipes the movements accumulated and executes them according to its animator and
     * 	temporal direction
     */
    void Update() {
		Vector3 tempTarget = new Vector3 ();
		if (this.transform.parent.name.Equals("Selected")) {
			
			if (targetPath.Count > 0) {
				tempTarget = targetPath [0];

				if (this.GetComponent<Animator> ().GetBool ("moving") && Vector3.Distance (transform.parent.position, tempTarget) > 0f) {
					foreach (AnimatorControllerParameter parameter in this.GetComponent<Animator>().parameters) {
						if (!parameter.name.Equals ("moving")) {
							this.GetComponent<Animator> ().SetBool (parameter.name, false);
						}
					}

					if (tempTarget.x > transform.parent.position.x && tempTarget.x - transform.parent.position.x > 0f) {
						this.GetComponent<Animator> ().SetBool ("movingRight", true);                
						transform.parent.position = Vector3.MoveTowards (transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
					} else if (tempTarget.x < transform.parent.position.x && transform.parent.position.x - tempTarget.x > 0f) {
						this.GetComponent<Animator> ().SetBool ("movingLeft", true);
						transform.parent.position = Vector3.MoveTowards (transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
					} else if (tempTarget.z > transform.parent.position.z && tempTarget.z - transform.parent.position.z > 0f) {
						this.GetComponent<Animator> ().SetBool ("movingForward", true);
						transform.parent.position = Vector3.MoveTowards (transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
					} else if (tempTarget.z < transform.parent.position.z && transform.parent.position.z - tempTarget.z > 0f) {
						this.GetComponent<Animator> ().SetBool ("movingBackward", true);
						transform.parent.position = Vector3.MoveTowards (transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
					}

				} else {
					if (transform.parent.position == target) {
						this.GetComponent<Animator> ().SetBool ("moving", false);
					}
					targetPath.RemoveAt (0);
				}
			}
		}
    }

	/**
	 * 	Changes the colour of the shape when moving the mouse into it if not selected previously
	 */ 
    public void OnMouseOver() {
		if (!this.transform.parent.name.Equals ("Selected")) {
			this.GetComponent<Renderer> ().material.color = Constants.HOLO_BLUE;
		}
    }

	/**
	 * 	Changes the colour of the shape when moving the mouse away of it if not selected
	 */ 
    public void OnMouseExit() {
		if (!this.transform.parent.name.Equals ("Selected")) {
			this.GetComponent<Renderer> ().material.color = Color.white;
		}
    }

	/**
	 * 	Changes the colour of the shape after selecting it
	 */ 
	public void OnMouseDown() {
		if (!GameObject.Find ("Selected").GetComponentInChildren<Animator> ().GetBool ("moving")) {
			if (GameObject.Find ("Selected") != null) {
				GameObject.Find ("Selected").transform.GetChild (0).gameObject.GetComponent<Renderer> ().material.color = Color.white;
				GameObject.Find ("Selected").name = "Player";
			}

			this.transform.parent.name = "Selected";
			GameObject.Find ("Main Camera").GetComponent<ButtonManager> ().restoreMoveButton ();
			this.GetComponent<Renderer> ().material.color = Constants.PURPLE;
		}
	}
  
	/**
	 * 	Sets the necessary parameters to start the movement of the shape, along with its animation
	 * 	and pathfinding.
	 */
    public void moveAnimation(Vector3 targetTile) {
        this.GetComponent<Animator>().SetBool("moving", true);
		GameObject.Find ("Main Camera").GetComponent<ButtonManager>().restoreMoveButton();

		target = new Vector3(targetTile.x,targetTile.y + Constants.UNIT_TILE_DIFF,targetTile.z);
		targetPath = Pathing.AStar (this.transform.parent.position, target);
    }


	public int getMovement(){
		return movement;
	}
}