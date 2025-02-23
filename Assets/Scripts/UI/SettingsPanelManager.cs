﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour {

	[Header("UI Components")]
	public InputField InputLink;
	public Toggle Toggle_Bloom;
	public Toggle Toggle_MotionBlur;
	public Dropdown dropdown_resolution;

	[Header("Buttons")]
	public Button btnClose;
	public Button btnApply;

	[Header("Debug Private Variables")]
	[SerializeField] string link;
	[SerializeField] bool bloom;
	[SerializeField] bool motionblur;
	[SerializeField] int resolutionIndex;

	Resolution[] resolutions;
	UIManager UIM;

	private void Start()
	{
		if (gameObject.name == "InGameSettingsPanel")
		{
			UIM = GameObject.FindGameObjectWithTag ("GameController").GetComponent<UIManager> ();

			btnClose.onClick.AddListener (() => {

				UIM.sfxClose();
				gameObject.SetActive(false);

			});

			btnApply.onClick.AddListener (() => {

				ApplySettings();
				UIM.sfxSpecial();

			});
		}
	}

	private void Awake()
	{
		// supply resolution list inside the resolution dropdown menu
		resolutions = Screen.resolutions;

		foreach(Resolution list_resolution in resolutions){

			dropdown_resolution.options.Add(new Dropdown.OptionData(list_resolution.ToString()));

		}
	}

	private void OnEnable()
	{	
		// set game configuration values to controls when SeetingAndControls is loaded
		if(SettingsAndControls.IsLoaded()){

			InputLink.text = SettingsAndControls.Settings.GetString("link");
			Toggle_Bloom.isOn = SettingsAndControls.Settings.GetBool("bloom");
			Toggle_MotionBlur.isOn = SettingsAndControls.Settings.GetBool("blur");
			dropdown_resolution.value = SettingsAndControls.Settings.GetInt("resolution_index");

		}
	}

	public void ApplySettings(){

		link = InputLink.text;
		bloom = Toggle_Bloom.isOn;
		motionblur = Toggle_MotionBlur.isOn;

		resolutionIndex = dropdown_resolution.value;
		Screen.SetResolution(resolutions[dropdown_resolution.value].width, resolutions[dropdown_resolution.value].height, Screen.fullScreen);

		SettingsAndControls.Settings.SetSetting("link", new SACString(link), Setting.SettingType.STRING);
		SettingsAndControls.Settings.SetSetting("bloom", new SACBool(bloom), Setting.SettingType.BOOLEAN);
		SettingsAndControls.Settings.SetSetting("blur", new SACBool(motionblur), Setting.SettingType.BOOLEAN);
		SettingsAndControls.Settings.SetSetting("resolution_index", new SACInt(resolutionIndex), Setting.SettingType.INTEGER);
		SettingsAndControls.Save();

		if (gameObject.name == "SettingsPanel")
		{
			//display messagebox from login module
			LoginModule loginModule = GameObject.FindGameObjectWithTag("GameController").GetComponent<LoginModule>();
			loginModule.messagePrompt("Settings has been changed successfully",2);
		}
		else if (gameObject.name == "InGameSettingsPanel")
		{
			gameObject.SetActive (false);
		}
	}
}
