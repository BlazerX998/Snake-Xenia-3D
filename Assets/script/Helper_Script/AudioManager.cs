using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance; 
	public AudioClip pickup_Sound, dead_Sound;

	// Use this for initialization
	void Awake () {
		MakeInstance();
	}
	
	void MakeInstance(){
		if (instance == null) {
			instance = this;
		}

	}
	public void play_PickupSound()
	{
		AudioSource.PlayClipAtPoint (pickup_Sound, transform.position);

	}

	public void play_DeadSound()
	{
		AudioSource.PlayClipAtPoint (dead_Sound, transform.position);

	}
}
