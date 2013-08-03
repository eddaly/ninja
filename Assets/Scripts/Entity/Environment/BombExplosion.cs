using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour {

	float minradius, maxradius, maxduration;
	ParticleSystem particlesys;

	// Use this for initialization
	void Start () {
		particlesys = transform.FindChild ("BombExplosionPS").particleSystem;
	}
	
	// Update is called once per frame
	void Update () {
		
		EnemyLookout[] enemies = FindObjectsOfType(typeof(EnemyLookout)) as EnemyLookout[];
        foreach (EnemyLookout enemy in enemies)
		{
			if( enemy.m_State != EnemyLookout.EnemyLookoutState.DEAD )
			{
				enemy.m_hasSeenPlayer = true;
			}
		}

		if (!particlesys.IsAlive ())
		{
			Destroy (this.gameObject);
			return;
		}
	}
	
	void OnTriggerStay (Collider other) {
		if (other.name == "EnemyRender")
		{
			EnemyLookout el = (EnemyLookout)other.gameObject.transform.parent.GetComponent<EnemyLookout>();
			el.Kill ();		
		}
	}
}
