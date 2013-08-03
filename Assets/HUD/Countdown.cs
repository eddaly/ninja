using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	
	bool started = false;
	public float timer;
	public Texture texture;
	
	// Use this for initialization
	void Start () {
		started = false;
		timer = 3;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
	}
	
	public void Begin () {started = true; timer = 3;}
	public bool IsFinished () {return started && timer <= 0;}
	
	void OnGUI ()
	{
		if (!started)
			return;
		
		Vector3 v = transform.parent.position;
		Vector3 v2 = Camera.main.WorldToScreenPoint (v);
		//v2.z = 0; // Will sort beneath screen overlays (if using GUITexture but that doesn't do UV anim and this does screen coords so can't sort!)
		float size = 30;
		Rect rect = new Rect (v2.x, Screen.height - v2.y, size, size);
		
		if (timer > 2)
		{
			GUI.DrawTextureWithTexCoords (rect, texture, new Rect (0, 0, .33f, 1f));
		}
		else if(timer > 1)
		{
			GUI.DrawTextureWithTexCoords (rect, texture, new Rect (.33f, 0, .33f, 1f));
		}
		else
		{
			GUI.DrawTextureWithTexCoords (rect, texture, new Rect (.66f, 0, .33f, 1f));
		}
	}
}
