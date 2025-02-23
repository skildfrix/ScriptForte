﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoPlayer_script : MonoBehaviour
{
	public string videoURL;

	private RectTransform canvas;
	private RawImage rawImage;
	private VideoPlayer videoPlayer;
	private AudioSource audioSource;

	private Transform bgPanel;
	private RectTransform background;

	[SerializeField] Sprite sprite_pause;
	[SerializeField] Sprite sprite_play;

	[SerializeField] private Image icn_playButton;
	[SerializeField] private Button playButton;
	[SerializeField] private Button stopButton;
	[SerializeField] private Button fullscreenButton;

	//[SerializeField] private Slider seekBar;
	[SerializeField] private Slider slider_volume;
	[SerializeField] Text statusText;

	[Header("UI References")]
	[SerializeField] GameObject viewPort;

	seekFunction sk;

	public float vidLength;
	private float playTime;
	private bool isSet = false;
	private bool donePreparing = false;
	bool isFullscreen = false;

	//Video rawImage canvas settings
	private Vector2 originalPosition;
	private float originalWidth;
	private float originalHeight;

	//Background Panel settings
	private Vector2 origBGSizeDelta;
	//private float origBGWidth;
	//private float origBGHeight;

	private void Start()
	{
		//Add actions to the buttons
		playButton.onClick.AddListener (playVideo);

		stopButton.onClick.AddListener (()=>{

			stopVideo();
			closePlayer();

		});

		fullscreenButton.onClick.AddListener(fullscreen);
	}

	private void Awake()
	{
		// get the components in this same gameobject
		sk = transform.Find("controlPanel").Find("seekBar").GetComponent<seekFunction>();
		canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
		audioSource = gameObject.GetComponent<AudioSource>();
		rawImage = gameObject.GetComponent<RawImage>();
		videoPlayer = gameObject.GetComponent<VideoPlayer>();

		bgPanel = gameObject.transform.parent.Find ("bgPanel").transform;
		background = bgPanel.gameObject.GetComponent<RectTransform> ();

		//store preset transform and position
		originalPosition = new Vector2 (transform.position.x, transform.position.y);
		originalWidth = rawImage.rectTransform.rect.width;
		originalHeight = rawImage.rectTransform.rect.height;

		origBGSizeDelta = background.sizeDelta;
	}
		
	private void OnEnable()
	{
		sk.progress.fillAmount = 0;
		bgPanel.gameObject.SetActive (true);
		slider_volume.value = audioSource.volume;
	}

	void Update()
	{
		//set the videostream link and prepare the video (only happens when the videoPlayer url is changed or unset)
		if (videoPlayer.url != videoURL && isSet == false)
		{
			statusText.text = "Loading...";
			Debug.Log("Loading video from server");

			videoPlayer.url = videoURL;

			videoPlayer.Prepare ();
			donePreparing = false;
			isSet = true;
		}

		if (videoPlayer.url != videoURL)
		{
			isSet = false;
		}

		//prepares the video
		if (!videoPlayer.isPrepared)
		{
			statusText.gameObject.SetActive(true);
			videoPlayer.Prepare ();
		}

		//calculate the length of the video and play the video
		if (videoPlayer.isPrepared && donePreparing == false)
		{
			statusText.gameObject.SetActive(false);

			vidLength = (float)videoPlayer.frameCount / (float)videoPlayer.frameRate;
			donePreparing = true;

			//Automatically play the video after preparing
			playVideo ();
		}

		//seek bar progress sync to the current video time
		if (videoPlayer.isPrepared && videoPlayer.isPlaying)
			sk.progress.fillAmount = (float)(videoPlayer.time / (videoPlayer.frameCount / videoPlayer.frameRate));

		audioSource.volume = slider_volume.value;
	}

	void playVideo()
	{
		//If the play button is clicked while the video is still preparing
		if (!videoPlayer.isPrepared)
		{
			statusText.text = "Loading...";
			Debug.Log ("Video is still preparing");
		}
		else
		{
			// If the video is playing, it will pause and vice versa
			if (!videoPlayer.isPlaying)
			{
				rawImage.texture = videoPlayer.texture;
				videoPlayer.Play ();
				audioSource.Play ();
				
				icn_playButton.sprite = sprite_pause;
				playButton.image.color = new Color(0, 255, 118, 255);
			}
			else if (videoPlayer.isPlaying)
			{
				rawImage.texture = videoPlayer.texture;
				videoPlayer.Pause ();
				audioSource.Pause ();

				icn_playButton.sprite = sprite_play;
				playButton.image.color = new Color(199, 255, 255, 255);
			}
		}
	}

	void stopVideo()
	{
		transform.position = originalPosition;
		rawImage.rectTransform.sizeDelta = new Vector2 (originalWidth, originalHeight);

		background.position = originalPosition;
		background.sizeDelta = origBGSizeDelta;

		bgPanel.gameObject.SetActive (false);

		isFullscreen = false;

		videoPlayer.Stop ();
		donePreparing = false;
	}

	void closePlayer()
	{
		transform.position = originalPosition;
		rawImage.rectTransform.sizeDelta = new Vector2 (originalWidth, originalHeight);

		background.position = originalPosition;
		background.sizeDelta = origBGSizeDelta;

		bgPanel.gameObject.SetActive (false);

		isFullscreen = false;

		// enable viewport and disable the player from player view
		gameObject.SetActive(false);
		viewPort.SetActive(true);

	}

	void fullscreen()
	{
		if(!isFullscreen)
		{
			transform.position = new Vector2 (Screen.width / 2, Screen.height / 2);
			rawImage.rectTransform.sizeDelta = new Vector2 (canvas.rect.width, Screen.height);

			background.position = new Vector2 (Screen.width / 2, Screen.height / 2);
			background.sizeDelta = new Vector2 (canvas.rect.width, canvas.rect.height);

			isFullscreen = true;

			Debug.Log("Fullscreen");

		}
		else
		{
			transform.position = originalPosition;
			rawImage.rectTransform.sizeDelta = new Vector2 (originalWidth, originalHeight);

			background.position = originalPosition;
			background.sizeDelta = origBGSizeDelta;

			isFullscreen = false;

			Debug.Log("Not fullscreen");
		}
	}
}
