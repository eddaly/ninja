//-----------------------------------------------------------------------------
// File: PlayerController.cs
//
// Desc:	Recieves and uses player input
//
// Copyright Echo Peak 2012
//-----------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public GameObject m_Crosshair = null;
	public GameObject m_PlaceSpring = null;
	public GameObject m_PlaceSleepingGas = null;
	public GameObject m_PlaceBomb = null;
	
	public Vector3 m_MousePos;
	public Vector3 m_MouseWorldPos;
	public bool m_FireDown;
	bool m_oldFireDown;
	public bool m_FirePressed;
	public bool m_FireReleased;
	
	public float m_MaxXPosition = 1024.0f;
	public float m_MaxZPosition = 768.0f;
	
	void Start() 
	{
		if (Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			m_Crosshair.renderer.enabled = false;
			Screen.orientation = ScreenOrientation.Landscape;
		}
		if (m_PlaceBomb != null )
			m_PlaceBomb.renderer.enabled = false;
		if (m_PlaceSpring != null )
			m_PlaceSpring.renderer.enabled = false;
		if (m_PlaceSleepingGas != null )
			m_PlaceSleepingGas.renderer.enabled = false;
	}
	
	public void UsePlaceBomb ( bool val)
	{
		if (m_PlaceBomb != null )
		{
			m_PlaceBomb.renderer.enabled = val;
		}
		if (Application.platform != RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer) {
			if (val)
				m_Crosshair.renderer.enabled = false;
		}
	}

	public void UsePlaceSpring ( bool val)
	{
		if (m_PlaceSpring != null )
		{
			m_PlaceSpring.renderer.enabled = val;
		}
		if (Application.platform != RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer) {
			if (val)
				m_Crosshair.renderer.enabled = false;
		}
	}
	
	public void UsePlaceSleepingGas ( bool val)
	{
		if (m_PlaceSleepingGas != null )
		{
			m_PlaceSleepingGas.renderer.enabled = val;
		}
		if (Application.platform != RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer) {
			if (val)
				m_Crosshair.renderer.enabled = false;
		}
	}
	
	public void UseCursor ( bool val )
	{
		if (Application.platform != RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer)
			m_Crosshair.renderer.enabled = val;
	}
	
	void Update()
	{	
		if( m_Crosshair == null )
			return;
		
		 if (Input.GetKeyDown(KeyCode.Escape)) //note, needed for android
   			Application.Quit(); 
		
		m_MousePos = Input.mousePosition;
		m_MousePos.z = Camera.main.transform.position.y;
		
		m_MouseWorldPos = Camera.main.ScreenToWorldPoint (m_MousePos);
		m_Crosshair.transform.position = new Vector3( m_MouseWorldPos.x, 5.0f, m_MouseWorldPos.z );
		m_PlaceSpring.transform.position = new Vector3( m_MouseWorldPos.x, 5.0f, m_MouseWorldPos.z );
		m_PlaceSleepingGas.transform.position = new Vector3( m_MouseWorldPos.x, 5.0f, m_MouseWorldPos.z );
		m_PlaceBomb.transform.position = new Vector3( m_MouseWorldPos.x, 5.0f, m_MouseWorldPos.z );
		
		m_FireDown = Input.GetMouseButton( 0 );
		
		if( !m_oldFireDown && m_FireDown )
			m_FirePressed = true;
		else
			m_FirePressed = false;
		
		if( m_oldFireDown && !m_FireDown )
			m_FireReleased = true;
		else
			m_FireReleased = false;
		
		m_oldFireDown = m_FireDown;
	}
}
