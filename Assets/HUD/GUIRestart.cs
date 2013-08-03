using UnityEngine;
using System.Collections;

public class GUIRestart : MonoBehaviour {
	
	HUD hud;
	
	// Use this for initialization
	void Start () {
		hud = transform.parent.gameObject.GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUp( ) {
		hud.GameReset();
	} 
}
