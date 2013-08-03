//-----------------------------------------------------------------------------
// File: DivePath.cs
//
// Desc:	Defines a possibly rebounding path through the environment
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DivePath
{
	const int kMaxNodes = 10;
	public int m_NumNodes;
	
	public Vector3[] m_NodePositions = new Vector3[kMaxNodes];
	public float[] m_NodeTimes = new float[kMaxNodes];
	public float[] m_NodeTimes_wait = new float[kMaxNodes];
	
	public void CreateDivePath( Vector3 start_pos, Vector3 dir, float time, int max_bounces, float radius, float wait_before_bounce )
	{
		m_NumNodes = 1;
		
		m_NodePositions[0] = start_pos;
		m_NodeTimes[0] = 0.0f;
		
		float totalDistance = 0;
		Vector3 currentDirection = dir;
		Vector3 pos = m_NodePositions[0];
		
		while( m_NumNodes < max_bounces + 2 )
		{
			//	Raycast the current path
			RaycastHit hit;
			int layerMask = 1<<13; // Environment Collision"
			if( Physics.SphereCast ( pos, radius, currentDirection, out hit, layerMask ) ) 
			{
				if (hit.distance > radius)
				{	
					m_NodePositions[m_NumNodes] = hit.point + hit.normal * radius; //hit.point is (wrongly?) the point on the ray not the edge of the sphere
					float real_hit_distance = (m_NodePositions[m_NumNodes] - m_NodePositions[m_NumNodes-1]).magnitude;
					totalDistance += real_hit_distance;
				}
				else// Not clear how happening but it is when firing straight at wall, safest to abort
				{
					m_NumNodes = 0;
					return;
				}
				m_NodeTimes[m_NumNodes] = totalDistance;
				
				currentDirection = -2.0f*(Vector3.Dot( currentDirection, hit.normal ))*hit.normal + currentDirection;
				currentDirection.y = 0.0f;
				currentDirection.Normalize();
				
				m_NumNodes++;
				pos = m_NodePositions[m_NumNodes - 1];
			}
			else
			{	
				m_NumNodes = 0;
				return;
			}
		}
		
		time *= totalDistance/1000f; // going with constant speed
		for (int i = 1; i < m_NumNodes; i++)
		{
			m_NodeTimes[i] = time * (m_NodeTimes[i]/totalDistance);
			m_NodeTimes_wait[i] = m_NodeTimes[i];
			if (m_NumNodes >= 3)//unless no intermediary walls
			{
				//or on last wall
				if (i < m_NumNodes -1)
					m_NodeTimes_wait[i] += i * wait_before_bounce;//wait a moment before release
				else
					m_NodeTimes_wait[i] += (i-1) * wait_before_bounce;
			}
		}
	}
}
