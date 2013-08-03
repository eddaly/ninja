using UnityEngine;
using System.Collections;

public class EntityPhysics : MonoBehaviour 
{
	public internalPhysics m_Internal;

	public Vector2 m_MinBoundary;
	public Vector2 m_MaxBoundary;

	public Vector2 m_Gravity;
	public float m_TimeScale = 1.0f;	
	public float m_Damping = 1.0f;
	
	//float m_GlobalConstraintPower = 1.0f;

	float m_TimeElapsed;
	
	//	Static accessor
	private static EntityPhysics ms_entityPhysicsObject = null;
    public static EntityPhysics Get()
    {
		//	Cache the object to avoid frequent string lookups
		if( ms_entityPhysicsObject != null )
			return ms_entityPhysicsObject;

		GameObject entityPhysics = GameObject.Find( "PhysicsObject" );
        if( entityPhysics == null )
        {
            Debug.Log( "!** No EntityPhysics object found (PhysicsObject)" );
        }
        else
        {
            ms_entityPhysicsObject = entityPhysics.GetComponent<EntityPhysics>();
        }

		return ms_entityPhysicsObject;
    }

	void Start () 
	{
		m_Internal = new internalPhysics();
		m_Internal.Initialise();

		m_Internal.m_MinBoundary = m_MinBoundary;
		m_Internal.m_MaxBoundary = m_MaxBoundary;

		m_Internal.m_Gravity = m_Gravity;
		
		m_TimeElapsed = 0.0f;
	}
	
	
	
	void Update () 
	{
		const float timeSections = 1.0f/60.0f;
		const int frameIterations = 5;
		
		float dTime = Time.deltaTime;
		
		m_TimeElapsed += dTime;
		
		while( m_TimeElapsed > timeSections )
		{
			for( int i = 0; i<frameIterations; i++ )
			{			
				Profiler.BeginSample( "Integrate" );
				m_Internal.Integrate( (timeSections*m_TimeScale)/(float)frameIterations );
				Profiler.EndSample();
				Profiler.BeginSample( "Boundaries" );
				m_Internal.CheckBoundaries();
				Profiler.EndSample();
				Profiler.BeginSample( "Constraints" );
				m_Internal.ProcessConstraints();
				Profiler.EndSample();
				Profiler.BeginSample( "Repel" );
				m_Internal.RepelParticles();		
				Profiler.EndSample();
				Profiler.BeginSample( "RepelLine" );
				m_Internal.RepelParticlesWithLines();
				Profiler.EndSample();
			}
	
			m_Internal.PrepareNextFrame();
			
			m_TimeElapsed -= timeSections;
		}
	}
	
	public int GetPhysicsParticle( float pos_x, float pos_y )
	{
		return GetPhysicsParticle( pos_x, pos_y, 0.0f, 0.0f, 10.0f, 1.0f );
	}
	public int GetPhysicsParticle( float pos_x, float pos_y, float damping )
	{
		return GetPhysicsParticle( pos_x, pos_y, 0.0f, 0.0f, 10.0f, damping );
	}
	public int GetPhysicsParticle( float pos_x, float pos_y, float vel_x, float vel_y, float damping )
	{
		return GetPhysicsParticle( pos_x, pos_y, vel_x, vel_y, 10.0f, damping );
	}
	public int GetPhysicsParticle( float pos_x, float pos_y, float vel_x, float vel_y, float radius, float damping )
	{
		return m_Internal.GetPhysicsParticle( pos_x, pos_y, vel_x, vel_y, radius, damping, 5.0f );
	}
	public int GetPhysicsParticle( float pos_x, float pos_y, float vel_x, float vel_y, 
					float radius, float damping, float repulsion )
	{
		return m_Internal.GetPhysicsParticle( pos_x, pos_y, vel_x, vel_y, radius, damping, repulsion );
	}
	
