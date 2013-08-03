//-----------------------------------------------------------------------------
// File: Shuriken.cs
//
// Desc:	Enemy with a field of vision that can spot the player
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class Shuriken : MonoBehaviour 
{
	public GameObject m_ShurikenQuad = null;

	public enum ShurikenState
	{
		FLYING = 0,
		EMBEDDED,
		
		NUM
	}
	public ShurikenState m_State = ShurikenState.FLYING;
	public ShurikenState m_OldState = ShurikenState.FLYING;
	public float m_StateTime;
	
	
	public Vector3 m_Direction;
	public Vector3 m_Velocity;
	
	public void Initialise( Vector3 start_pos, Vector3 dir, float speed )
	{
		m_ShurikenQuad = PrimitiveLibrary.Get.GetQuad();		
		
		MeshRenderer renderer = m_ShurikenQuad.GetComponent<MeshRenderer>();
		renderer.material.mainTexture = TextureLibraryInGame.GetTexture( TextureLibraryInGame.Tex.SHURIKEN );

		renderer.material.SetColor( "_Color", Color.white );
		if (Application.platform == RuntimePlatform.Android)
			renderer.material.shader = Shader.Find ("Ninja/UnlitAlpha");///Transparent");
		else 
			renderer.material.shader = Shader.Find ("Unlit/Transparent");
		
		transform.position = start_pos;
		m_ShurikenQuad.transform.position = transform.position;
		
		Vector3 scale = new Vector3( NinjaDefs.kShurikenSize, NinjaDefs.kShurikenSize, 0.0f );
		m_ShurikenQuad.transform.localScale = scale;		
		
		m_ShurikenQuad.transform.localEulerAngles = new Vector3( 90.0f, 0.0f, 0.0f );	
		
		m_Direction = dir;
		m_Velocity = dir*speed;
	}
	
	void Update()
	{	
		m_StateTime += MainLoop.Get().m_FrameTime;
		
		switch( m_State )
		{
		default:
		case ShurikenState.FLYING:
			updateFlying();
			break;
		case ShurikenState.EMBEDDED:
			updateEmbedded();
			break;
		}
		
		m_ShurikenQuad.transform.position = transform.position;
	}
	
	void updateFlying()
	{
		//	Test to see if we're embedded in a wall
		Vector3 frameMovement = m_Velocity*MainLoop.Get().m_FrameTime;
		float frameMoveDist = frameMovement.magnitude;
		
		RaycastHit hit;
		int layerMask = (1<<LayerMask.NameToLayer ("EnemyCollision")) | (1<<LayerMask.NameToLayer ("Ignore Raycast"));
		layerMask = ~layerMask;
		if( Physics.Raycast( transform.position - m_Direction, m_Direction, out hit, frameMoveDist + 1.0f, layerMask  ) ) 
		{
			//	We collided, so embed the shuriken
			Vector3 embedPos = hit.point;
			embedPos.y = 3.0f;
			transform.position = embedPos;
			
			ChangeState( ShurikenState.EMBEDDED );
		}
		else
		{
			//	No collision, so keep flying
			transform.position += frameMovement;
		}		
		
		Vector3 angle = new Vector3( 90.0f, m_StateTime*500.0f, 0.0f );
		m_ShurikenQuad.transform.localEulerAngles = angle;

		enemyCollide();
	}
	
	void updateEmbedded()
	{
		if( m_StateTime > 10.0f )
		{
			PrimitiveLibrary.Get.ReleaseQuad( m_ShurikenQuad );
			Destroy( this.gameObject );
		}
	}
	
	void ChangeState( ShurikenState new_state )
	{
		m_StateTime = 0.0f;
		m_OldState = m_State;
		m_State = new_state;
	}
	
	void enemyCollide()
	{
		int numEnemies = ReferenceLibraryInGame.GetReferenceListLength( ReferenceLibraryInGame.Ref.ENEMY_LOOKOUT );
		
		for( int e = 0; e<numEnemies; e++ )
		{
			GameObject thisEnemy = ReferenceLibraryInGame.GetReference( ReferenceLibraryInGame.Ref.ENEMY_LOOKOUT, e );
			
			Vector3 diff = transform.position - thisEnemy.transform.position;
			float dist = diff.magnitude;
			//Debug.Log (dist);
			
			if( dist < NinjaDefs.kShurikenSize )
			{
				//	Hit the enemy and the shuriken
				EnemyLookout enemy = thisEnemy.GetComponent<EnemyLookout>();
				if (enemy.m_State != EnemyLookout.EnemyLookoutState.DEAD)
				{
					enemy.Hit();
					
					PrimitiveLibrary.Get.ReleaseQuad( m_ShurikenQuad );
					Destroy( this.gameObject );

				}
			}
		}
	}
}
