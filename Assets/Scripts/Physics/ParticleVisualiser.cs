//-----------------------------------------------------------------------------
// File: ParticleVisualiser.cs
//
// Desc:	Helper class for manipulating physics particles and showing various
//			debug information about them
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ParticleVisualiser : MonoBehaviour 
{
	public GameObject m_ParticleRender;
	public int m_ParticleID;
	
	public bool m_Visible = false;
	
	bool m_initialised = false;
	
	void Start()
	{	
	}
	
	void Update()
	{
		if( !m_initialised )
			return;
		
		float x1 = EntityPhysics.Get().m_Internal.m_Particles[m_ParticleID].pos.x;
		float y1 = EntityPhysics.Get().m_Internal.m_Particles[m_ParticleID].pos.y;

		Vector3 newPos = new Vector3( x1, y1, -5.0f );
		m_ParticleRender.transform.position = newPos;
	}
	
	public void Initialse( Texture texture, float x_pos, float y_pos, 
							float x_vel, float y_vel, float radius, float repulsion )
	{
		float particleScale = radius*2.0f;
		
		m_ParticleRender = PrimitiveLibrary.Get.GetQuad();
		MeshRenderer renderer = m_ParticleRender.GetComponent<MeshRenderer>();
		renderer.material.mainTexture = texture;
		
		Color particleColour = new Color( 0.0f, 0.5f, 0.0f, 1.0f );
		Color invisible = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
		
		if( m_Visible )
			renderer.material.SetColor( "_Color", particleColour );
		else
			renderer.material.SetColor( "_Color", invisible );
		
		Vector3 particleScaleVector = new Vector3( particleScale, particleScale, particleScale );
		m_ParticleRender.transform.localScale = particleScaleVector;
			
		m_ParticleID = EntityPhysics.Get().GetPhysicsParticle( x_pos, y_pos, x_vel, y_vel, radius, 0.975f, repulsion );				
		
		m_initialised = true;
	}
}
