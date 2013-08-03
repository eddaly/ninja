using UnityEngine;
using System.Collections;

public class ShurikenPickup : MonoBehaviour {
	
	public int m_Num_Shuriken = 5;
	HUD hud;

	// Use this for initialization
	void Start () {
		GameObject playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		PlayerNinja playerNinja = playerObject.GetComponent<PlayerNinja>();
		hud = playerNinja.hud;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.name == "Ninja" )
		{
			hud.AddShuriken (m_Num_Shuriken);
			Destroy( this.gameObject );
		}
	}
}
