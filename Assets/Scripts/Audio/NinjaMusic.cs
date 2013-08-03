using UnityEngine;
using System.Collections;

public class NinjaMusic : MonoBehaviour {
	
	public EPSound m_IdleMusic;
	public EPSound m_AimingMusic;
	public EPSound m_ActionMusic;
	public EPSound m_ActionEndMusic;
	public float m_IdleMusicLevel;
	public float m_IdleMusicActionLevel;
	public float m_ActionMusicLevel;
	public float m_ActionEndMusicLevel;
	public float m_AimingMusicLevelMin;
	public float m_AimingMusicLevelMax;
	
	float m_AimingVolume;
	
	// Objects used to reference player state
	GameObject playerObject;
	PlayerNinja player;
	PlayerNinja.PlayerNinjaState m_LastState;
	
	
	// Use this for initialization
	void Start ()
	{
		m_IdleMusic.Play( 0.0f, 1.0f );
		m_IdleMusic.Fade( m_IdleMusicLevel, 2.0f, false);
		
		// Get the player state
		playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		player = playerObject.GetComponent<PlayerNinja>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		//Debug.Log("Player State: " + player.m_State);
		//Debug.Log("Last State: " + m_LastState);
		
		if ( player.m_State != m_LastState )
		{
			if ( player.m_State == PlayerNinja.PlayerNinjaState.IDLE )
			{
				if ( m_LastState == PlayerNinja.PlayerNinjaState.DIVING )
				{
					m_ActionEndMusic.Play();
				}
				m_ActionMusic.Fade( 0.0f, 0.5f, true );
				m_IdleMusic.Fade ( m_IdleMusicLevel, 10.0f, false );
			}
			else if ( player.m_State == PlayerNinja.PlayerNinjaState.AIMING )
			{
				m_ActionMusic.Play();
			}
			else if ( player.m_State == PlayerNinja.PlayerNinjaState.DIVING )
			{
				m_ActionMusic.Play();
				m_IdleMusic.Fade ( m_IdleMusicActionLevel, 0.5f, false );
			}
		}
		
		// Set aiming music volume according to Ping Power
		if ( player.m_State == PlayerNinja.PlayerNinjaState.AIMING )
		{
			m_AimingVolume = m_AimingMusicLevelMin + ( ( m_AimingMusicLevelMax - m_AimingMusicLevelMin ) * player.m_CurrentPingPower );
			m_ActionMusic.m_Sources[0].volume = m_AimingVolume;
		}
		
		m_LastState = player.m_State;
	}

}
