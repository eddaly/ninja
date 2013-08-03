//-----------------------------------------------------------------------------
// File: PlayerNinja.cs
//
// Desc:	The main Ninja player enitity
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class PlayerNinja : MonoBehaviour 
{
	public GameObject m_NinjaModel = null;
	public GameObject m_NinjaMesh = null;
	
	PlayerController m_Controller = null;

	public enum PlayerNinjaState
	{
		IDLE = 0,
		AIMING,
		DIVING,
		DYING
	}
	public PlayerNinjaState m_State = PlayerNinjaState.IDLE;
	public PlayerNinjaState m_OldState = PlayerNinjaState.IDLE;
	public float m_StateTime;
	
	public DivePath m_DivePath;
	
	public float m_MaxPower = 500.0f;
	public float m_PingTime = 0.5f;
	public float m_AimRadius = 50.0f;
	public float m_DeadZone = 20;
	public float m_MaxArrowLength = 5;
	
	public int m_MaxBounces = 3;
	
	public float m_SwordRadius = 200.0f;
	public float m_ShurikenRadius = 500.0f;
	
	public float m_ShurikenSpeed = 1000.0f;
	public float m_MaxSwordRange = 5f;
	
	
	public float m_CurrentPingPower = 0.0f;
	
	static int gShurikenID = 0;
	
	public HUD hud;
	public bool inshadow;
	public GameObject m_Aimarrow = null;
	public bool discrete_bounce_mode = true;
	public GameObject animatedPlayerModel; //Animated model that will have all the animations in it
	public SphereCollider ninja_sphere_collider;
	public float wait_before_bounce = .125f;
	public Material m_TrailMaterial = null;
	bool dying = false;
	
	bool jumping = false;
	public float jumpingHeight = 100;
	float jumpingPosition = 0;
	public float jumpingDistance = 200;
	
	// Players's stuff (that would need to be saved etc.)
	public int persistent_coins;
	public int persistent_bombgadgets;
	public int persistent_sleepinggasgadgets;
	public int persistent_springgadgets;
	
	void Awake ()
	{
		gShurikenID = 0;//reentry
		dying = false;
	}
	
	void Start() 
	{
		GameObject playerGroup = GameObject.Find( "Player" );
		
		if( playerGroup != null )
		{
			Transform controllerObject = playerGroup.transform.FindChild( "PlayerController" );
			
			if( controllerObject != null )
			{
				m_Controller = controllerObject.GetComponent<PlayerController>();
			}
		}
		inshadow = false;
		m_Aimarrow.renderer.enabled = false;
			
		if (!CheckAnims ())
		{
			Debug.Log ("ANIMATIONS MISSING?");
		}
		if (Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			m_DeadZone *= 1.5f;
			m_AimRadius *= 2f;
		}
		
		// Load it!
		persistent_coins = persistent_bombgadgets = persistent_sleepinggasgadgets = persistent_springgadgets = 0;
		
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Ninja/ParticlesAdditiveOverlay"));
        lineRenderer.SetColors(new Color(0,0,1,1f), new Color(0,0,1,1f));
        lineRenderer.SetWidth(10,10);
        lineRenderer.SetVertexCount(0);
	}
	
	bool CheckAnims()
	{
		if (!animatedPlayerModel)
			return false;

		if (animatedPlayerModel.animation["Ninja_01_Idle"] == null ||
			animatedPlayerModel.animation["Ninja_01_Run"] == null ||
			animatedPlayerModel.animation["Ninja_01_Throw"] == null ||
			animatedPlayerModel.animation["Ninja_01_Sword"] == null)
			return false;

		return true;
	}
	
	void Update()
	{	
		m_StateTime += MainLoop.Get().m_FrameTime;
		m_CurrentPingPower = 0.0f;
		m_Aimarrow.renderer.enabled = false;
		
		if (hud.hudstate != HUD.HudState.PLAY)
			return;
		
		switch( m_State )
		{
		case PlayerNinjaState.IDLE:
			updateIdle();
			break;
		case PlayerNinjaState.AIMING:
			updateAiming();
			break;
		case PlayerNinjaState.DIVING:
			updateDiving();
			break;
		case PlayerNinjaState.DYING:
			updateDying();
			break;
		}
		m_NinjaModel.transform.position = transform.position;
	}

	
	void updateIdle()
	{
		/*if (RuntimePlatform == RuntimePlatform.Android || RuntimePlatform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.GetTouch (0).phase == TouchPhase.Began)//(m_Controller.m_FireDown )
				
		}*/
		
		bool insideAimRadius = false;
		
		Vector3 diff = transform.position - m_Controller.m_MouseWorldPos;
		diff.y = 0.0f;
		
		float sqDist = diff.sqrMagnitude;
		if( sqDist < m_AimRadius*m_AimRadius )
			insideAimRadius = true;
		
		MeshRenderer renderer = m_NinjaModel.GetComponent<MeshRenderer>();
		if( insideAimRadius )
		{
			renderer.material.SetColor( "_Color", new Color( 0.0f, 1.0f, 0.0f, 1.0f ));
			
			if( m_Controller.m_FireDown )
				ChangeState( PlayerNinjaState.AIMING );
		}
		else
		{
			renderer.material.SetColor( "_Color", new Color( 1.0f, 1.0f, 1.0f, 1.0f ));
			AttackFireCheck();
			if (!animatedPlayerModel.animation.IsPlaying ("Ninja_01_Throw") &&
				!animatedPlayerModel.animation.IsPlaying ("Ninja_01_Sword"))
				animatedPlayerModel.animation.Play("Ninja_01_Idle");
		}
	}
	
	public void ExitReached ()
	{
		hud.ExitReached ();
		ChangeState( PlayerNinjaState.IDLE );
		hud.SetMaxBounces (-1);
	}

	
	void updateAiming()
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		
		Vector3 diff = transform.position - m_Controller.m_MouseWorldPos;
		diff.y = 0.0f;

		// Face front man
		Vector3 v = transform.position + diff;
		animatedPlayerModel.transform.LookAt (v);

		float sqDist = diff.sqrMagnitude;
		if( sqDist < m_DeadZone * m_DeadZone )
		{
			//	Input is in deadzone - cancel the shot
			ChangeState( PlayerNinjaState.IDLE );
			hud.SetMaxBounces (-1);
			lineRenderer.SetVertexCount (0);
			return;
		}
		
		m_CurrentPingPower = (diff.magnitude/m_AimRadius);
		int maxbounces;
		if (Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (diff.magnitude >= m_AimRadius * 2)
				maxbounces = 3;
			else if(diff.magnitude >= m_AimRadius * 1.5)
				maxbounces = 2;
			else
				maxbounces = 1;
		}
		else
		{
			if (diff.magnitude >= m_AimRadius * 2)
				maxbounces = 3;
			else if(diff.magnitude >= m_AimRadius * 1.5)
				maxbounces = 2;
			else
				maxbounces = 1;
		}
		
		DivePath currentPath = new DivePath();			
		float radius = ninja_sphere_collider.radius * ninja_sphere_collider.transform.localScale.z;
		Vector3 p = transform.position;
		currentPath.CreateDivePath( p, diff.normalized, m_PingTime, maxbounces-1, 
			radius, wait_before_bounce);
		
		if (currentPath.m_NumNodes == 0) // zero if couldnt create path (as firing into wall)
		{
			hud.SetMaxBounces (-1);
			lineRenderer.SetVertexCount (0);
			return;
		}
				
		DrawAimArrow (maxbounces);
		
		lineRenderer.SetVertexCount (currentPath.m_NumNodes);
		lineRenderer.SetWidth(10,10);
		lineRenderer.SetPosition(0, currentPath.m_NodePositions[0]);
		
		//	Draw path
		for( int r = 0; r<currentPath.m_NumNodes - 1; r++ )
		{
			lineRenderer.SetPosition(r+1, currentPath.m_NodePositions[r+1]);
		}
		//	Letting go of fire initiates the dive
		if( !m_Controller.m_FireDown )
		{
			m_DivePath = currentPath;
			lineRenderer.SetVertexCount (0);
			ChangeState( PlayerNinjaState.DIVING );
			hud.SetMaxBounces (-1);
		}
	}
	
	void updateDiving()
	{
		if (m_DivePath.m_NumNodes == 0) // possible if aborted
		{
			ChangeState( PlayerNinjaState.IDLE );
			return;
		}
		
		float normalTime = m_StateTime/m_DivePath.m_NodeTimes_wait[m_DivePath.m_NumNodes - 1];
		
		if( normalTime > 1.0f )
		{
			if (jumping)
			{
				Vector3 p = transform.position;
				p.y = 0;
				transform.position = p;
				jumping = false;
			}
			
			//	Finished dive
			ChangeState( PlayerNinjaState.IDLE );
							
			// Face front man
			animatedPlayerModel.transform.RotateAround (Vector3.up, 180f);
			
			hud.TurnComplete();
		}
		else
		{
			int finalNode = 0;
			
			while( m_StateTime >= m_DivePath.m_NodeTimes_wait[finalNode] && finalNode < m_DivePath.m_NumNodes )
				finalNode++;

			//	Work out section-specific normal time
			float sectionTime = m_DivePath.m_NodeTimes[finalNode] - m_DivePath.m_NodeTimes[finalNode - 1];
			float timeInSection = m_StateTime - (m_DivePath.m_NodeTimes_wait[finalNode - 1]);
			float sectionNormal = timeInSection/sectionTime;
			if (sectionNormal > 1)
			{
				sectionNormal = 1; //waiting
			}
			
			Vector3 newPos = (1.0f - sectionNormal)*m_DivePath.m_NodePositions[finalNode - 1] + 
							 sectionNormal*m_DivePath.m_NodePositions[finalNode];
			Vector3 oldPos = transform.position;
			
			transform.position = newPos;
			
			if (jumping)
			{
				Vector3 newPosfloor = newPos, oldPosfloor = oldPos; 
				newPosfloor.y = oldPosfloor.y = 0;
				jumpingPosition += (newPosfloor - oldPosfloor).magnitude;
				if (jumpingPosition >= jumpingDistance)
				{
					jumpingPosition = jumpingDistance;
					jumping = false;
				}
				else
				{
					float t = (jumpingPosition / jumpingDistance) * 2;
					if (t > 1)
						t = 1 - (t-1);
					Vector3 p = transform.position;
					p.y = t * jumpingHeight;
					transform.position = p;
				}
			}
			
			// Face front man
			Vector3 v = m_DivePath.m_NodePositions[finalNode]; //(note used by trails below)
			animatedPlayerModel.transform.LookAt (v);
			
			if (!animatedPlayerModel.animation.IsPlaying ("Ninja_01_Sword"))
				animatedPlayerModel.animation.Play("Ninja_01_Run");
	
			//trails
			if (m_NinjaMesh != null && m_TrailMaterial != null)
			{
				if (m_NinjaMesh.renderer != null)
				{					
					SkinnedMeshRenderer smr = (SkinnedMeshRenderer)m_NinjaMesh.GetComponent("SkinnedMeshRenderer");
					if (smr != null)
					{
						Mesh mesh = smr.sharedMesh; //assuming this doesn't memory leak?
						Matrix4x4 matrix = Matrix4x4.identity;
						Vector3 pos = m_NinjaMesh.transform.position;
						Vector3 dir = (v - m_NinjaMesh.transform.position).normalized;
						for (int i = 0; i < 16; i++)
						{
							pos = pos + dir * i * Time.deltaTime * 50;
							matrix.SetTRS (pos, m_NinjaMesh.transform.rotation, m_NinjaMesh.transform.lossyScale);
							Graphics.DrawMesh (mesh, matrix, m_TrailMaterial, 0);
						}
					}
				}
			}
		}
		
		//AttackFireCheck();
	}
	
	void DrawAimArrow (int maxbounces)
	{
		if (m_Aimarrow == null)
		{
			print ("Set AimArrow in inspector FFS");
			return;
		}
		m_Aimarrow.renderer.enabled = true;
		Vector3 ninjapos = transform.position;
		ninjapos.y = 1;
		Vector3 mousewpos = m_Controller.m_MouseWorldPos;
		mousewpos.y = 1;
		
		float arrowlength = Mathf.Min (m_MaxArrowLength, (ninjapos -  mousewpos).magnitude);
		
		RaycastHit hit;
		int layerMask = 1<<13; // Environment Collision"
		Vector3 v = ninjapos -  mousewpos;
		
		float scale = ninja_sphere_collider.transform.lossyScale.z * 10; // 10 being the plane size
		if (Physics.Raycast (ninjapos, v.normalized, out hit, arrowlength, layerMask))
		{
			arrowlength = hit.distance;	
		}
		
		m_Aimarrow.transform.LookAt (mousewpos);
		m_Aimarrow.transform.localScale = new Vector3 (1, 1, (arrowlength / scale)* 2);//*2 to pull back to thumb/cursor
		m_Aimarrow.transform.localPosition = new Vector3 (0, 1, 0);
		hud.SetMaxBounces (maxbounces);
	}
	
	public void ChangeState( PlayerNinjaState new_state )
	{
		m_StateTime = 0.0f;
		m_OldState = m_State;
		m_State = new_state;
	}
	
	void AttackFireCheck()
	{
		if( Input.GetMouseButtonUp (0) && hud.hudstate == HUD.HudState.PLAY
			&& hud.num_shuriken > 0)
		{
			Vector3 throwDirection = m_Controller.m_MouseWorldPos - transform.position;
			
			// Face front man
			animatedPlayerModel.transform.LookAt (m_Controller.m_MouseWorldPos);
			if (animatedPlayerModel != null && animatedPlayerModel.animation != null)
				animatedPlayerModel.animation.Play("Ninja_01_Throw");
		
			//if (throwDirection.magnitude > m_MaxSwordRange)
			{
				//	Fire a shuriken
				throwDirection.y = 0.0f;
				throwDirection.Normalize();
				
				GameObject shurikenObject = new GameObject();
				shurikenObject.name = "Shuriken " + gShurikenID.ToString();
				// shurikenObject.transform.parent = transform;
				
				Shuriken shuriken = shurikenObject.AddComponent<Shuriken>();
				shuriken.Initialise( transform.position, throwDirection, m_ShurikenSpeed );
				EPSoundController.Get().Play("ShurikenThrow");
				hud.UsedShuriken ();
			}
			//else swing sword
			{
				
			}
		}
	}
	
	public void Jump ()
	{
		if (!jumping)
		{
			jumping = true;
			//jumpingDistance = 200;
			//jumpingHeight = 100;
			jumpingPosition = 0;
		}
	}
		
	public void Die ()
	{
		ChangeState (PlayerNinjaState.DYING);
	}
	
	void updateDying ()
	{	
		if (animatedPlayerModel != null && animatedPlayerModel.animation != null)
		{
			animatedPlayerModel.animation.wrapMode = WrapMode.Once;
			if (!dying)
			{
				//Instantiate(m_BloodSplat, transform.position, transform.rotation);
				animatedPlayerModel.animation.Play("Ninja_01_Death");
				EPSoundController.Get().Play("Kill");
				dying = true;
			}
			if (!animatedPlayerModel.animation.IsPlaying("Ninja_01_Death"))
			{
				hud.Died ();
			}
		}
		else
		{
			EPSoundController.Get().Play("Kill");
			hud.Died ();
		}
	}
}
