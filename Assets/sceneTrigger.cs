﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class sceneTrigger : MonoBehaviour
{
	private string link;
	private string testType;

	[SerializeField] private CanvasGroup loadingCanvas;

	[SerializeField] private string userID;
	[SerializeField] private string username;
	[SerializeField] private string testID;
	[SerializeField] private string rating;
	[SerializeField] private string testMode;

	private GameObject GameController;
	private Transform LoadingScreen;
	private testHandler testHandler;
	private Button btnProceed;

	private Transform loadingIcon;
	private Transform statusText;

	private Text totalQuestions;
	private Text Corrects;
	private Text Mistakes;
	private Text totalScore;

	FirstPersonController FPC;

	//Referencing other scripts
	private void Awake()
	{
		GameController = GameObject.FindGameObjectWithTag ("GameController");
		testHandler = GameObject.Find ("The Testing Ground").GetComponent<testHandler> ();
		testType = GameController.GetComponent<DBContentProcessor> ().testMode;
		btnProceed = GameObject.Find ("PlayerUI_Canvas").transform.Find ("LoadingCanvas").Find ("Button").GetComponent<Button> ();
		LoadingScreen = GameObject.Find ("PlayerUI_Canvas").transform.Find ("LoadingCanvas");

		loadingIcon = LoadingScreen.Find ("LoadingIcon");
		statusText = LoadingScreen.Find ("statusText");

		loadingIcon.gameObject.SetActive (false);
		statusText.gameObject.SetActive (false);

		totalQuestions = LoadingScreen.Find ("Total Questions").Find ("Value").GetComponent<Text> ();
		Corrects = LoadingScreen.Find ("Corrects").Find ("Value").GetComponent<Text> ();
		Mistakes = LoadingScreen.Find ("Mistakes").Find ("Value").GetComponent<Text> ();
		totalScore = LoadingScreen.Find ("Total Score").Find ("Value").GetComponent<Text> ();

		btnProceed.onClick.AddListener (() => {
			
			if (testType == "PRE")
			{
				Initiate.Fade ("learn", Color.black, 5f);
			}
			else if (testType == "POST")
			{
				Initiate.Fade ("Main UI", Color.black, 5f);
			}

		});
	}

	/*
	 * When the player enters the trigger:
	 * - Show loading screen
	 * - Proccess test data
	 * - Submit data to the database
	*/
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			FPC = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> ();

			FPC.walkToggle = false;

			LoadingScreen.gameObject.SetActive (true);

			FPC.canPause = false;

			totalQuestions.text = testHandler.totalQuestions.ToString ();
			Corrects.text = testHandler.correctCount.ToString ();
			Mistakes.text = testHandler.mistakeCount.ToString ();
			totalScore.text = testHandler.playerScore.ToString ();

			userID = GameController.GetComponent<LoginModule> ().userID;
			username = GameController.GetComponent<LoginModule> ().accountUsername;
			testID = GameController.GetComponent<DBContentProcessor> ().testIndexID;
			rating = testHandler.scoreAverage.ToString ();
			testMode = GameController.GetComponent<DBContentProcessor> ().testMode;

			StartCoroutine(submitScore (userID, username, testID, rating, testMode));
		}
	}

	private IEnumerator submitScore(string sf_userID, string sf_username, string sf_testID, string sf_rating, string sf_testMode)
	{
		FPC.walkToggle = false;
		FPC.m_MouseLook.lockCursor = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		StartCoroutine (FadeInLoadingCanvas (loadingCanvas, loadingCanvas.alpha, 1));

		LoadingScreen.GetComponent<CanvasGroup> ().interactable = true;
		LoadingScreen.GetComponent<CanvasGroup> ().blocksRaycasts = true;

		loadingIcon.gameObject.SetActive (true);
		statusText.gameObject.SetActive (true);

		WWWForm form = new WWWForm ();
		form.AddField ("sf_userID", sf_userID);
		form.AddField ("sf_username", sf_username);
		form.AddField ("sf_testID", sf_testID);
		form.AddField ("sf_rating", sf_rating);
		form.AddField ("sf_testMode", sf_testMode);

		link = "http://" + GameController.GetComponent<GameSettingsManager> ().link + "/game_client/score_submit.php";
		WWW www = new WWW (link, form);

		yield return www;

		Text txtStatus = statusText.GetComponent<Text> ();
		loadingIcon.gameObject.SetActive (false);
		txtStatus.text = "You may now click " + '"' + "Proceed" + '"' + " to continue.";

		btnProceed.interactable = true;
	}

	private IEnumerator FadeInLoadingCanvas(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
	{
		float timeStarted = Time.time;
		float timeLapsed = Time.time - timeStarted;
		float percentage = timeLapsed / lerpTime;

		while(true)
		{
			timeLapsed = timeLapsed = Time.time - timeLapsed;
			percentage = timeLapsed / lerpTime;

			float current = Mathf.Lerp (start, end, percentage);

			cg.alpha = current;

			if (percentage >= 1)
				break;

			yield return new WaitForEndOfFrame ();
		}
	}
}
