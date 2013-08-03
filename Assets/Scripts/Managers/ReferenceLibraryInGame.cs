// File: ReferenceLibraryInGame.cs
//
// Desc:	Holder for commonly used object references
//
// Copyright Team WGC 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceLibraryInGame : MonoBehaviour 
{
	public enum Ref
	{
		PLAYER_CONTROLLER,
		PLAYER_NINJA,
		NINJA,
		
		ENEMY_LOOKOUT
	}
	
	public GameObject m_PlayerController;
	public GameObject m_PlayerNinja;
	public GameObject m_Ninja;
	public List<GameObject> m_EnemyLookoutList;
	
	static ReferenceLibraryInGame m_referenceLibrary = null;
	
	
	void Start()
	{
		GameObject playerGroup = GameObject.Find( "Enemies" );
		
		if( playerGroup != null )
		{
			EnemyLookout[] enemyArray = playerGroup.GetComponentsInChildren<EnemyLookout>();
			
			m_EnemyLookoutList.Clear();
			
			for( int e = 0; e<enemyArray.GetLength(0); e++ )
			{
				m_EnemyLookoutList.Add( enemyArray[e].gameObject );
			}
		}
		else
			Debug.LogError( "Can't find Enemies GameObject group" );
	}
	
	//-----------------------------------------------------------------------------
	// Method:	GetReference()
	// Desc:	Returns the specified reference.
	static public GameObject GetReference( Ref reference )
	{
		return GetReference ( reference, 0 );
	}
	static public GameObject GetReference( Ref reference, int ref_index )
	{
		if( m_referenceLibrary == null )
		{
			GameObject libraryObject = GameObject.Find("ReferenceLibrary");
	        if( libraryObject == null )
	        {
	            Debug.Log( "**! No In-game reference library object found (ReferenceLibrary)" );
				return null;
	        }
	        else
	        {
	            m_referenceLibrary = (ReferenceLibraryInGame)libraryObject.GetComponent( typeof(ReferenceLibraryInGame) );
	        }
			
			if( !m_referenceLibrary )
				return null;
		}
		
		GameObject returnReference = null;
		
		switch( reference )
		{
		default:
		case ReferenceLibraryInGame.Ref.PLAYER_CONTROLLER:
			returnReference = m_referenceLibrary.m_PlayerController;
			break;
		case ReferenceLibraryInGame.Ref.PLAYER_NINJA:
			returnReference = m_referenceLibrary.m_PlayerNinja;
			break;
		case ReferenceLibraryInGame.Ref.NINJA:
			returnReference = m_referenceLibrary.m_Ninja;
			break;
		case ReferenceLibraryInGame.Ref.ENEMY_LOOKOUT:
			returnReference = m_referenceLibrary.m_EnemyLookoutList[ref_index];
			break;
		}			
			
		return returnReference;
	}
	
	static public int GetReferenceListLength( Ref reference )
	{
		if( m_referenceLibrary == null )
		{
			GameObject libraryObject = GameObject.Find("ReferenceLibrary");
	        if( libraryObject == null )
	        {
	            Debug.Log( "**! No In-game reference library object found (ReferenceLibrary)" );
				return 0;
	        }
	        else
	        {
	            m_referenceLibrary = (ReferenceLibraryInGame)libraryObject.GetComponent( typeof(ReferenceLibraryInGame) );
	        }
			
			if( !m_referenceLibrary )
				return 0;
		}
		
		int returnCount = 0;
		
		switch( reference )
		{
		default:
		case ReferenceLibraryInGame.Ref.PLAYER_CONTROLLER:
			returnCount = 1;
			break;
		case ReferenceLibraryInGame.Ref.PLAYER_NINJA:
			returnCount = 1;
			break;
		case ReferenceLibraryInGame.Ref.NINJA:
			returnCount = 1;
			break;
		case ReferenceLibraryInGame.Ref.ENEMY_LOOKOUT:
			returnCount = m_referenceLibrary.m_EnemyLookoutList.Count;
			break;
		}			
			
		return returnCount;
	}
}