	public int GetConstraint( int particle1, int particle2 )
	{
		float xDist = m_Internal.m_Particles[particle1].pos.x - 
						m_Internal.m_Particles[particle2].pos.x;
		float yDist = m_Internal.m_Particles[particle1].pos.y - 
						m_Internal.m_Particles[particle2].pos.y;
		
		float dist = Mathf.Sqrt( xDist*xDist + yDist*yDist );
		
		return GetConstraint( particle1, particle2, dist );
	}
	public int GetConstraint( int particle1, int particle2, float length )
	{
		return GetConstraint( particle1, particle2, length, 5, 0.5f );
	}
	public int GetConstraint( int particle1, int particle2, float length, int iterations, float strength )
	{
		float newLength = length;
		if( newLength < 0.0f )
		{
			float xDist = m_Internal.m_Particles[particle1].pos.x - 
							m_Internal.m_Particles[particle2].pos.x;
			float yDist = m_Internal.m_Particles[particle1].pos.y - 
							m_Internal.m_Particles[particle2].pos.y;
			
			newLength = Mathf.Sqrt( xDist*xDist + yDist*yDist );
		}
		
		return m_Internal.GetConstraint( particle1, particle2, newLength, iterations, strength );
	}
}

public class internalPhysics
{
	const int m_maxEntities = 200;	
	public struct particle
	{
		public Vector2 oldPos;
		public Vector2 pos;
		public Vector2 acc;
		
		public float repulsion;
		
		public float damping;
		
		public float radius;
		public float weight;
		
		public bool active;
	};
	public particle[] m_Particles = new particle[m_maxEntities];
	int m_particleSearchCursor;
	
	
	const int m_maxConstraints = 500;
	public struct constraint
	{
		public float length;
		public int iterations;
		public float strength;
		
		public int particle1;
		public int particle2;
		
		public bool active;
	};
	public constraint[] m_Constraints = new constraint[m_maxConstraints];
	public Vector2[] m_ParticlePositionBuffer = new Vector2[m_maxEntities];
	int m_constraintSearchCursor;

	
	const int m_maxLines = 10;
	public struct line
	{
		public Vector2 start;
		public Vector2 end;
		
		public float repulsion;
		
		public bool active;
	};
	public line[] m_Lines = new line[m_maxLines];
	int m_lineSearchCursor;

	
	public Vector2 m_MinBoundary;
	public Vector2 m_MaxBoundary;

	public Vector2 m_Gravity;
	
	public float m_GlobalConstraintPower = 1.0f;
		
	public void Initialise()
	{		
		for( int e = 0; e<m_maxEntities; e++ )
		{
			m_Particles[e].oldPos.x = 0.0f;
			m_Particles[e].oldPos.y = 0.0f;

			m_Particles[e].pos.x = 0.0f;
			m_Particles[e].pos.y = 0.0f;

			m_Particles[e].acc.x = 0.0f;
			m_Particles[e].acc.y = 0.0f;
			
			m_Particles[e].repulsion = 5.0f;
			
			m_Particles[e].damping = 1.0f;
			
			m_Particles[e].weight = 1.0f;
			
			m_Particles[e].active = false;
		}		
		m_particleSearchCursor = 0;
		
		
		for( int c = 0; c<m_maxConstraints; c++ )
		{
			m_Constraints[c].active = false;
		}
		m_constraintSearchCursor = 0;		
		
		
		for( int l = 0; l<m_maxLines; l++ )
		{
			m_Lines[l].active = false;
		}
		m_lineSearchCursor = 0;
	}
	
	public int GetPhysicsParticle( float pos_x, float pos_y, float vel_x, float vel_y, 
					float radius, float damping, float repulsion )
	{
		int originalSearchPoint = m_particleSearchCursor++;
 		if( m_particleSearchCursor == m_maxEntities )
			m_particleSearchCursor = 0;
		
		while( m_particleSearchCursor != originalSearchPoint && 
				m_Particles[m_particleSearchCursor].active )
		{
			m_particleSearchCursor++;
			if( m_particleSearchCursor == m_maxEntities )
				m_particleSearchCursor = 0;
		}
		
		//	Return -1 if we can't find a spare one
		if( m_particleSearchCursor == originalSearchPoint )
			return -1;
	
		Debug.Log( "got " + m_particleSearchCursor.ToString() );
		
		m_Particles[m_particleSearchCursor].oldPos.x = pos_x - vel_x;
		m_Particles[m_particleSearchCursor].oldPos.y = pos_y - vel_y;
		
		m_Particles[m_particleSearchCursor].pos.x = pos_x;
		m_Particles[m_particleSearchCursor].pos.y = pos_y;
				
		m_Particles[m_particleSearchCursor].radius = radius;
		m_Particles[m_particleSearchCursor].damping = damping;
		
		m_Particles[m_particleSearchCursor].repulsion = repulsion;
		
		m_Particles[m_particleSearchCursor].active = true;
		return m_particleSearchCursor;
	}
	
