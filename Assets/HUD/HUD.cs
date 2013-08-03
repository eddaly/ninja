using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	
	public enum HudState {START = 0, PLAY, PLACINGGADGETS, LEVELCOMPLETE, OUTOFTURNS, STOREINLEVEL, STOREPOSTLEVEL, DIED};
	[HideInInspector]
	public HudState hudstate;
	int maxbounces = -1;
	//TurnsCounter turnsCounter;
	GameObject coinsCounter, killsCounter, shurikenCounter;
	private GameObject levelEnd;
	[HideInInspector]
	public PlayerController playerController;
	private int num_coins, num_kills;
	[HideInInspector]
	public int num_shuriken = 5;
	public LevelParams m_LevelParams;
	[HideInInspector]
	public PlayerNinja player_ninja = null;
	[HideInInspector]
	public int num_turns;
	private bool startclicked = false, storeclicked = false;
	
	GameObject m_OutOfTurnsScreen;

	// Use this for initialization
	void Start () {
		GameObject m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		if (m_playerObject != null)
			player_ninja = m_playerObject.GetComponent<PlayerNinja>();
		
		GameObject playerGroup = GameObject.Find( "Player" );
		if( playerGroup != null )
		{
			Transform controllerObject = playerGroup.transform.FindChild( "PlayerController" );
			if( controllerObject != null )
			{
				playerController = controllerObject.GetComponent<PlayerController>();
			}
		}

		if (m_LevelParams == null)
		{
			Debug.LogError ("levelParams must be set");
		}
		num_turns = m_LevelParams.BronzeMaxTurns;

		hudstate = HudState.START;
		//hudstate = HudState.PLACINGGADGETS;
		maxbounces = -1;
		//turnsCounter = GetComponentInChildren<TurnsCounter>();
		coinsCounter = transform.FindChild("CoinsCounter").gameObject;
		killsCounter = transform.FindChild("KillsCounter").gameObject;
		shurikenCounter = transform.FindChild("ShurikenCounter").gameObject;
		levelEnd = transform.FindChild("LevelEnd").gameObject;
		num_coins = num_kills = 0;
		num_shuriken = m_LevelParams.extraShuriken;
		shurikenCounter.guiText.text = ""+(num_shuriken);
		startclicked = storeclicked = false;
		
		m_OutOfTurnsScreen = transform.FindChild("OutOfTurnsScreen").gameObject;
	}
	
	void Reset () {
		hudstate = HudState.PLAY;
		maxbounces = -1;
		num_coins = num_kills = 0;
		startclicked = storeclicked = false;
	}
	
	public void SetMaxBounces (int mb) {maxbounces = mb;}
	public int GetMaxBounces () {return maxbounces;}
				
	void OnGUI ()
	{ 
		Texture tex = null;
		float x = Screen.width/2*1.055f;
		float y = 0;
		
		switch (hudstate)
		{
		case HudState.LEVELCOMPLETE:
			if (num_turns <= m_LevelParams.GoldMaxTurns)
			{
				tex = levelEnd.transform.FindChild ("GOLD").guiTexture.texture;
				y = Screen.height/2*.71f;
			}
			else if (num_turns <= m_LevelParams.SilverMaxTurns)
			{
				tex = levelEnd.transform.FindChild ("SILVER").guiTexture.texture;
				y = Screen.height/2*.85f;
			}
			else
			{
				tex = levelEnd.transform.FindChild ("BRONZE").guiTexture.texture;
				y = Screen.height/2*.99f;
			}
			break;
		case HudState.STOREINLEVEL:
		case HudState.STOREPOSTLEVEL:
			tex = TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.STORE);
			break;
		}
		float w = Screen.height;
		float h = w / 2;
		switch (hudstate)
		{
		case HudState.START:
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), 
				TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.START),  ScaleMode.ScaleAndCrop);
			break;
		case HudState.OUTOFTURNS:
			//GUI.DrawTexture (new Rect (Screen.width/2 - w/2, Screen.height/2 - h/2, w, h), 
			//	TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.OUTOFTURNS));
			break;
		case HudState.LEVELCOMPLETE:
			GUI.DrawTexture (new Rect (Screen.width/2 - Screen.height/2, Screen.height*.1f, Screen.height, Screen.height*.8f), 
				TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.LEVELCOMPLETE));
			GUI.DrawTexture (new Rect (x, y, 25, 25), tex);
			break;			
		case HudState.STOREINLEVEL:
		case HudState.STOREPOSTLEVEL:
			GUI.DrawTexture (new Rect (Screen.width/2 - Screen.height/2, Screen.height*.1f, Screen.height, Screen.height*.8f), tex);
			break;
		case HudState.DIED:
			GUI.DrawTexture (new Rect (Screen.width/2 - w/2, Screen.height/2 - h/2, w, h), 
				TextureLibraryInGame.GetTexture (TextureLibraryInGame.Tex.DIED));
			break;
		}	
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (hudstate)
		{
		case HudState.START:
			if (startclicked) {	// Because called before other Updates which process the click
				hudstate = HudState.PLAY;
				startclicked = false;
			}
			if (Input.GetMouseButtonUp(0))
				startclicked = true;
			break;
		case HudState.OUTOFTURNS:
			m_OutOfTurnsScreen.SetActive (true);
			break;
		case HudState.STOREINLEVEL:
			if (storeclicked) {
				hudstate = HudState.PLAY;
				storeclicked = false;
			}
			if (Input.GetMouseButtonUp(0))
				storeclicked = true;
			break;
		case HudState.LEVELCOMPLETE:
			if (Input.GetMouseButtonUp(0))
				hudstate = HudState.STOREPOSTLEVEL;
			break;
		case HudState.STOREPOSTLEVEL:	
			if (Input.GetMouseButton(0))
			{
				hudstate = HudState.PLAY;
				GameReset ();
			}
			break;
		case HudState.DIED:	
			if (Input.GetMouseButton(0))
			{
				hudstate = HudState.PLAY;
				GameReset ();
			}
			break;
		}	
	}
	
	public void ExitReached ()
	{
		hudstate = HudState.LEVELCOMPLETE;
	}
	
	public void Died ()
	{
		hudstate = HudState.DIED;
	}
	
	public void AddCoin ()
	{
		coinsCounter.guiText.text = ""+(++num_coins);
	}

	public void UsedShuriken ()
	{
		shurikenCounter.guiText.text = ""+(--num_shuriken);
	}

	public void AddShuriken (int n)
	{
		num_shuriken += n;
		shurikenCounter.guiText.text = ""+(num_shuriken);
	}

	public void AddKill ()
	{
		killsCounter.guiText.text = ""+(++num_kills);
	}
	
	public void TurnComplete ()
	{
		if (--num_turns == 0)
		{
			hudstate = HudState.OUTOFTURNS;
		}
	}
	
	public bool HUDControlOnly ()
	{
		return !(hudstate == HudState.PLAY);
	}
	
	public void GameReset ()
	{
		Shuriken[] shurikens = FindObjectsOfType (typeof (Shuriken)) as Shuriken[];
		foreach (Shuriken s in shurikens)
		{
			PrimitiveLibrary.Get.ReleaseQuad(s.m_ShurikenQuad );
			Destroy (s);
		}
		
		Application.LoadLevel (Application.loadedLevelName);
	}
}
