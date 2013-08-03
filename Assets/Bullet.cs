using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	GameObject m_playerObject;
	PlayerNinja m_playerNinja;
	
	// Use this for initialization
	void Start () 
	{
		m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		m_playerNinja = m_playerObject.GetComponent<PlayerNinja>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnTriggerEnter(Collider Hit)
	{
		if(Hit.name == "Ninja")
		{
			m_playerNinja.Die ();
			print ("You got hit!");
		}
	}
}
