//-----------------------------------------------------------------------------
// File: MainLoop.cs
//
// Desc:	Main loop for Ninja game
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class MainLoop : MonoBehaviour 
{
	//	Static accessor
	static MainLoop s_MainLoop = null;
    public static MainLoop Get()
    {
		//	Cache the object to avoid frequent string lookups
		if( s_MainLoop != null )
			return s_MainLoop;

		GameObject mainLoopObject = GameObject.Find( "MainLoop" );
        if( mainLoopObject == null )
        {
            Debug.Log( "!** No main loop found (MainLoop)" );
        }
        else
        {
            s_MainLoop = (MainLoop)mainLoopObject.GetComponent( typeof(MainLoop) );
        }

        return s_MainLoop;
    }
	
	
	//	Global time to be used by all objects
	public float m_GameTime = 0.0f;
	public float m_FrameTime = 0.0f;
	
	//	For now, only one type of game state
	public InGameLoop m_GameState = null;

	
	void Start()
	{
		PrimitiveLibrary.Get.Poke();		
		
		GameObject gameStateObject = new GameObject( "InGameState" );
		gameStateObject.transform.parent = transform;
		gameStateObject.transform.localPosition = Vector3.zero;
		
		m_GameState = gameStateObject.AddComponent<InGameLoop>();
	}
	
	void Update()
	{
		m_FrameTime = Time.deltaTime;
		if( m_FrameTime > 1.0f/30.0f )
			m_FrameTime = 1.0f/30.0f;
		
		m_GameTime += m_FrameTime;
	}
	
	public void PlayerInput( NinjaDefs.InputButton button, float x_pos, float y_pos )
	{
		m_GameState.PlayerInput( button, x_pos, y_pos );		
	}
}
