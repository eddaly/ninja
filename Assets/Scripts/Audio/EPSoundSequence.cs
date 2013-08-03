//-----------------------------------------------------------------------------
// File: EPSoundEvent.cs
//
// Desc:	Class providing sequenced sounds from a single event trigger
//
// Copyright Echo Peak Ltd 2012
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EPSoundSequence : EPSoundEvent {
	
	public int m_numSounds;
	public float m_MasterVolume = 1, m_MasterPitch = 1;
	public EPSound[] m_EPSounds = new EPSound[0];
	public float[] m_Times = new float[0];
	public float[] m_Volumes = new float[0];
	public float[] m_Pitches = new float[0];
	
	float m_InstanceVolume = 1, m_InstancePitch = 1;
	
	// EPSoundTrigger
	public class EPSoundTrigger
	{
		public string m_soundName;
		public EPSound m_sound;
		public float m_time;
		public float m_volume;
		public float m_pitch;
		
		// Constructor
		public EPSoundTrigger(EPSound sound, string soundName, float time, float volume, float pitch)
		{
			m_sound = sound;
			m_soundName = soundName;
			m_time = time;
			m_volume = volume;
			m_pitch = pitch;
		}
	}
	
	// Create a list of EPSoundTriggers
	public List<EPSoundTrigger> m_TriggerList = new List<EPSoundTrigger>();
	
	// A float to hold the timestamp of the last update, so we can evaluate whether an event should have occurred
	float m_startTime = 0.0f;
	float m_lastUpdate = -0.1f;
	bool m_isActive = false;
	
	// Use this for initialization
	void Start ()
	{
		// Clear the list before updating
		m_TriggerList.Clear();
		
		for ( int i = 0; i < m_numSounds; i++)
		{
			m_TriggerList.Add ( new EPSoundTrigger(m_EPSounds[i], m_EPSounds[i].name, m_Times[i], m_Volumes[i], m_Pitches[i]) );
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_isActive )
		{
			float seqTime = Time.time - m_startTime;
			
			for ( int i = 0; i < m_TriggerList.Count; i++)
			{	
				float t = m_TriggerList[i].m_time;
				
				// Is the sound in the active time range?
				if ( t <= seqTime && t > m_lastUpdate )
				{
					//Console.WriteLine("i = " + i);
					//Console.WriteLine("Playing trigger " + (i+1) + " of " + m_TriggerList.Count + " at time: " + seqTime );
					m_TriggerList[i].m_sound.Play(m_TriggerList[i].m_volume * m_InstanceVolume, m_TriggerList[i].m_pitch * m_InstancePitch);
				}
			}
			
			if (seqTime >= m_TriggerList[m_TriggerList.Count-1].m_time)
			{
				m_isActive = false;
				//Console.WriteLine("Time elapsed = " + ( gameTime - m_startTime) );
				//Console.WriteLine ("Event finished");
			}
			// Set m_lastUpdate to the current time, ready for the next update
			m_lastUpdate = seqTime;
			//Console.WriteLine("Time = " + gameTime);
		}		
	}
	
	public override void Play ()
	{
		Play (1.0f , 1.0f);
	}
	
	public override void Play ( float volume, float pitch )
	{
		m_InstanceVolume = m_MasterVolume * volume;
		m_InstancePitch = m_MasterPitch * pitch;
		m_startTime = Time.time;
		m_lastUpdate = -0.1f;
		m_isActive = true;
	}
	
	public override void Stop ()
	{
		m_isActive = false;
	}
		
	public override void SetVolume( float volume )
	{}
	
	public override bool IsPlaying()
	{
		if (m_isActive)
			return true;
		else
			return false;
	}
		
	public override float time()
	{
		foreach ( EPSound sound in m_EPSounds )
		{
			if ( sound != null && sound.IsPlaying() )
				return sound.time();
		}
		return 0.0f;
	}
	
	public override void time( float time )
	{
		foreach ( EPSound sound in m_EPSounds )
		{
			if ( sound != null && sound.IsPlaying() )
				sound.time( time );
		}
	}
}
