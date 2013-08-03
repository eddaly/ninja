//-----------------------------------------------------------------------------
// File: PrimitiveLibrary.h
//
// Desc:	Singleton class which manages pools of standard primitives created
//			at runtime for other objects to use as renderers.
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public sealed class PrimitiveLibrary
{
	//	A static instance is only instantiated when first accessed at runtime
	static readonly PrimitiveLibrary instance = new PrimitiveLibrary();

	public static PrimitiveLibrary Get
	{
		get
		{
			 return instance;
		}
	}

	
	//	Used to construct unique names to help debugging
	static int m_uniqueIDNumber = 0;

	//	Objects used to organise the library pools in the scene hierarchy
	Transform m_HierarchyTopLevel;
	Transform m_HierarchyQuadPool;
	Transform m_HierarchyRingPool;
	Transform m_HierarchyTrailPool;

	//	The quad pool. An array of GameObjects with the quads attached as
	//	MeshFilters, and an array of bools to record whether they're being used.
	const int gPL_NumQuads = 250;
	GameObject[] m_quadPool = new GameObject[gPL_NumQuads];
	int m_quadPoolCursor;

	//	The ring pool. Same as the quad pool, but creates flat rings on the z-plane
	//	as tubes of zero length. Used mainly for halos and action fx
	const int gPL_NumRings = 5;
	GameObject[] m_ringPool = new GameObject[gPL_NumRings];
	int m_ringPoolCursor;

	const int gPL_NumTrails = 10;
	GameObject[] m_trailPool = new GameObject[gPL_NumTrails];
	int m_trailPoolCursor;

	public void ReclaimPrimitivesInPool()
	{
		for( int p = 0; p<gPL_NumQuads; ++p )
		{
			Transform transform = m_quadPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyQuadPool;
			m_quadPool[p].SetActive (false);
		}		

		for( int p = 0; p<gPL_NumRings; ++p )
		{
			Transform transform = m_ringPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyRingPool;
			m_ringPool[p].SetActive (false);
		}		

		for( int p = 0; p<gPL_NumTrails; ++p )
		{
			Transform transform = m_trailPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyTrailPool;
			m_trailPool[p].SetActive (false);
		}		
	}
	
	
	//-----------------------------------------------------------------------------
	// Constructor
	//
	// Desc:	Creates the pools of primitives that the library will manage
	PrimitiveLibrary()
	{
		//	Create containing objects for scene tidyness, and to aid debugging.
		//	Most of the time the primitives, once taken, will be moved into 
		//	a local hierarchy, but they exist in 'library space' by default,
		//	which is at the world center.
		GameObject topLevel = new GameObject();
		topLevel.name = "PrimitiveLibrary";
		GameObject.DontDestroyOnLoad(topLevel);
		m_HierarchyTopLevel = topLevel.GetComponent<Transform>();

		GameObject quadPool = new GameObject();
		quadPool.name = "QuadPool";
		m_HierarchyQuadPool = quadPool.GetComponent<Transform>();
		m_HierarchyQuadPool.parent = m_HierarchyTopLevel;

		GameObject ringPool = new GameObject();
		ringPool.name = "RingPool";
		m_HierarchyRingPool = ringPool.GetComponent<Transform>();
		m_HierarchyRingPool.parent = m_HierarchyTopLevel;

		GameObject trailPool = new GameObject();
		trailPool.name = "TrailPool";
		m_HierarchyTrailPool = trailPool.GetComponent<Transform>();
		m_HierarchyTrailPool.parent = m_HierarchyTopLevel;
		
		//	Set up the quad pool
		for( int p = 0; p<gPL_NumQuads; ++p )
		{
			m_quadPool[p] = PrimitivePlane.MakePlane( 1.0f, 1.0f, 1, 1 );
			GameObject.DontDestroyOnLoad( m_quadPool[p] );
	
			m_quadPool[p].SetActive (false);

			//	Force the name to QUADPOOL for debugging
			m_quadPool[p].name = "Primitive" + (m_uniqueIDNumber++).ToString() + " [QUADPOOL]";

			Transform transform = m_quadPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyQuadPool;
			Vector3 newPos = new Vector3( 0.0f, 0.0f, 0.0f );
			transform.localPosition = newPos;

			MeshRenderer renderer = m_quadPool[p].GetComponent<MeshRenderer>();
			renderer.material.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		}

		m_quadPoolCursor = 0;
		
		//	Set up the ring pool
		for( int p = 0; p<gPL_NumRings; ++p )
		{
			m_ringPool[p] = PrimitiveTube.MakeTube( 30, 1, 1.0f, 1.1f, 0.0f );
			GameObject.DontDestroyOnLoad( m_ringPool[p] );
			m_ringPool[p].SetActive (false);

			//	Force the name to RINGPOOL for debugging
			m_ringPool[p].name = "Primitive" + (m_uniqueIDNumber++).ToString() + " [RINGPOOL]";

			Transform transform = m_ringPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyRingPool;
			Vector3 newPos = new Vector3( 0.0f, 0.0f, 0.0f );
			transform.localPosition = newPos;

			MeshRenderer renderer = m_ringPool[p].GetComponent<MeshRenderer>();
			renderer.material.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		}

		m_ringPoolCursor = 0;	

		for( int p = 0; p<gPL_NumTrails; ++p )
		{
			m_trailPool[p] = PrimitiveTrail.MakeTrail( 20, 0.5f, 5.0f, 1.0f );
			GameObject.DontDestroyOnLoad( m_trailPool[p] );
			m_trailPool[p].SetActive (false);
			
			//	Force the name to TRAILPOOL for debugging
			m_trailPool[p].name = "Primitive" + (m_uniqueIDNumber++).ToString() + " [TRAILPOOL]";

			Transform transform = m_trailPool[p].GetComponent<Transform>();
			transform.parent = m_HierarchyTrailPool;
			Vector3 newPos = new Vector3( 0.0f, 0.0f, 0.0f );
			transform.localPosition = newPos;

			MeshRenderer renderer = m_trailPool[p].GetComponent<MeshRenderer>();
			renderer.material.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		}

		m_trailPoolCursor = 0;	
	}

	//-----------------------------------------------------------------------------
	// Method:	Poke()
	// Desc:	Used to poke the singleton awake
	public void Poke() {}

	//-----------------------------------------------------------------------------
	// Method:	GetQuad()
	// Desc:	Find an inactive quad in the pool and return it.
	// Note:	The quads shouldn't have their geometry altered - if you need to
	//			do that then create your own quad with Primitivequad.Makequad()
	//			Use the transform to alter position and size.
	//
	// Returns:	A GameObject with the quad attached as a MeshFilter, or null if
	//			there are no inactive quads	left in the pool.
	public GameObject GetQuad()
	{
		int currentCursor = m_quadPoolCursor++;
		if( m_quadPoolCursor == gPL_NumQuads )
			m_quadPoolCursor = 0;

		while( m_quadPool[m_quadPoolCursor].activeInHierarchy && m_quadPoolCursor != currentCursor )
		{
			m_quadPoolCursor++;
			if( m_quadPoolCursor == gPL_NumQuads )
				m_quadPoolCursor = 0;
		}

		if( currentCursor == m_quadPoolCursor )
			return null;
		else
		{
			m_quadPool[m_quadPoolCursor].SetActive(true);
			
			PrimitivePlane.ModifyUVs( m_quadPool[m_quadPoolCursor], 0.0f, 0.0f, 1.0f, 1.0f );
			
			return m_quadPool[m_quadPoolCursor];
		}
	}

	//-----------------------------------------------------------------------------
	// Method:	ReleaseQuad()
	// Desc:	Returns the quad back to the library pool
	// Note:	The calling object should set their own reference to null to avoid
	//			conflicts
	public void ReleaseQuad( GameObject quad_object )
	{
		if( quad_object == null )
		{
			Debug.Log( "!** Trying to release null quad" );
			return;
		}
			
		quad_object.SetActive(false);

		//	Put the quad object back in our hierarchy
		Vector3 hideQuad = new Vector3( -1000.0f, 0.0f, 0.0f );
		Transform transform = quad_object.GetComponent<Transform>();
		transform.parent = m_HierarchyQuadPool;
		transform.localPosition = hideQuad;
		
		Vector3 zeroRotation = new Vector3( 0.0f, 0.0f, 0.0f );
		transform.localEulerAngles = zeroRotation;
		
		Vector3 unitScale = new Vector3( 1.0f, 1.0f, 1.0f );
		transform.localScale = unitScale;
		
		MeshRenderer quadRenderer = quad_object.GetComponent<MeshRenderer>();
		Color glowColour = new Color( 0.0f, 0.0f, 0.0f, 0.0f );
		quadRenderer.material.SetColor( "_Emissive", glowColour );
		
		PrimitivePlaneInfo info = quad_object.GetComponent<PrimitivePlaneInfo>();
		info.m_TexUTile = 1.0f;
		info.m_TexVTile = 1.0f;
		info.m_TexUOffset = 0.0f;
		info.m_TexVOffset = 0.0f;
		
	}
	

	//-----------------------------------------------------------------------------
	// Method:	GetRing()
	// Desc:	Find an inactive ring in the pool and return it.
	// Note:	Use PrimitiveTube.ModifyVerts() to change shape, thickness, etc,
	//			but only use transform.localPosition to set it's translation.
	//
	// Returns:	A GameObject with the ring attached as a MeshFilter, or null if
	//			there are no inactive ring left in the pool.
	public GameObject GetRing()
	{
		int currentCursor = m_ringPoolCursor++;
		if( m_ringPoolCursor == gPL_NumRings )
			m_ringPoolCursor = 0;

		while( m_ringPool[m_ringPoolCursor].activeInHierarchy && m_ringPoolCursor != currentCursor )
		{
			m_ringPoolCursor++;
			if( m_ringPoolCursor == gPL_NumRings )
				m_ringPoolCursor = 0;
		}

		if( currentCursor == m_ringPoolCursor )
			return null;
		else
		{
			m_ringPool[m_ringPoolCursor].SetActive (true);
			return m_ringPool[m_ringPoolCursor];
		}
	}
		
	//-----------------------------------------------------------------------------
	// Method:	ReleaseRing()
	// Desc:	Returns the ring back to the library pool
	// Note:	The calling object should set their own reference to null to avoid
	//			conflicts
	public void ReleaseRing( GameObject ring_object )
	{
		if( ring_object == null )
		{
			Debug.Log( "!** Trying to release null ring" );
			return;
		}
		
		ring_object.SetActive(false);

		//	Put the ring object back in our hierarchy
		Vector3 hideRing = new Vector3( -1000.0f, 0.0f, 0.0f );
		Transform transform = ring_object.GetComponent<Transform>();
		transform.parent = m_HierarchyRingPool;
		transform.localPosition = hideRing;
		
		Vector3 unitScale = new Vector3( 1.0f, 1.0f, 1.0f );
		transform.localScale = unitScale;
	}


	public GameObject GetTrail()
	{
		int currentCursor = m_trailPoolCursor++;
		if( m_trailPoolCursor == gPL_NumTrails )
			m_trailPoolCursor = 0;

		while( m_trailPool[m_trailPoolCursor].activeInHierarchy && m_trailPoolCursor != currentCursor )
		{
			m_trailPoolCursor++;
			if( m_trailPoolCursor == gPL_NumTrails )
				m_trailPoolCursor = 0;
		}

		if( currentCursor == m_trailPoolCursor )
			return null;
		else
		{
			m_trailPool[m_trailPoolCursor].SetActive(true);
			return m_trailPool[m_trailPoolCursor];
		}
	}

	public void ReleaseTrail( GameObject trail_object )
	{
		if( trail_object == null )
		{
			Debug.Log( "!** Trying to release null trail" );
			return;
		}
		
		trail_object.SetActive(false);

		//	Put the ring object back in our hierarchy
		Vector3 hideTrail = new Vector3( -1000.0f, 0.0f, 0.0f );
		Transform transform = trail_object.GetComponent<Transform>();
		transform.parent = m_HierarchyTrailPool;
		transform.localPosition = hideTrail;
		
		Vector3 unitScale = new Vector3( 1.0f, 1.0f, 1.0f );
		transform.localScale = unitScale;
	}
}
