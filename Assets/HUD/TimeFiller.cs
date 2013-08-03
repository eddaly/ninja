using UnityEngine;
using System.Collections;

public class TimeFiller : MonoBehaviour {
	
	public float time, flash_when;
	float orig_time;
	Color orig_color;
	public AudioClip timeout;
	bool paused;
	//GameObject m_playerObject;
	//PlayerNinja m_player;

	
	// Use this for initialization
	void Start () {
		//m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		//m_player = m_playerObject.GetComponent<PlayerNinja>();

		orig_time = time;
		orig_color = renderer.material.color;
		paused = true;
	}

	void Reset () {
		orig_time = time;
		orig_color = renderer.material.color;
		paused = true;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (paused)
			return;
		
		float t = (time / orig_time);
		
		if (t < 0)
			t = 0;
		
		if (t < flash_when)
		{
			if (time - Mathf.FloorToInt (time) < .5)
				renderer.material.color = Color.black;
			else
				renderer.material.color = orig_color;
		}
		
		time -= Time.deltaTime;
		
		Vector3 s = new Vector3 (1, 1, t);
		transform.localScale = s;
		Vector3 p = new Vector3 (transform.localPosition.x, transform.localPosition.y, ((1-t) * -5f));
		transform.localPosition = p;
		
		if (time <= 0)
		{
			audio.PlayOneShot (timeout);
			//print ("OUT OF TIME");
			//m_player.hud.OutOfTime ();
			paused = true;
		}
	}
	
	public void Pause() {paused = true;}
	public void Go () {paused = false;}
}
