using UnityEngine;
using System.Collections;

public class OutOfTurnsScreen : MonoBehaviour {
	
	GameObject buy5More, restart;
	HUD hud;
	bool clickedbuymore;

	// Use this for initialization
	void Start () {
		hud = transform.parent.gameObject.GetComponent<HUD>();
		buy5More = transform.FindChild("Buy5More").gameObject;
		buy5More.guiTexture.texture = null;
		restart = transform.FindChild("Restart").gameObject;
		restart.guiTexture.texture = null;
		gameObject.SetActive (false);
		clickedbuymore = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (clickedbuymore) // Hold off to avoid the game catching the click and firing shuriken as hudstate not solid
		{
			hud.num_turns += 5;
			hud.hudstate = HUD.HudState.PLAY;
			gameObject.SetActive (false);
			clickedbuymore = false;
		}
	}
	
	void OnMouseUp( ) {
		// Note, will only trigger if behind the main screen (as otherwise doesn't get the MouseDown)
		if (buy5More.guiTexture.HitTest (Input.mousePosition)) { 
			clickedbuymore = true;
		}
		else if (restart.guiTexture.HitTest (Input.mousePosition)) {
			hud.GameReset ();
		}
	}
}
