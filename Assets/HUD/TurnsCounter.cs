using UnityEngine;
using System.Collections;

public class TurnsCounter : MonoBehaviour {
	
	private HUD hud;
	
	// Use this for initialization
	void Start () {
		hud = transform.parent.gameObject.GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
		if (hud.m_LevelParams.BronzeMaxTurns - hud.num_turns < hud.m_LevelParams.GoldMaxTurns)
		{
			guiText.material.color = Color.yellow;
		}
		else if (hud.m_LevelParams.BronzeMaxTurns - hud.num_turns < hud.m_LevelParams.SilverMaxTurns)
		{
			guiText.material.color = new Color (190f/255f, 190f/255f, 190f/255f);
		}
		else
		{
			guiText.material.color = new Color (255f/255f, 100f/255f, 40f/255f);
		}
		guiText.text = ""+hud.num_turns;
	}
}
