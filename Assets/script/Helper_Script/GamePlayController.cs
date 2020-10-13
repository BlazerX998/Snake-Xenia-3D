using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController   : MonoBehaviour {

	public static GamePlayController  instance; 

	public GameObject fruit_Pickup, bomb_Pickup;

	private Text score_Text;
	private int scoreCount;

	private float Min_X = -4.25f , Max_X= 4.25f , Min_Y = -2.25f , Max_Y = 2.25f ;
	private float Z_Pos = 5.8f;
	void Awake () {
		MakeInstace();
	}

	void Start()
	{
		score_Text = GameObject.Find("Score").GetComponent<Text>();

		Invoke("StartSpawning", 0.5f);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void MakeInstace(){
		if(instance == null){
			instance = this;

		}
	}

	void StartSpawning(){
		StartCoroutine (SpawnPickUps());

	}

	public void CancelSpawning(){
		CancelInvoke("StartSpawning");

	}

	IEnumerator SpawnPickUps()
	{
		yield return new WaitForSeconds(Random.Range(1f,1.5f));

		if (Random.Range (0, 10) >= 2) {
			Instantiate (fruit_Pickup, new Vector3 (Random.Range (Min_X, Max_X), Random.Range (Min_Y, Max_Y), Z_Pos), Quaternion.identity);
		} 
		else {
			
				Instantiate (bomb_Pickup , new Vector3 (Random.Range (Min_X, Max_X), Random.Range (Min_Y, Max_Y), Z_Pos), Quaternion.identity);

		}


		Invoke("StartSpawning", 0f);

		}

	public void IncreaseScore(){
		scoreCount++;
		score_Text.text = "Score : " + scoreCount;


	}






}
