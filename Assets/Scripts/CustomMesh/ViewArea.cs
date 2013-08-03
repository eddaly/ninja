//-----------------------------------------------------------------------------
// File: ViewArea.cs
//
// Desc:	Creates a custom m_Mesh to highlight the parts of a level that the
//			enemy can see
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ViewArea : MonoBehaviour
{ 
	Mesh m_Mesh;
	MeshRenderer m_renderer;
	
	//	We keep private copies of the number of sections because once the mesh is
	//	created we don't alter the number of vertices in the mesh but we need to
	//	know how many sections the mesh was created with
	public int m_XSections = 10;
	int m_xSections;
	public int m_ZSections = 10;
	int m_zSections;	

	float m_time;
	
	public float m_ViewAngle = 60.0f;
	
	void Start()
	{
		MakePlane( 200.0f, 200.0f, m_XSections, m_ZSections );
	}
	
	void Update()
	{
		m_time += MainLoop.Get().m_FrameTime/2;
		ChangeAreaShape( 200.0f, m_ViewAngle );		
	}
	
	void MakePlane( float scale_x, float scale_z, int sections_x, int sections_z )
	{
		m_xSections = sections_x;
		m_zSections = sections_z;
		
		//	Create the mesh component and it's renderer
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		m_renderer = gameObject.AddComponent<MeshRenderer>();

		if( meshFilter.sharedMesh == null )
		{
			meshFilter.sharedMesh = new Mesh();
		}
		m_Mesh = meshFilter.sharedMesh;
		m_Mesh.Clear();
		
		int numVertsX = m_xSections + 1;
		int numVertsZ = m_zSections + 1;
		
		int numVerts = numVertsX*numVertsZ;
		int numIndices = sections_x*m_zSections*6;

		//	Declare arrays for the vertices
		Vector3[] myVertices = new Vector3[numVerts];
		Vector2[] myUVs = new Vector2[numVerts];
		Color[] myColours = new Color[numVerts];
		int[] myTris = new int[numIndices];
		
		float xPos = -scale_x*0.5f;
		float zPos = -scale_z*0.5f;
		float dX = scale_x/(float)sections_x;
		float dZ = scale_z/(float)m_zSections;
		
		float uPos = 0.0f;
		float vPos = 0.0f;
		float dU = 1.0f/(float)sections_x;
		float dV = 1.0f/(float)m_zSections;		
		
		int vertIdx = 0;
		int triIdx = 0;
		
		for( int zv = 0; zv<numVertsZ; ++zv )
		{
			for( int xv = 0; xv<numVertsX; ++xv )
			{
				myVertices[vertIdx] = new Vector3( xPos, 0.0f, zPos );
				myUVs[vertIdx] = new Vector2( uPos, vPos );
				myColours[vertIdx] = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
				
				xPos += dX;				
				uPos += dU;

				//	Set the triangle indices
				if( xv < sections_x && zv < m_zSections )
				{
					myTris[triIdx++] 	= vertIdx;
					myTris[triIdx++] 	= vertIdx + numVertsX;
					myTris[triIdx++] 	= vertIdx + 1;

					myTris[triIdx++]	= vertIdx + 1;
					myTris[triIdx++] 	= vertIdx + numVertsX;
					myTris[triIdx++] 	= vertIdx + numVertsX + 1;
				}
				
				vertIdx++;
			}

			xPos = -scale_x*0.5f;
			uPos = 0.0f;
			
			vPos += dV;
			zPos += dZ;
		}

		//	Apply the verts to make the plane
		m_Mesh.vertices = myVertices;
		m_Mesh.uv = myUVs;
		m_Mesh.colors = myColours;
		m_Mesh.triangles = myTris;
		
		m_Mesh.RecalculateNormals();
		m_Mesh.RecalculateBounds();
 
		//	Create a unique material with the alpha shader
		Material newMaterial = new Material( Shader.Find( "Ninja/ParticlesAdditiveOverlay"));//other shader doesn't work (neither pick up material alpha)
		m_renderer.sharedMaterial = newMaterial;
		m_renderer.sharedMaterial.color = new Color( 1.0f, .0f, .0f, .5f );
		m_renderer.sharedMaterial.mainTexture = TextureLibraryInGame.GetTexture( TextureLibraryInGame.Tex.ENEMY_VIEW_AREA );
	}
	
	void ChangeAreaShape( float length, float angle )
	{
		float angleRad = angle*Mathf.Deg2Rad;
		
		int numVertsX = m_xSections + 1;
		int numVertsZ = m_zSections + 1;
		
		int numVerts = numVertsX*numVertsZ;

		//	We're only modifying vert position and uv co-ordinates for now
		Vector3[] myVertices = new Vector3[numVerts];
		Vector2[] myUVs = new Vector2[numVerts];
		
		float uPos = 0.0f;
		float vPos = -m_time;
		float dU = 1.0f/(float)m_xSections;
		float dV = 1.0f/(float)m_zSections;		
		
		int vertIdx = 0;
		
		for( int zv = 0; zv<numVertsZ; ++zv )
		{
			//	The verts are now positioned as a radiating circle section
			float proportion = (float)zv/((float)numVertsZ - 1.0f);			
			float radius = proportion*length;

			float currentAngle = -(angleRad*0.5f);
			float dAngle = angleRad/(float)m_xSections;
			
			for( int xv = 0; xv<numVertsX; ++xv )
			{
				float xPos = radius*Mathf.Cos( currentAngle );
				float zPos = radius*Mathf.Sin( currentAngle );
				
				myVertices[vertIdx] = new Vector3( xPos, 0.0f, zPos );
				myUVs[vertIdx] = new Vector2( uPos, vPos );

				currentAngle += dAngle;
				uPos += dU;
				
				vertIdx++;
			}
			
			uPos = 0.0f;			
			vPos += dV;
		}

		//	Apply the verts to alter the mesh
		m_Mesh.vertices = myVertices;
		m_Mesh.uv = myUVs;
		
		m_Mesh.RecalculateBounds();
	}
}

