using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float cameraSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton (1)) {
			float rotX = Input.GetAxis("Mouse X") * cameraSpeed;
            
			transform.RotateAround(Vector3.zero,Vector3.up,rotX);
		}

        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if (zoom > 0f) {
            transform.Translate(Vector3.forward * Time.deltaTime * cameraSpeed * 20f);
        } else if(zoom < 0f) {
            transform.Translate(Vector3.back * Time.deltaTime * cameraSpeed * 20f);
        }


        if (Input.GetKey (KeyCode.W)) {
			transform.Translate (new Vector3 (0,1,1) * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.S)) {
			transform.Translate (new Vector3(0,-1,-1) * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Vector3.left * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Vector3.right * Time.deltaTime * cameraSpeed);
		}
	}
}
