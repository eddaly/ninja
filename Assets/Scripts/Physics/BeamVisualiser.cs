//-----------------------------------------------------------------------------
// File: BeamVisualiser.cs
//
// Desc:	Helper class for manipulating beams and showing various
//			debug information about them
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class BeamVisualiser : MonoBehaviour 
{
	public GameObject m_BeamRender;
	public int m_BeamID;

	bool m_initialised = false;
	
	bool m_Visible = true;

	public Vector2 m_Start;
	public Vector2 m_End;
	
	void Start()
	{	
	}
	
	void Update()
	{
		if( !m_initialised )
			return;
		
		EntityPhysics.Get().m_Internal.m_Lines[m_BeamID].start = m_Start;
		EntityPhysics.Get().m_Internal.m_Lines[m_BeamID].end = m_End;
		
		PrimitivePlane.StretchParticle( m_BeamRender, m_Start.x, m_Start.y, m_End.x, m_End.y, 1.0f );
	}
	
	public void Initialse( Texture texture, Vector3 start, Vector3 end )
	{
		Initialse( texture, start, end, 200000.0f );
	}
	public void Initialse( Texture texture, Vector2 start, Vector2 end, float strength )
	{
		const float beamScale = 1.0f;
		
		m_BeamRender = PrimitiveLibrary.Get.GetQuad();
		MeshRenderer renderer = m_BeamRender.GetComponent<MeshRenderer>();
		renderer.material.mainTexture = texture;
		
		Color beamColour = new Color( 0.1f, 0.1f, 0.1f, 1.0f );
		Color invisible = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
		
		if( m_Visible )
			renderer.material.SetColor( "_Color", beamColour );
		else
			renderer.material.SetColor( "_Color", invisible );
		
		Vector3 beamScaleVector = new Vector3( beamScale, beamScale, beamScale );
		m_BeamRender.transform.localScale = beamScaleVector;
			
		m_BeamID = EntityPhysics.Get().m_Internal.GetLine( start, end, strength );	
		m_Start = start;
		m_End = end;
		
		m_initialised = true;
	}
}
