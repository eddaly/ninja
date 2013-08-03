using UnityEngine;
using System.Collections;

public class EPSoundEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void Play() {}
	
	public virtual void Play ( float volume, float pitch ) {}
	
	public virtual void Stop() {}
	
	public virtual void Pause() {}
	
	public virtual void Resume() {}
	
	public virtual void SetVolume( float volume ) {}
	
	public virtual float GetVolume()
	{
		return 0.0f;
	}
	
	public virtual void Fade( float endVol, float duration, bool isFadeOut ) {}
	
	public virtual bool IsPlaying()
	{
		return false;
	}
	
	public virtual float time()
	{
		return 0.0f;
	}
	
	public virtual void time( float time ) {}
}