using UnityEngine;
using System.Collections;

public class SleepingGas : MonoBehaviour {
	
	float timer = 0;
	float minradius, maxradius, maxduration;
	ParticleSystem particlesys;
	SphereCollider myCollider;

	// Use this for initialization
	void Start () {
		timer = 0;
		particlesys = transform.FindChild ("SleepingGasPS").particleSystem;
		myCollider = transform.GetComponent<SphereCollider>();
		minradius = myCollider.radius / 4;
		maxradius = myCollider.radius;
		maxduration = particlesys.duration * 2; // It lasts longer but by then it's fading
		
		Vector3 v = transform.position;
		v.y += 20; //*** hack as the enemies are floating so go over the volume
		transform.position = v;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (!particlesys.IsAlive ())
		{
			Destroy (this.gameObject);
			return;
		}
		if (timer < maxduration)
			myCollider.radius = Mathf.Max (minradius, (timer / maxduration) * maxradius);
		else
			myCollider.enabled = false;
	}
	
	void OnTriggerStay (Collider other) {
		if (other.name == "EnemyRender")
		{
			EnemyLookout el = (EnemyLookout)other.gameObject.transform.parent.GetComponent<EnemyLookout>();
			el.Sleep ();		
		}
	}
}
