using UnityEngine;
using System.Collections;

public class Lasergate : MonoBehaviour {

	public float blinkduration = 1;
	float currentblink_t;
	bool on;
	GameObject m_playerObject;
	PlayerNinja m_player;
	
	// Use this for initialization
	void Start () {
		m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		m_player = m_playerObject.GetComponent<PlayerNinja>();
		currentblink_t = 0;
		on = true;
		renderer.enabled = true;
	}
	
	void Reset ()
	{
		currentblink_t = 0;
		on = true;
		renderer.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if ((currentblink_t += Time.deltaTime) >= blinkduration)
		{
			if (on)
			{
				renderer.enabled = false;
				GetComponent<BoxCollider>().enabled = false;
				on = false;
			}
			else
			{
				renderer.enabled = true;
				GetComponent<BoxCollider>().enabled = true;
				on = true;
			}
			currentblink_t = 0;
		}
	}
	
	void OnTriggerStay (Collider other) {
		if (on && other.name == "Ninja" )
		{
			m_player.Die();//  hud.GameReset ();
			print ("burn!!!"); // and do whatever else
		}
	}
}
