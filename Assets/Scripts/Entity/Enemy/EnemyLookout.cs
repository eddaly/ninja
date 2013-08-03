//-----------------------------------------------------------------------------
// File: EnemyLookout.cs
//
// Desc:	Enemy with a field of vision that can spot the player
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyLookout : MonoBehaviour 
{
	public enum EnemyLookoutState
	{
		IDLE = 0,
		PATROLLING,
		WAYPOINT,
		SLEEP,
		DEAD,
		
		NUM
	}
	
	public EnemyHealth enemyHealth;
	
	public float m_FOV = 60;
	public float m_VisionRange = 128;
	public List<Waypoint> m_Waypoints = new List<Waypoint>();
	public GameObject m_EnemyModel = null;
	public GameObject m_AnimatedEnemyModel = null;
	public EnemyLookoutState m_State = EnemyLookoutState.PATROLLING;
	public EnemyLookoutState m_OldState = EnemyLookoutState.PATROLLING;
	public float m_StateTime;
	public GameObject m_BloodSplat;
	public LayerMask m_VisionLayerMask;
	public GameObject m_Bullet;
	public float m_ShootPower = 10;
	public float m_ShootDelay = 0.5f;
	
	[HideInInspector]
	public bool m_hasSeenPlayer = false;
	
	//private members
	GameObject m_playerObject;
	MeshRenderer m_renderer;
	PlayerNinja m_player;
	bool m_startNewWaypoint = true;
	float m_speedToMove;
	Vector3 m_lastPosition;
	float m_timeTillNext;
	int m_nextWayPoint = 0;
	int m_executeWaypoint;
	float m_timeToTake;
	float m_ShootTimer;
	Vector3 m_defaultConeScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
	Vector3 m_ConeScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
	bool dying = false;
	float sleep_timer;
	LevelParams levelparams;
	GameObject viewCone;
	
	protected virtual void Start() 
	{
		m_playerObject = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.PLAYER_NINJA );
		m_player = m_playerObject.GetComponent<PlayerNinja>();
		m_renderer = m_EnemyModel.GetComponent<MeshRenderer>();
		viewCone = transform.FindChild("View").gameObject;
	}
	
	protected virtual void Update()
	{			
		m_StateTime += MainLoop.Get().m_FrameTime;
		m_ShootTimer -= Time.deltaTime;
		
		switch( m_State )
		{
		case EnemyLookoutState.IDLE:
			updateIdle();
			break;
		case EnemyLookoutState.PATROLLING:
			UpdatePatrolling();
			break;
		case EnemyLookoutState.WAYPOINT:
			UpdateWaypoint();
			break;
		case EnemyLookoutState.SLEEP:
			UpdateSleep();
			break;
		case EnemyLookoutState.DEAD:
			updateDead();
			break;
		}
		
		m_EnemyModel.transform.position = transform.position;

		UpdateConeScale();
	}
	
	void UpdateConeScale()
	{		
		m_ConeScale.x = ( m_FOV / 60 ) * m_defaultConeScale.x;
		m_ConeScale.z = ( m_VisionRange / 128 ) * m_defaultConeScale.z;
		if ( transform.FindChild("View") != null )
		{
			transform.FindChild("View").localScale = m_ConeScale;
		}
	}
	
	void updateIdle()
	{	
		checkKillRadius();
		checkVision();
	}
	
	void UpdatePatrolling() //handle the enemies patrolling between waypoints
	{
		if (m_AnimatedEnemyModel != null && m_AnimatedEnemyModel.animation != null)
			m_AnimatedEnemyModel.animation.Play("Enemy_Walk");
		else
			print ("enemy anim error");
		
		if( m_startNewWaypoint )
		{
			m_lastPosition = transform.position;
			m_startNewWaypoint = false;
			m_timeTillNext =0;
			m_timeToTake = Vector3.Distance( m_lastPosition, m_Waypoints[m_nextWayPoint].transform.position ) / m_Waypoints[m_nextWayPoint].m_SpeedToReach;
			
		}
		
		m_timeTillNext += Time.deltaTime / m_timeToTake;
		transform.position = Vector3.Lerp( m_lastPosition, m_Waypoints[m_nextWayPoint].transform.position, m_timeTillNext );
		transform.LookAt( m_Waypoints[m_nextWayPoint].transform );
		
		if(m_timeTillNext >= 1) //when we reach our current waypoint do the actions then move on.
		{
			if( m_Waypoints[m_nextWayPoint].m_waypointActions.Count > 0 )
			{
				m_executeWaypoint = m_nextWayPoint;
				ChangeState(EnemyLookoutState.WAYPOINT);
				return;
			}
			
			m_startNewWaypoint = true;
			m_nextWayPoint ++;
			if(m_nextWayPoint > m_Waypoints.Count-1)
			{
				m_nextWayPoint = 0;
			}
		}
		
		checkKillRadius();
		checkVision();
	}
	
	public void Sleep()
	{
		if (m_State != EnemyLookoutState.SLEEP)
		{
			ChangeState (EnemyLookoutState.SLEEP);
			levelparams = m_player.hud.m_LevelParams;
			sleep_timer = 0;
			viewCone.SetActive (false);
		}
	}
	
	void UpdateSleep ()
	{
		if (sleep_timer == 0 && m_AnimatedEnemyModel != null && m_AnimatedEnemyModel.animation != null)
		{
			m_AnimatedEnemyModel.animation.wrapMode = WrapMode.Once;
			m_AnimatedEnemyModel.animation.Play("Enemy_Die");
		}

		sleep_timer += Time.deltaTime;
		if (sleep_timer >= levelparams.m_Sleep_Gas_Effect_Duration)
		{
			ChangeState (EnemyLookoutState.PATROLLING);
			viewCone.SetActive (true);
			return;
		}
		
		//EPSoundController.Get().Play("Kill");
		//***turn off vision cone
	}
	
	void UpdateWaypoint() //complete all actions set at the current waypoint
	{
		Waypoint waypoint =  m_Waypoints[m_executeWaypoint];
		
		for( int i=0; i<waypoint.m_waypointActions.Count; i++ )
		{
			if( m_StateTime > waypoint.m_waypointActionStartTimes[i] && m_StateTime < ( waypoint.m_waypointActionStartTimes[i] + waypoint.m_waypointActionLengths[i] ) )
			{
				switch (waypoint.m_waypointActions[i])
				{
					case Waypoint.WaypointAction.LOOKATANGLE:
					{	
						transform.rotation = Quaternion.AngleAxis( waypoint.m_LookAngles[i], Vector3.up );
						break;
					}
					case Waypoint.WaypointAction.WAIT:
					{	
						break;
					}
				}
			}
		}
		
		checkVision();
		
		if( m_StateTime > ( waypoint.m_waypointActionStartTimes[waypoint.m_waypointActions.Count-1] + waypoint.m_waypointActionLengths[waypoint.m_waypointActions.Count-1] ) )
		{
			m_startNewWaypoint = true;
			m_nextWayPoint ++;
			if(m_nextWayPoint > m_Waypoints.Count-1)
			{
				m_nextWayPoint = 0;
			}
			ChangeState(EnemyLookoutState.PATROLLING);
			return;
		}
	}
	
	void updateDead()
	{
		if(gameObject.activeSelf)
		{
			if (m_AnimatedEnemyModel != null && m_AnimatedEnemyModel.animation != null)
			{
				m_AnimatedEnemyModel.animation.wrapMode = WrapMode.Once;
				if (!dying)
				{
					Instantiate(m_BloodSplat, transform.position, transform.rotation);
					m_AnimatedEnemyModel.animation.Play("Enemy_Die");
					EPSoundController.Get().Play("Kill");
					dying = true;
				}
				if (!m_AnimatedEnemyModel.animation.IsPlaying("Enemy_Die"))
				{
					gameObject.SetActive(false);
					//gameObject.collider.enabled = false;
				}
			}
			else
			{
				gameObject.SetActive(false);
				Instantiate(m_BloodSplat, transform.position, transform.rotation);
			}			
		}
	}
	
	void ChangeState( EnemyLookoutState new_state )
	{
		m_StateTime = 0.0f;
		m_OldState = m_State;
		m_State = new_state;
	}
	
	bool checkVision() //check if the player is within the enemies FOV range. 
	{
		if (m_player.inshadow)
		{
			return false;
		}
		
		Vector3 directionToTarget = transform.position - m_player.transform.position;
	    float angel = 180 - Vector3.Angle(transform.forward, directionToTarget);
		
	    if ( angel <= m_FOV/2 )
		{
			Vector3 rayDirection =  m_player.transform.position - transform.position;
 			RaycastHit hit;
				
			if ( Physics.Raycast ( transform.position, rayDirection, out hit, m_VisionRange, m_VisionLayerMask ) ) 
			{
				if(hit.transform == ReferenceLibraryInGame.GetReference(ReferenceLibraryInGame.Ref.NINJA).transform)
				{
					// enemy can see the player!
					m_renderer.material.color = Color.green;
					m_hasSeenPlayer = true;
					Debug.Log("Can see player");
					
					// if diving he's too quick to shoot
					if (m_player.m_State != PlayerNinja.PlayerNinjaState.DIVING)
					{	
						if(m_ShootTimer <= 0)
						{
							ShootPlayer();
							m_ShootTimer = m_ShootDelay;
								
						}
					}				
					return true;
				}
			}
		}
		return false;
	}
	
	bool checkKillRadius() //check if the player is close enough to attack 
	{
		// if diving he's too quick to shoot
		if (m_player.inshadow || m_player.m_State == PlayerNinja.PlayerNinjaState.DIVING)
			return false;
		
		float killRadius = m_player.m_SwordRadius;
		Vector3 playerPos = m_player.transform.position;
		
		Vector3 diff = transform.position - playerPos;
		diff.y = 0.0f;
		
		float sqDist = diff.sqrMagnitude;

		if( sqDist < killRadius*killRadius )
		{
			m_renderer.material.SetColor( "_Color", new Color( 1.0f, 0.0f, 1.0f, 1.0f ));
			return true;
		}
		else
		{
			m_renderer.material.SetColor( "_Color", new Color( 0.0f, 0.0f, 1.0f, 1.0f ));
			return false;
		}
	}

	public virtual void Hit()
	{
		if (m_State == EnemyLookoutState.SLEEP) //seems mean, also displaying hearts and zzz is a mess
			return;
		
		if (m_State != EnemyLookoutState.DEAD && enemyHealth != null)
		{
			if (--enemyHealth.health == 0)
			{
				Kill ();
			}
		}
		else
		{
			Kill ();
		}
	}
	
	public void Kill()
	{
		if (m_State != EnemyLookoutState.DEAD )
		{
			ChangeState( EnemyLookoutState.DEAD	);
			m_player.hud.AddKill();
		}
	}
	
	void ShootPlayer()
	{
		if (dying)
			return;
		
	    Vector3 shootDir =  m_player.transform.position - transform.position;
		shootDir.Normalize();
		shootDir.y = 0;
	    GameObject bulletShot = (GameObject)Instantiate(m_Bullet, transform.position, transform.rotation);
	    bulletShot.GetComponent<Rigidbody>().AddForce(shootDir * m_ShootPower*Time.deltaTime * 25);
		
		EPSoundController.Get().Play("Shoot");
	}
	
	/*void OnDrawGizmosSelected()
    {
    float totalFOV = 70.0f;
    float rayRange = 10.0f;
    float halfFOV = totalFOV / 2.0f;
    Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
    Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
    Vector3 leftRayDirection = leftRayRotation * transform.forward;
    Vector3 rightRayDirection = rightRayRotation * transform.forward;
    Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
    Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
    }*/
}
