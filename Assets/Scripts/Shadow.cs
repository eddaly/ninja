using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour {
	
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
	
	void OnTriggerEnter (Collider other) {
		
		//m_player.hud.timefiller.Pause ();
		m_player.inshadow = true;
	}
	
	void OnTriggerExit (Collider other) {
		
		//m_player.hud.timefiller.Go ();
		m_player.inshadow = false;
	}
}
