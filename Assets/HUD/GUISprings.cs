using UnityEngine;
using System.Collections;

public class GUISprings : MonoBehaviour {
	
	PlayerNinja player_ninja = null;
	
	// Use this for initialization
	void Start () {
		GameObject m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		if (m_playerObject != null)
			player_ninja = m_playerObject.GetComponent<PlayerNinja>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player_ninja == null || player_ninja.m_Aimarrow == null)
			return;
		guiTexture.enabled = false;
		
		/*if (player_ninja.m_State == PlayerNinja.PlayerNinjaState.AIMING && player_ninja.hud.GetMaxBounces () != -1)
		{
			if (player_ninja.hud.GetMaxBounces () == 1)
				this.guiTexture.texture = TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.SPRING1);			
			else if (player_ninja.hud.GetMaxBounces () == 2)
				this.guiTexture.texture = TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.SPRING2);
			else if (player_ninja.hud.GetMaxBounces () == 3)
				this.guiTexture.texture = TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.SPRING3);
			else
				Debug.Log ("error");
			
			Vector3 v = Vector3.Cross (player_ninja.m_Aimarrow.transform.forward, Vector3.up);
			v *= 50;
			v += player_ninja.transform.position;
			transform.position = Camera.main.WorldToViewportPoint (v);
			guiTexture.enabled = true;
		}*/
	}
}
