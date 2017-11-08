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

		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Vector3.forward * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Vector3.back * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Vector3.left * Time.deltaTime * cameraSpeed);
		} else if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Vector3.right * Time.deltaTime * cameraSpeed);
		}
	}
}
