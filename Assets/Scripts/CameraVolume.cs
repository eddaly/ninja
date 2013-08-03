using UnityEngine;
using System.Collections;

public class CameraVolume : MonoBehaviour {
	
	public Vector3 cameraPosition;
	Vector3 prev_cameraPosition;
	
	// Use this for initialization
	void Start () {
		gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	//*** Hardly optimal, better just to test player in update etc.
	void OnTriggerEnter (Collider other) {
		if (other.name == "Ninja" )
		{
			prev_cameraPosition = Camera.main.transform.position;
			Camera.main.transform.position = cameraPosition;
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.name == "Ninja" )
		{
			Camera.main.transform.position = prev_cameraPosition;
		}
	}
}
