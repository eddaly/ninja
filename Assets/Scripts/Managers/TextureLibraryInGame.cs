// File: InGametextureLibrary.cs
//
// Desc:	Holder for the in-game wTexget textures.
//
// Note:	This class is an "instantiated singleton" class. This means that
//			it must have an instantiated GameObject within the scene, but
//			that it will only be accessed through static members, as a
//			pseudo-singleton.
//
// Copyright Team WGC 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class TextureLibraryInGame : MonoBehaviour 
{
	public enum Tex
	{
		BLAST_AREA,
		ENEMY,
		SHURIKEN,
		ENEMY_VIEW_AREA,
		OUTOFTURNS,
		LEVELCOMPLETE,
		STORE,
		DIED,
		START
	}
	
	public Texture m_BlastArea;
	public Texture m_Enemy;
	public Texture m_Shuriken;
	public Texture m_EnemyViewArea;
	public Texture m_OutOfTurns;
	public Texture m_LevelComplete;
	public Texture m_Store;
	public Texture m_Died;
	public Texture m_Start;
	
	static TextureLibraryInGame m_textureLibrary = null;
	
	//-----------------------------------------------------------------------------
	// Method:	GetTexture()
	// Desc:	Returns the specified texture.
	static public Texture GetTexture( Tex texture )
	{
		return GetTexture ( texture, 0 );
	}
	static public Texture GetTexture( Tex texture, int tex_index )
	{
		if( m_textureLibrary == null )
		{
			GameObject libraryObject = GameObject.Find("TextureLibrary");
	        if( libraryObject == null )
	        {
	            Debug.Log( "**! No In-game texture library object found (TextureLibrary)" );
				return null;
	        }
	        else
	        {
	            m_textureLibrary = (TextureLibraryInGame)libraryObject.GetComponent( typeof(TextureLibraryInGame) );
	        }
			
			if( !m_textureLibrary )
				return null;
		}
		
		Texture returnTexture = null;
		
		switch( texture )
		{
		default:
		case TextureLibraryInGame.Tex.BLAST_AREA:
			returnTexture = m_textureLibrary.m_BlastArea;
			break;
		case TextureLibraryInGame.Tex.ENEMY:
			returnTexture = m_textureLibrary.m_Enemy;
			break;
		case TextureLibraryInGame.Tex.SHURIKEN:
			returnTexture = m_textureLibrary.m_Shuriken;
			break;
		case TextureLibraryInGame.Tex.ENEMY_VIEW_AREA:
			returnTexture = m_textureLibrary.m_EnemyViewArea;
			break;
		case TextureLibraryInGame.Tex.OUTOFTURNS:
			returnTexture = m_textureLibrary.m_OutOfTurns;
			break;
		case TextureLibraryInGame.Tex.LEVELCOMPLETE:
			returnTexture = m_textureLibrary.m_LevelComplete;
			break;
		case TextureLibraryInGame.Tex.STORE:
			returnTexture = m_textureLibrary.m_Store;
			break;
		case TextureLibraryInGame.Tex.DIED:
			returnTexture = m_textureLibrary.m_Died;
			break;
		case TextureLibraryInGame.Tex.START:
			returnTexture = m_textureLibrary.m_Start;
			break;
		}			
			
		return returnTexture;
	}
}
