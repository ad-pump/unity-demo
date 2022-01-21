using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdPumbPlugin;


[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour , AdPumbPlugin.AdCompletion {

	public delegate void PlayerDelegate();
	public static event PlayerDelegate OnPlayerDied;
	public static event PlayerDelegate OnPlayerScored;

	public float tapForce = 10;
	public float tiltSmooth = 5;
	public Vector3 startPos;
	public AudioSource tapSound;
	public AudioSource scoreSound;
	public AudioSource dieSound;

	private int adCount = 0;

	Rigidbody2D rigidBody;
	Quaternion downRotation;
	Quaternion forwardRotation;

	GameManager game;
	TrailRenderer trail;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		downRotation = Quaternion.Euler(0, 0 ,-100);
		forwardRotation = Quaternion.Euler(0, 0, 40);
		game = GameManager.Instance;
		rigidBody.simulated = false;
		//trail = GetComponent<TrailRenderer>();
		//trail.sortingOrder = 20; 
	}

	void OnEnable() {
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable() {
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}

	void OnGameStarted() {
		rigidBody.velocity = Vector3.zero;
		rigidBody.simulated = true;
	}

	void OnGameOverConfirmed() {
		transform.localPosition = startPos;
		transform.rotation = Quaternion.identity;
		showAd();
	}

	void showAd(){
		adCount++;
		int adToLoad = adCount %3;
		
		switch (adToLoad) {
			case 0:
				Toast.show("loading Interstitial ad with custom loader and onCompletion  freq cap 5 sec ");
				LoaderSettings loader = new LoaderSettings();
				loader.setLogoResID("com.unity3d.player.UnityPlayer");
				AdPlacementBuilder placement = AdPlacementBuilder.Interstitial()
					.name("unity_Interstitial_full")
					.showLoaderTillAdIsReady(true)
					.loaderTimeOutInSeconds(5)
					.frequencyCapInSeconds(5)
					.loaderUISetting(loader)
					.onAdCompletion( this.onAdCompletion );
				DisplayManager.Instance.showAd(placement);
				break;
			case 1:
				Toast.show("loading simple Interstitial ad  freq cap 5 sec ");
				AdPlacementBuilder placement2 = AdPlacementBuilder.Interstitial()
					.name("unity_Interstitial_simple")
					.frequencyCapInSeconds(5);
				DisplayManager.Instance.showAd(placement2);
				break;
			case 2:
				Toast.show("loading Reward ad");
				AdPlacementBuilder placement3 = AdPlacementBuilder.Rewarded()
					.name("unity_Reward")
					.loaderTimeOutInSeconds(5)
					.onAdCompletion( this.onRewardAdCompletion );
				DisplayManager.Instance.showAd(placement3);
				break;
			case 3:
				// Native ad
				break;
		}


	}

	public void onAdCompletion(bool success){  Toast.show("Ad completed");  }

	public void onRewardAdCompletion(bool success){  Toast.show("Reward Ad completed");  }

	void Update() {
		if (game.GameOver) return;

		if (Input.GetMouseButtonDown(0)) {
			rigidBody.velocity = Vector2.zero;
			transform.rotation = forwardRotation;
			rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
			tapSound.Play();
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "ScoreZone") {
			OnPlayerScored();
			scoreSound.Play();
		}
		if (col.gameObject.tag == "DeadZone") {
			rigidBody.simulated = false;
			OnPlayerDied();
			dieSound.Play();
		}
	}

}
