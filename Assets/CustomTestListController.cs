﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTestListController : MonoBehaviour {

	Button btnMain;
	[SerializeField] Button btnRefresh;
	[SerializeField] GameObject content_panel;

	// animation components
	Animator mainAnimator;
	[SerializeField] AnimationClip anim_open;

	Vector2 origPosition;

	// variables
	[SerializeField] bool isOpen = false;

	private void OnEnable()
	{
		origPosition = new Vector2(transform.position.x, transform.position.y);
	}

	// Use this for initialization
	void Start () {
		mainAnimator = this.gameObject.GetComponent<Animator>();
		btnMain = gameObject.GetComponent<Button>();

		UIManager uim = GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		btnRefresh.onClick.AddListener(() => {
			uim.sfxOpen();
		});
		
	}

	private void OnDisable()
	{
		isOpen = false;
		mainAnimator.ResetTrigger("isOpen");
		mainAnimator.SetTrigger("isClosed");

		transform.position = origPosition;
	}

	public void refresh(){
		content_panel.SetActive(false);
		content_panel.SetActive(true);
	}

	public void open(){
		StartCoroutine(openPanel());
	}

	//open the panel through animation
	IEnumerator openPanel(){

		// make button not interactable till the animation is finished playing
		btnMain.enabled = false;

		// begin open animation
		if(!isOpen){

			//open if in closed state
			mainAnimator.ResetTrigger("isClosed");
			mainAnimator.SetTrigger("isOpen");

			yield return new WaitForSeconds(anim_open.length);
			content_panel.SetActive(true);

			isOpen = true;

		}else{

			//close if in closed state
			mainAnimator.ResetTrigger("isOpen");
			mainAnimator.SetTrigger("isClosed");

			yield return new WaitForSeconds(anim_open.length);
			content_panel.SetActive(false);

			isOpen = false;
		}

		// make button interactable after animation
		btnMain.enabled = true;
		
	}

}
