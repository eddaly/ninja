using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	
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
	
	void OnTriggerEnter (Collider other)
	{
		if (other.name == "Ninja")
		{
			EPSoundController.Get().Play("CoinPickup");
			hud.AddCoin ();
			Destroy( this.gameObject );
		}
	}
}
