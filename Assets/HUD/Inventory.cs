using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
	
	GameObject gadgets, gadgetBomb, gadgetSleepingGas, gadgetSpring, storeIcon, dock;
	GameObject gadgetActiveBomb, gadgetActiveSleepingGas, gadgetActiveSpring;
	GameObject gadgetBombCounter, gadgetSleepingGasCounter, gadgetSpringCounter;
	public GameObject m_GadgetBomb;
	public GameObject m_GadgetSpring;
	public GameObject m_GadgetSleepingGas;
	private GameObject selectedGadget;

	private bool resethudstatenextframe;
	private HUD hud;
	HUD.HudState next_hudstate;
	
	int num_bombgadgets, num_sleepinggasgadgets, num_springgadgets;
	
	// Use this for initialization
	void Start () {

		hud = transform.parent.transform.parent.gameObject.GetComponent<HUD>();

		gadgets = transform.FindChild("Gadgets").gameObject;
		gadgetBomb = gadgets.transform.FindChild("GadgetActive_Bomb").gameObject;
		gadgetSleepingGas = gadgets.transform.FindChild("GadgetActive_Sleepinggas").gameObject;
		gadgetSpring = gadgets.transform.FindChild("GadgetActive_Spring").gameObject;
		
		gadgetActiveBomb = gadgets.transform.FindChild("Gadget_Bomb").gameObject;
		gadgetActiveSleepingGas = gadgets.transform.FindChild("Gadget_Sleepinggas").gameObject;
		gadgetActiveSpring = gadgets.transform.FindChild("Gadget_Spring").gameObject;

		gadgetBombCounter = gadgets.transform.FindChild("Gadget_BombCounter").gameObject;
		gadgetSleepingGasCounter = gadgets.transform.FindChild("Gadget_SleepingGasCounter").gameObject;
		gadgetSpringCounter = gadgets.transform.FindChild("Gadget_SpringCounter").gameObject;
		
		// Number of gadgets is whatever play 'owns' plus extras given for level
		num_bombgadgets = (hud.player_ninja.persistent_bombgadgets + hud.m_LevelParams.ExtraBombGadgets);
		num_sleepinggasgadgets = (hud.player_ninja.persistent_sleepinggasgadgets + hud.m_LevelParams.ExtraSleepingGasGadgets);
		num_springgadgets = (hud.player_ninja.persistent_springgadgets + hud.m_LevelParams.ExtraSpringGadgets);
		gadgetBombCounter.guiText.text = ""+num_bombgadgets;
		gadgetSleepingGasCounter.guiText.text = ""+num_sleepinggasgadgets;
		gadgetSpringCounter.guiText.text = ""+num_springgadgets;

		storeIcon = transform.FindChild("StoreIcon").gameObject;
		dock = transform.FindChild("InventoryDock").gameObject;
		
		gadgets.SetActive (false);
		storeIcon.SetActive (false);
		dock.SetActive (false);
		
		gadgetActiveBomb.SetActive (false);
		gadgetActiveSleepingGas.SetActive (false);
		gadgetActiveSpring.SetActive (false);
		
		resethudstatenextframe = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (resethudstatenextframe) {
			hud.hudstate = next_hudstate;
			resethudstatenextframe = false;
		}
		if (hud.hudstate == HUD.HudState.PLACINGGADGETS)
		{
			if (gadgetBomb.guiTexture.HitTest (Input.mousePosition) && num_bombgadgets > 0)
			{
				hud.playerController.UsePlaceBomb (true);
				hud.playerController.UsePlaceSpring (false);
				hud.playerController.UsePlaceSleepingGas (false);
				hud.playerController.UseCursor (false);
				selectedGadget = (GameObject)m_GadgetBomb;
				gadgetActiveBomb.SetActive (true);
				gadgetActiveSleepingGas.SetActive (false);
				gadgetActiveSpring.SetActive (false);
			}
			else if (gadgetSleepingGas.guiTexture.HitTest (Input.mousePosition) && num_sleepinggasgadgets > 0)
			{
				hud.playerController.UsePlaceBomb (false);
				hud.playerController.UsePlaceSleepingGas (true);
				hud.playerController.UsePlaceSpring (false);
				hud.playerController.UseCursor (false);
				selectedGadget = (GameObject)m_GadgetSleepingGas;
				gadgetActiveBomb.SetActive (false);
				gadgetActiveSleepingGas.SetActive (true);
				gadgetActiveSpring.SetActive (false);
			}
			else if (gadgetSpring.guiTexture.HitTest (Input.mousePosition) && num_springgadgets > 0)
			{
				hud.playerController.UsePlaceBomb (false);
				hud.playerController.UsePlaceSpring (true);
				hud.playerController.UsePlaceSleepingGas (false);
				hud.playerController.UseCursor (false);
				selectedGadget = (GameObject)m_GadgetSpring;
				gadgetActiveBomb.SetActive (false);
				gadgetActiveSleepingGas.SetActive (false);
				gadgetActiveSpring.SetActive (true);
			}
			/*else if (storeIcon.guiTexture.HitTest (Input.mousePosition))
			{
				hud.playerController.UsePlaceBomb (false);
				hud.playerController.UsePlaceSpring (false);
				hud.playerController.UsePlaceSleepingGas (false);
				hud.playerController.UseCursor (true);
				gadgetActiveBomb.SetActive (false);
				gadgetActiveSleepingGas.SetActive (false);
				gadgetActiveSpring.SetActive (false);
			}*/
		}
	}
	
	void OnMouseDown( ) {
		gadgets.SetActive (true);
		storeIcon.SetActive (true);
		dock.SetActive (true);
		hud.hudstate = HUD.HudState.PLACINGGADGETS;
	}
	
	void OnMouseUp( ) {

		resethudstatenextframe = true;

		if (storeIcon.guiTexture.HitTest (Input.mousePosition))
			next_hudstate = HUD.HudState.STOREINLEVEL;
		else
			next_hudstate = HUD.HudState.PLAY;
		
		// work out where and if can place
		// make it and place it
		if (selectedGadget != null) // In case unimplemented
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	        RaycastHit[] hits;
			hits = Physics.RaycastAll (ray);
			if (hits.Length == 1)
			{
				if (hits[0].collider.name == "PlayArea")
				{
					Vector3 pos = hits[0].point;
					pos.y += 1;
					Instantiate (selectedGadget, pos,  Quaternion.identity);
					if (selectedGadget == m_GadgetBomb)
						gadgetBombCounter.guiText.text = ""+(--num_bombgadgets);
					else if (selectedGadget == m_GadgetSleepingGas)
						gadgetSleepingGasCounter.guiText.text = ""+(--num_sleepinggasgadgets);
					else if (selectedGadget == m_GadgetSpring)
						gadgetSpringCounter.guiText.text = ""+(--num_springgadgets);
				}
			}
			selectedGadget = null;
		}
			
		gadgets.SetActive (false);
		storeIcon.SetActive (false);
		dock.SetActive (false);
		hud.playerController.UsePlaceBomb (false);
		hud.playerController.UsePlaceSpring (false);
		hud.playerController.UsePlaceSleepingGas (false);
		hud.playerController.UseCursor (true);
		gadgetActiveBomb.SetActive (false);
		gadgetActiveSleepingGas.SetActive (false);
		gadgetActiveSpring.SetActive (false);

	}
}
