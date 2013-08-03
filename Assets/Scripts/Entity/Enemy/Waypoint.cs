using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
	public enum WaypointAction
	{
		WAIT = 0,
		LOOKATANGLE,
		
		NUM
	}
	
	public List<WaypointAction> m_waypointActions = new List<WaypointAction>();
	public List<float> m_waypointActionLengths = new List<float>();
	public List<float> m_LookAngles = new List<float>();
	public float m_SpeedToReach = 100;
	
	[HideInInspector]
	public List<float> m_waypointActionStartTimes = new List<float>();
	

	// Use this for initialization
	void Start () 
	{
		//calculate the start times for all of the actions.
		m_waypointActionStartTimes.Add( 0 );
		for ( int i=1; i<m_waypointActions.Count; i++ )
		{
			m_waypointActionStartTimes.Add(  m_waypointActionStartTimes[i-1] + m_waypointActionLengths[i-1] );
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
