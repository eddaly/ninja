using UnityEngine;
using System.Collections;

public class EnemyBig : EnemyLookout
{
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		enemyHealth.health = 3;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		// Didn't end up doing any more but left for now in case
		base.Update();
	}
	
	public override void Hit ()
	{
		base.Hit();
	}
}

