using UnityEngine;
using System.Collections;

public class EnemyAlert : MonoBehaviour {
	
	EnemyLookout enemy_lookout;
	
	// Use this for initialization
	void Start () {
		enemy_lookout = transform.parent.GetComponentInChildren<EnemyLookout>();
		transform.parent = null; //bit hacky but don't want parent transform as is in viewport co-ords
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (enemy_lookout != null)
		{
			if (enemy_lookout.m_hasSeenPlayer && enemy_lookout.m_State != EnemyLookout.EnemyLookoutState.DEAD &&
				enemy_lookout.m_State != EnemyLookout.EnemyLookoutState.SLEEP)
			{
				Vector3 v = enemy_lookout.transform.position;
				v.x -= 10;
				transform.position = Camera.main.WorldToViewportPoint (v);
				guiTexture.enabled = true;
			}
			else
			{
				guiTexture.enabled = false;
			}
		}
	}
}