	public void ReleasePhysicsParticle( int particle_id )
	{
		m_Particles[particle_id].active = false;
	}

	public int GetConstraint( int particle1, int particle2, float length, int iterations, float strength )
	{
		int originalSearchPoint = m_constraintSearchCursor++;
 		if( m_constraintSearchCursor == m_maxConstraints )
			m_constraintSearchCursor = 0;
		
		while( m_constraintSearchCursor != originalSearchPoint && 
				m_Constraints[m_constraintSearchCursor].active )
		{
			m_constraintSearchCursor++;
			if( m_constraintSearchCursor == m_maxConstraints )
				m_constraintSearchCursor = 0;
		}
		
		//	Return -1 if we can't find a spare one
		if( m_constraintSearchCursor == originalSearchPoint )
			return -1;

		Debug.Log( "constraint " + m_constraintSearchCursor.ToString() );
		
		m_Constraints[m_constraintSearchCursor].length = length;
		m_Constraints[m_constraintSearchCursor].iterations = iterations;
		m_Constraints[m_constraintSearchCursor].strength = strength;
		
		m_Constraints[m_constraintSearchCursor].particle1 = particle1;
		m_Constraints[m_constraintSearchCursor].particle2 = particle2;
		
		m_Constraints[m_constraintSearchCursor].active = true;
		
		return m_constraintSearchCursor;
	}
	
	public void ReleaseConstraint( int constraint_id )
	{
		m_Constraints[constraint_id].active = false;
	}

	public int GetLine( Vector2 start, Vector2 end, float repulsion )
	{
		int originalSearchPoint = m_lineSearchCursor++;
 		if( m_lineSearchCursor == m_maxLines )
			m_lineSearchCursor = 0;
		
		while( m_lineSearchCursor != originalSearchPoint && 
				m_Lines[m_lineSearchCursor].active )
		{
			m_lineSearchCursor++;
			if( m_lineSearchCursor == m_maxLines )
				m_lineSearchCursor = 0;
		}
		
		//	Return -1 if we can't find a spare one
		if( m_lineSearchCursor == originalSearchPoint )
			return -1;

		Debug.Log( "line " + m_lineSearchCursor.ToString() );

		m_Lines[m_lineSearchCursor].start = start;
		m_Lines[m_lineSearchCursor].end = end;

		m_Lines[m_lineSearchCursor].repulsion = repulsion;
		
		m_Lines[m_lineSearchCursor].active = true;
		return m_lineSearchCursor;
	}
	
	public void ReleaseLine( int line_id )
	{
		m_Lines[line_id].active = false;
	}
	
	public void Integrate( float d_time )
	{	
		float dTime = d_time;
		if( dTime > 1/30.0f )
			dTime = 1/30.0f;
		
		float sqDTime = d_time*d_time;
		
		float posX;
		float posY;
		float damping;
		
		float frameMoveX;
		float frameMoveY;
		
		//	The following loop with 10,000,000 entities, in ms...
		//
		//	314 accessing floats directly, without caching current particle
		//	340 with caching!!
		//	330 caching with predeclaration <- means compiler doesn't do this!!
		//	310 with caching only those that are used multiple times
		//	1212 using Vector2 operations!!!!!!
		
		for( int e = 0; e<m_maxEntities; e++ )
		{
			if( m_Particles[e].active )
			{
				posX = m_Particles[e].pos.x;
				posY = m_Particles[e].pos.y;
				damping = m_Particles[e].damping;
				
				//	Integrate with the new forces
				float frameMoveX2 = posX;
				frameMoveX2 -= m_Particles[e].oldPos.x;
				
				frameMoveX = (posX - m_Particles[e].oldPos.x)*damping*EntityPhysics.Get().m_Damping;
				frameMoveY = (posY - m_Particles[e].oldPos.y)*damping*EntityPhysics.Get().m_Damping;
				
				m_Particles[e].oldPos.x = posX;
				m_Particles[e].oldPos.y = posY;
	
				m_Particles[e].pos.x += frameMoveX + m_Particles[e].acc.x*sqDTime;
				m_Particles[e].pos.y += frameMoveY + m_Particles[e].acc.y*sqDTime;
			}
		}
	}

