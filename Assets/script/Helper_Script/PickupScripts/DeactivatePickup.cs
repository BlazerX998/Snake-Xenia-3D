using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Deactivate", Random.Range(3f, 6f));
	}
	
	// Update is called once per frame
	void Deactivate()
	{

		gameObject.SetActive (false);
	}
}
