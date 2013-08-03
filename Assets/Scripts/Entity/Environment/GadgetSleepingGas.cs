using UnityEngine;
using System.Collections;

public class GadgetSleepingGas : MonoBehaviour {
	
	//GameObject m_playerObject;
	//PlayerNinja m_playerNinja;
	public GameObject m_SleepingGas;
	public Countdown countdown;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (countdown.IsFinished())
		{
			Quaternion q = Quaternion.identity;
			Instantiate(m_SleepingGas, transform.position,  q);
			Destroy (countdown.gameObject);
			Destroy( this.gameObject );
		}
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.name == "Ninja" )
		{
			countdown.Begin ();
		}
	}
}