	public void ProcessConstraints()
	{
		int p1, p2;
		float xDiff, yDiff;
		float sqDist;
		float distError;
		float xAdjust, yAdjust;
		
		//	First reset the position buffer we use so we avoid cross-processing particles
		for( int e = 0; e<m_maxEntities; e++ )
			m_ParticlePositionBuffer[e] = m_Particles[e].pos;
		
		//	Constraints are applied a maximum of 10 times per frame
		for( int i = 0; i<10; i++ )
		{			
			//	Now apply each constraint in turn
			for( int c = 0; c<m_maxConstraints; c++ )
			{
				if( m_Constraints[c].active && m_Constraints[c].iterations > i )
				{
					p1 = m_Constraints[c].particle1;
					p2 = m_Constraints[c].particle2;
					
					xDiff = m_Particles[p2].pos.x - m_Particles[p1].pos.x;
					yDiff = m_Particles[p2].pos.y - m_Particles[p1].pos.y;
					
					sqDist = xDiff*xDiff + yDiff*yDiff;
					if( sqDist < 0.05f ) 
					{
						sqDist = 0.05f;
						xDiff = 0.05f;
						yDiff = 0.0f;
					}
			
					distError = (sqDist - m_Constraints[c].length*m_Constraints[c].length)/sqDist;
					distError *= m_GlobalConstraintPower;

					xAdjust = xDiff*distError*m_Constraints[c].strength;
					yAdjust = yDiff*distError*m_Constraints[c].strength;

					m_ParticlePositionBuffer[p1].x += xAdjust;
					m_ParticlePositionBuffer[p1].y += yAdjust;
					
					m_ParticlePositionBuffer[p2].x -= xAdjust;
					m_ParticlePositionBuffer[p2].y -= yAdjust;
				}
			}
			
			//	Apply the adjustments for this iteration
			for( int e = 0; e<m_maxEntities; e++ )
			{
				m_Particles[e].pos.x = m_ParticlePositionBuffer[e].x;
				m_Particles[e].pos.y = m_ParticlePositionBuffer[e].y;
			}
		}
		
	}
	
	public void CheckBoundaries()
	{
		const float restitution = 0.999995f;
		
		for( int e = 0; e<m_maxEntities; e++ )
		{
			if( m_Particles[e].active )
			{
				if( m_Particles[e].pos.x < m_MinBoundary.x )
				{
					float vel = m_Particles[e].pos.x - m_Particles[e].oldPos.x;
					
					m_Particles[e].pos.x = m_MinBoundary.x;
					m_Particles[e].oldPos.x = m_Particles[e].pos.x + vel*restitution;
				}
				else if( m_Particles[e].pos.x > m_MaxBoundary.x )
				{
					float vel = m_Particles[e].pos.x - m_Particles[e].oldPos.x;
					
					m_Particles[e].pos.x = m_MaxBoundary.x;
					m_Particles[e].oldPos.x = m_Particles[e].pos.x + vel*restitution;
				}
				
				if( m_Particles[e].pos.y < m_MinBoundary.y )
				{
					float vel = m_Particles[e].pos.y - m_Particles[e].oldPos.y;
					
					m_Particles[e].pos.y = m_MinBoundary.y;
					m_Particles[e].oldPos.y = m_Particles[e].pos.y + vel*restitution;
				}
				else if( m_Particles[e].pos.y > m_MaxBoundary.y )
				{
					float vel = m_Particles[e].pos.y - m_Particles[e].oldPos.y;
					
					m_Particles[e].pos.y = m_MaxBoundary.y;
					m_Particles[e].oldPos.y = m_Particles[e].pos.y + vel*restitution;
				}
			}
		}
	}

