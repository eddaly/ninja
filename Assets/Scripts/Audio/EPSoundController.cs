//-----------------------------------------------------------------------------
// File: EPSoundController.cs
//
// Desc:	Class to control playback of EPSoundSequence and EPSound objects
//
// Copyright Echo Peak Ltd 2012
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EPSoundController : MonoBehaviour 
{
	public bool m_PopulateList = false;
	
	public List<EPSoundEvent> m_EPSoundEventList = new List<EPSoundEvent>();
	public List<string> m_EPSoundNames = new List<string>();

	[HideInInspector]
	public float m_GlobalSFXVolume = 1.0f;
	[HideInInspector]
	public float m_GlobalMusicVolume = 1.0f;
	[HideInInspector]
	public float m_SFXVolMaster = 1.0f;
	[HideInInspector]
	public float m_MusicVolMaster = 0.6f;
	[HideInInspector]
	public float m_DuckingAmount = 1.0f;
	
	//	Pseudo-singleton pattern
	private static EPSoundController ms_soundController = null;
    public static EPSoundController Get()
    {
		//	Cache the object to avoid frequent string lookups
		if( ms_soundController != null )
			return ms_soundController;

		GameObject soundControllerObject = GameObject.Find( "SoundController" );
        if( soundControllerObject == null )
        {
            Debug.Log( "!** No sound controller object found (SoundController)" );
        }
        else
        {
            ms_soundController = (EPSoundController)soundControllerObject.GetComponent( typeof(EPSoundController) );
        }
		
		if( ms_soundController != null )
	        return ms_soundController;
		
		Debug.Log( "!** Couldn't get EPSoundController component" );
		return null;
    }
	
	// Use this for initialization
	void Start()
	{
		m_DuckingAmount = 1.0f;

		populateLists();
		
		m_GlobalSFXVolume = m_SFXVolMaster;
		m_GlobalMusicVolume = m_MusicVolMaster;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_PopulateList )
		{
			populateLists();			
			m_PopulateList = false;
		}


		/*// Play sound with spacebar
		if (Input.GetKeyUp(KeyCode.Space))
		{
			Play("Motion_special_rise_01");
		}//*/
		
		/*// Play sound with return
		if (Input.GetKeyUp(KeyCode.Return))
		{
			Play("Motion_long_super_01");
		}//*/
		
		/*
		// Pause sounds
		if (Input.GetKeyUp (KeyCode.P))
			Pause();
		
		// Resume sounds
		if (Input.GetKeyUp (KeyCode.R))
			Resume();
		*/
	}
	
	// Play sound by name
	public void Play( string sound_name )
	{
		int soundIdx = GetIndex ( sound_name );
		Play ( soundIdx );
	}
	
	public void Play( string sound_name, float volume, float pitch )
	{
		int soundIdx = GetIndex( sound_name );
		Play(soundIdx, volume, pitch);
		Debug.Log("Play sound: " + sound_name);
	}
	
	// Play sound by list ID
	public void Play( int sound_idx )
	{
		m_EPSoundEventList[sound_idx].Play ();
	}
	
	public void Play( int sound_idx, float volume, float pitch )
	{
		m_EPSoundEventList[sound_idx].Play( volume, pitch );
	}
	
	// Play sound by object reference
	public void Play( EPSoundEvent sound )
	{
		sound.Play( 1.0f, 1.0f );
	}
	
	public void Play( EPSoundEvent sound, float volume, float pitch )
	{
		sound.Play( volume, pitch );
	}
	
	public void StopAll()
	{	
		foreach (EPSoundEvent sound in m_EPSoundEventList)
		{
			sound.Stop();
		}
	}
	
	public void Pause()
	{
		foreach (EPSoundEvent sound in m_EPSoundEventList)
		{
			sound.Pause();
		}
	}
	
	public void Resume()
	{
		foreach (EPSoundEvent sound in m_EPSoundEventList)
		{
			sound.Resume ();
		}
	}
	
	// Functions for handling lists and sound name string checking
	int populateLists()
	{
		ClearLists();
		
		EPSoundEvent[] events = GetComponentsInChildren<EPSoundEvent>();
		foreach( EPSoundEvent child in events )
		{
			if ( ValidName( child.name ) )
			{
				m_EPSoundEventList.Add( child.GetComponent<EPSoundEvent>() );
				m_EPSoundNames.Add( child.name );
			}
			else
			{
				ClearLists();
				return -1;
			}
		}
		
		Debug.Log("Sound lists updated.");
		return 0;
	}
	
	void ClearLists()
	{
		m_EPSoundEventList.Clear();
		m_EPSoundNames.Clear ();
	}
	
	bool ValidName(string name)
	{
		if (m_EPSoundNames.Contains(name))
		{
			Debug.Log("Duplicate name found: " + name + ". List cannot contain duplicates");
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public int GetIndex( string sound_name )
	{
		for( int idx = 0; idx<m_EPSoundNames.Count; idx++ )
		{
			//Debug.Log ("Index: " + idx);
			if( sound_name == m_EPSoundNames[idx] )
				return idx;
		}
		
		Debug.Log("Sound name not found");
		return 0;
	}
}