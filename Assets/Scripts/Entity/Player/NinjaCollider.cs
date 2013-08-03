using UnityEngine;
using System.Collections;

public class NinjaCollider : MonoBehaviour {
	
	PlayerNinja pn = null;
	
	// Use this for initialization
	void Start () {
		pn = transform.parent.GetComponentInChildren<PlayerNinja>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (LayerMask.LayerToName (other.gameObject.layer) == "EnemyCollision")
		{
			if(pn != null)
			{
				//pn.ChangeState( PlayerNinja.PlayerNinjaState.IDLE );
				//screw that, kill 'im
				other.transform.parent.GetComponent<EnemyLookout>().Hit();
				pn.animatedPlayerModel.animation["Ninja_01_Sword"].speed = 1.5f;
				pn.animatedPlayerModel.animation.Play("Ninja_01_Sword");
			}
		}
	}
}
