using UnityEngine;
using System.Collections;

public class Pit : MonoBehaviour {
	
	GameObject m_playerObject;
	PlayerNinja m_player;

	// Use this for initialization
	void Start () {
		m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		m_player = m_playerObject.GetComponent<PlayerNinja>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay (Collider other) {
		if (other.name == "Ninja" )
		{
			m_player.Die();//  hud.GameReset ();
			print ("fall in pit!!!"); // and do whatever else
		}
	}
}
