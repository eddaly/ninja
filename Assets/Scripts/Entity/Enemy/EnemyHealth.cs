using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
	
	EnemyLookout enemy_lookout;
	public int health = 1;
	public Texture heart1, heart2, heart3;
	
	// Use this for initialization
	void Start () {
		enemy_lookout = transform.parent.GetComponentInChildren<EnemyLookout>();
		transform.parent = null; //bit hacky but don't want parent transform as is in viewport co-ords
		
		if (health > 3)
		{
			Debug.LogError ("Maximum health is 3");
			health = 3;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (enemy_lookout != null)
		{
			if (enemy_lookout.m_State != EnemyLookout.EnemyLookoutState.DEAD && 
				enemy_lookout.m_State != EnemyLookout.EnemyLookoutState.SLEEP)
			{
				Vector3 v = enemy_lookout.transform.position;
				v.z += 1;
				Vector3 v2 = Camera.main.WorldToViewportPoint (v);
				v2.z = 0; // Will sort beneath screen overlays
				transform.position = v2;
				if (health == 3)
				{
					guiTexture.texture = heart3;
					guiTexture.pixelInset = new Rect (0, 0, 90, 30);
				}
				else if (health == 2)
				{
					guiTexture.texture = heart2;
					guiTexture.pixelInset = new Rect (0, 0, 60, 30);
				}
				else// if (health == 1)
				{
					guiTexture.texture = heart1;
					guiTexture.pixelInset = new Rect (0, 0, 30, 30);
				}
				guiTexture.enabled = true;
			}
			else
			{
				guiTexture.enabled = false;
			}
		}
	}
}
