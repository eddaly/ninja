using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour 
{	
	GameObject m_playerObject;
	PlayerNinja m_player;
	
	// Use this for initialization
	void Start () {
		m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		m_player = m_playerObject.GetComponent<PlayerNinja>();
	}
	void OnTriggerEnter (Collider penetrator) 
	{
   		if(penetrator.name == "Ninja")
		{
			EnemyLookout[] enemies = FindObjectsOfType(typeof(EnemyLookout)) as EnemyLookout[];
        	foreach (EnemyLookout enemy in enemies)
			{
				if( enemy.m_hasSeenPlayer && enemy.m_State != EnemyLookout.EnemyLookoutState.DEAD)
				{
					return;
				}
			}
			m_player.ExitReached ();
		}
	}
}