	public void RepelParticles()
	{		
		float xDist;
		float yDist;
		float dist;
		float maxDist;
		float power;
		
		float normX;
		float normY;
		
		float pushX1;
		float pushY1;
		float pushX2;
		float pushY2;
		
		int numPushes = 0;
		
		for( int e1 = 0; e1<m_maxEntities - 1; e1++ )
		{
			if( m_Particles[e1].active )
			{
				for( int e2 = e1 + 1; e2<m_maxEntities; e2++ )
				{
					if( m_Particles[e2].active )
					{
						maxDist = m_Particles[e1].radius + m_Particles[e2].radius;

						xDist = m_Particles[e1].pos.x - m_Particles[e2].pos.x;
						
						if( xDist < maxDist )
						{
							yDist = m_Particles[e1].pos.y - m_Particles[e2].pos.y;

							if( yDist < maxDist )
							{
								numPushes++;
								
								dist = xDist*xDist + yDist*yDist;
								maxDist *= maxDist;
														
								power = 1.0f - ( dist/maxDist );
								if( power > 0.0f )
								{				
									normX = xDist/dist;
									normY = yDist/dist;
									
									pushX1 = normX*power*m_Particles[e1].repulsion;
									pushY1 = normY*power*m_Particles[e1].repulsion;
		
									pushX2 = normX*power*m_Particles[e2].repulsion;
									pushY2 = normY*power*m_Particles[e2].repulsion;
									
									m_Particles[e1].acc.x += pushX2;
									m_Particles[e1].acc.y += pushY2;
									m_Particles[e2].acc.x -= pushX1;
									m_Particles[e2].acc.y -= pushY1;
								}
							}
						}
					}
				}
			}
		}
		
//		Debug.Log ( numPushes.ToString() );
	}

	public void RepelParticlesWithLines()
	{
		for( int l = 0; l<m_maxLines; l++ )
		{
			if( m_Lines[l].active )
			{
				for( int p = 0; p<m_maxEntities; p++ )
				{
					if( m_Particles[p].active )
					{
						Vector2 lineVec = m_Lines[l].end - m_Lines[l].start;
						float lineSqMag = lineVec.sqrMagnitude;
						Vector2 lineStartToParticle = m_Particles[p].pos - m_Lines[l].start;
						
						float normalIntersection = Vector2.Dot( lineStartToParticle, lineVec )/lineSqMag;
						
						if( normalIntersection >= 0.0f && normalIntersection <= 1.0f )
						{
							Vector2 projectionPoint = m_Lines[l].start + lineVec*normalIntersection;
							
							Vector2 diff = projectionPoint - m_Particles[p].pos;
							float dist = diff.sqrMagnitude;
							
							float maxDist = m_Particles[p].radius*m_Particles[p].radius;
							if( dist < maxDist )
							{
								//	Collision detected, push back the particle
								float power = 1.0f - ( dist/maxDist );
								if( power > 0.0f )
								{				
									float normX = diff.x/dist;
									float normY = diff.y/dist;
									
									float pushX = normX*power*m_Lines[l].repulsion;
									float pushY = normY*power*m_Lines[l].repulsion;
		
									m_Particles[p].acc.x -= pushX;
									m_Particles[p].acc.y -= pushY;
								}
								
							}
						}
						
						/*
	if(normalIntersection >= 0.f && normalIntersection <= 1.f)
	{
		colInfo.mCollisionPoint = mLineStart + lineVec*normalIntersection;

		VEC3 diff = colInfo.mCollisionPoint - sphere->mCollidePosition;
		if(VEC3LengthSq(&diff) < mRadius*mRadius + sphere->mRadius*sphere->mRadius)
			colInfo.mCollided = TRUE;*/
					}
				}
			}
		}
		
//		Debug.Log ( numPushes.ToString() );
	}
	
	public void PrepareNextFrame()
	{
		for( int e = 0; e<m_maxEntities; e++ )
		{
			if( m_Particles[e].active )
			{
				m_Particles[e].acc.x = m_Gravity.x;
				m_Particles[e].acc.y = m_Gravity.y;
			}
		}
	}
}