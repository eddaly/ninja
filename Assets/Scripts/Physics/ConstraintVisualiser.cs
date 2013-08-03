//-----------------------------------------------------------------------------
// File: ConstraintVisualiser.cs
//
// Desc:	Helper class for manipulating constraints and showing various
//			debug information about them
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ConstraintVisualiser : MonoBehaviour 
{
	public GameObject m_ConstraintRender;
	public int m_ConstraintID;

	bool m_initialised = false;
	
	bool m_Visible = false;
	
	void Start()
	{	
	}
	
	void Update()
	{
		if( !m_initialised )
			return;
		
		int p1 = EntityPhysics.Get().m_Internal.m_Constraints[m_ConstraintID].particle1;
		int p2 = EntityPhysics.Get().m_Internal.m_Constraints[m_ConstraintID].particle2;
		
		float x1 = EntityPhysics.Get().m_Internal.m_Particles[p1].pos.x;
		float y1 = EntityPhysics.Get().m_Internal.m_Particles[p1].pos.y;
		float x2 = EntityPhysics.Get().m_Internal.m_Particles[p2].pos.x;
		float y2 = EntityPhysics.Get().m_Internal.m_Particles[p2].pos.y;

		PrimitivePlane.StretchParticle( m_ConstraintRender, x1, y1, x2, y2, 1.0f );
	}
	
	public void Initialse( Texture texture, int particle1, int particle2 )
	{
		Initialse( texture, particle1, particle2, -1.0f, 1, 0.1f );
	}
	public void Initialse( Texture texture, int particle1, int particle2, float length )
	{
		Initialse( texture, particle1, particle2, length, 1, 0.1f );
	}
	public void Initialse( Texture texture, int particle1, int particle2, 
				float length, int iterations, float strength )
	{
		const float constraintScale = 1.0f;
		
		m_ConstraintRender = PrimitiveLibrary.Get.GetQuad();
		MeshRenderer renderer = m_ConstraintRender.GetComponent<MeshRenderer>();
		renderer.material.mainTexture = texture;
		
		Color constraintColour = new Color( 0.1f, 0.1f, 0.1f, 1.0f );
		Color invisible = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
		
		if( m_Visible )
			renderer.material.SetColor( "_Color", constraintColour );
		else
			renderer.material.SetColor( "_Color", invisible );
		
		Vector3 constraintScaleVector = new Vector3( constraintScale, constraintScale, constraintScale );
		m_ConstraintRender.transform.localScale = constraintScaleVector;
			
		m_ConstraintID = EntityPhysics.Get().GetConstraint( particle1, particle2, length, 1, 0.0075f );				
		
		m_initialised = true;
	}
}
