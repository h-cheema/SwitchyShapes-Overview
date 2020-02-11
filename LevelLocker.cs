using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLocker : MonoBehaviour
{
	// References to other scripts
	private GlobalData globalData;
	private Menu_Handler menu_handler;
	private AudioManager audio;

	// LevelLocker variables
	private int[] levelsToLock;
	private int playerLevels;
	private int numberOfLevelButtons;
	private int numberOfLevels;
	private int levelsPerPage;
	private int currentPage;
	private int firstLevelButton;
	private int maxPages;
	private Color unlockedButtonColor;

	// Level Button GameObjects
	private GameObject[] buttonsOrdered;
	public Sprite lockSprite;
	public Sprite unlockedImage;
	private Text txtPage;

	// Slider GameObject
	private Slider levelSpeedSlider;
	private Text levelSpeedText;
	public GameObject levelButton;

	// Colors for flashing button (coroutine)
	public Color32 goldColor = new Color32(255, 170, 0, 255);
	public Color32 greyColor = new Color32(79, 79, 79, 255);

	// Flashing button game objects. Only one is used at a time.
	public GameObject currentLevelButton;
	public GameObject prevArrow;
	public GameObject nextArrow;


	void Start()
	{
		Time.timeScale = 1f;

		// Set references to other scripts
		globalData = GameObject.FindGameObjectWithTag("global_data").GetComponent<GlobalData>();
		menu_handler = GameObject.FindGameObjectWithTag("main_menu").GetComponent<Menu_Handler>();
		audio = AudioManager.globalAudioManager; // globalData has a reference to the static AudioManager.

		// Pages settings variables
		numberOfLevels = globalData.TotalLevels - 1;   
		currentPage = 0;
		levelsPerPage = globalData.LevelsPerPage;
		firstLevelButton = currentPage * levelsPerPage;
		unlockedButtonColor = globalData.LevelLocker_green; // Unlocked level buttons color.

		// Set the number of pages based on number of levels.
		if (numberOfLevels % levelsPerPage == 0)
		{
			maxPages = (numberOfLevels / levelsPerPage); // Complete level pages.
		}
		else
		{
			// Incomplete level pages. Total levels != multiple of var levelsPerPage.
			maxPages = ((numberOfLevels / levelsPerPage) + 1);
		}

		// Get the player's number of completed levels.
		playerLevels = SaveLoad.LoadPlayer().level;

		// Slider setup.
		levelSpeedSlider = GameObject.FindGameObjectWithTag("slider_levelSpeed").GetComponent<Slider>();
		levelSpeedText = GameObject.FindGameObjectWithTag("text_levelSpeed").GetComponent<Text>();
		ChangeLevelSpeed();

		// Stages panel text
		txtPage = GameObject.Find("panel_Stages_StageProgress").GetComponent<Text>();

		// Initialize first flashing button.
		currentLevelButton = null;
		ChangePage(0);

		/* 
		 *	globalSettings.IsStartingLevel can be set through the GlobalData GameObject's inspector in the "menu" scene.
		 *	It is used during level testing to skip menus and go straight to a level on pressing play.
		 *	LevelLocker sets the required values for the level before this "if-statement" is triggered.
		 *	That is, only if the IsStartingLevel bool is set to true through the Unity inspector.
		 */
		if (globalData.GlobalSettings.IsStartingLevel)
		{
			currentPage = 1;
			globalData.CurrentLevelIndex = globalData.GlobalSettings.StartingLevel - 1;
			levelSpeedSlider.value = globalData.GlobalSettings.StartingLevelSpeed;
			menu_handler.loadSceneGame();
		}

		// If all levels are unlocked, change the "Levels" panel title's text.
		if(globalData.Player.Level >= (levelsPerPage * globalData.SwitcherSetupMapping.Count))
		{
			GameObject.Find("panel_LevelButtons_Title").GetComponent<Text>().text = "<color=#FFA700>All Levels Completed!</color>";
			GameObject.Find("panel_LevelButtons_Title").GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		}

	}

	/// <summary>
	/// Locks levels based on how many levels the player has completed. (Player data saved in Unity.PersistentData).
	/// </summary>
	public void LockLevels()
	{
		// Number of buttons in the scene.
		numberOfLevelButtons = GameObject.FindGameObjectsWithTag("level_button").Length;
		buttonsOrdered = GameObject.FindGameObjectsWithTag("level_button");

		// Finds and stores all "level buttons" in an array to modify them iteratively.
		for (int i = 0; i < numberOfLevelButtons; i++)
		{
			// Disable level buttons with greater value than the player's levels completed.
			if (Int32.Parse(buttonsOrdered[i].name) > playerLevels)
			{
				// Disable locked buttons and change their gameobject properties.
				buttonsOrdered[i].GetComponentInChildren<Text>().text = "LOCKED";
				buttonsOrdered[i].GetComponentInChildren<Text>().fontSize = 30;
				buttonsOrdered[i].GetComponent<Button>().enabled = false;

				// Set disabled button color.
				buttonsOrdered[i].GetComponent<Image>().color = new Color32(255, 71, 68, 255); // Light red.
			}
		}

		// Set the current level's button color.
		int currentLevel = SaveLoad.LoadPlayer().level;
		if (GameObject.Find(currentLevel.ToString()) != null)
		{
			GameObject.Find(currentLevel.ToString()).GetComponent<Image>().color = new Color32(255, 170, 0, 255);
			currentLevelButton = GameObject.Find(currentLevel.ToString());
			//Debug.Log("currentLevelButton set");
		}

	}

	/// <summary>
	/// Used by the slider to set the flowing shape speed and the height gap between shapes.
	/// </summary>
	public void ChangeLevelSpeed()
	{
		float sliderSpeed = levelSpeedSlider.value;

		switch (sliderSpeed)
		{
			case 1f: ChangeLevelSpeed_Values("Very Easy", 4f, -3.5f); 
				break;

			case 2f: ChangeLevelSpeed_Values("Easy", 4f, -4.5f); 
				break;

			case 3f: ChangeLevelSpeed_Values("Medium", 4.5f, -6f);
				break;

			case 4f: ChangeLevelSpeed_Values("Hard", 6f, -7f);
				break;

			case 5f: ChangeLevelSpeed_Values("Very Hard", 6f, -8f);
				break;
		}
		//Debug.Log("Level speed changed to: " + sliderSpeed);
	}

	/// <summary>
	/// Called by ChangeLevelSpeed() in switch cases to set the values needed for an interval of the slider.
	/// </summary>
	/// <param name="text">Level speed text (Easy, Medium, etc...).</param>
	/// <param name="hightGap">Height gap between shapes when they are spawned.</param>
	/// <param name="speed">Speed of the flowing shapes.</param>
	public void ChangeLevelSpeed_Values(string text, float hightGap, float speed)
	{
		globalData.HeightGap = hightGap;
		globalData.Speed = speed;
		levelSpeedText.text =  "Difficulty: " + text + "\n" + levelSpeedSlider.value.ToString("#.#");
		globalData.Difficulty = text;
	}

	/// <summary>
	/// Instantiates level buttons in the levels menu. Called every time the page is changed or when it is first initialized.
	/// </summary>
	public void InstantiateButtons()
	{

		// Horizontal positions of the "level buttons" on the page
		float[] x_positions = new float[5];
		float x_pos = -600f;
		float x_posGap = 200f;

		// Vertical positions of the "level buttons" on the page
		float[] y_positions = new float[5];
		float y_pos = 0f;
		float y_posGap = -200f;

		//Create an array of all x y positions needed to evenly space out the "level buttons" in the panel. 
		for (int i = 0; i < 5; i++)
		{
			x_pos += x_posGap;
			x_positions[i] = x_pos;

			y_pos += y_posGap;
			y_positions[i] = y_pos;
		}

		GameObject levels_menu = GameObject.FindGameObjectWithTag("levels_menu");

		// Instantiate default (unmodified) level buttons row by row.
		for (int i = 0; i < 3; i++) // Rows
		{

			for (int j = 0; j < 5; j++) // Columns
			{
				int buttonNumber = ((levelsPerPage * currentPage) + ((i * 5) + j));

				//Button between multiples of levelsPerPage value according to the page number.
				if (buttonNumber >= (currentPage * levelsPerPage) && buttonNumber <= (currentPage + 1) * levelsPerPage)
				{
					//Instatiate button:
					GameObject tempLevelButton = Instantiate(levelButton, new Vector3(0f, 0f, 0f), Quaternion.identity);

					//Set name:
					tempLevelButton.name = buttonNumber.ToString();

					//Set color:
					tempLevelButton.GetComponent<Button>().GetComponent<Image>().color = unlockedButtonColor;

					// Set transforms:
					tempLevelButton.transform.SetParent(levels_menu.transform);
					tempLevelButton.GetComponent<Button>().transform.Rotate(0, 0, 45, Space.Self);
                    			tempLevelButton.transform.localScale = new Vector3(1f, 1f, 1f);
					tempLevelButton.transform.localPosition = new Vector3(x_positions[j], y_positions[i], 1);

					// Add listener:
					tempLevelButton.GetComponent<Button>().onClick.AddListener(() => menu_handler.LevelButtonClick(tempLevelButton));
					tempLevelButton.GetComponent<Button>().onClick.AddListener(() => menu_handler.MenuButtonClick());

					// Set button text (button number):
					tempLevelButton.transform.Find("Text_Number").GetComponent<Text>().text = (buttonNumber + 1).ToString();
					tempLevelButton.transform.Find("Text_Number").GetComponent<Text>().fontSize = 60;

					//Set tag:
					tempLevelButton.tag = "level_button";
				}
			}

		}
		Debug.Log("Buttons instatiated.");
	}


	/// <summary>
	/// Changes the page of the levels menu screen.
	/// </summary>
	/// <param name="page">Page number to change to.</param>
	public void ChangePage(int page)
	{
		switch (page)
		{
			case 0:
				break;

			case -1:
				if (currentPage + page <= 0) { currentPage = 0; } else { currentPage--; }
				break;

			case 1:
				if ((currentPage + page) >= maxPages) { currentPage = 0; } else { currentPage++; }
				break;
		}

		// Set level text.
		txtPage.text = (currentPage + 1) + " of " + ((numberOfLevels/levelsPerPage) + 1);

		// Destory game buttons.
		if (GameObject.FindGameObjectWithTag("level_button") != null)
		{
			foreach (GameObject button in GameObject.FindGameObjectsWithTag("level_button"))
			{
				Destroy(button);
			}
		}

		InstantiateButtons();
		LockLevels();
		SetSwitcherSetupImage();
		SetFlashingGameObject();
	}
	public void NextPage()
	{
		ChangePage(1);
    	}
	public void PreviousPage()
	{
		if (currentPage <= 0)
		{
           		currentPage = numberOfLevels / levelsPerPage;
           		ChangePage(currentPage);
		}
		else
		{
			ChangePage(-1);
		}
	}
	/// <summary>
	/// Changes the image of the stage (based on page number).
	/// </summary>
	public void SetSwitcherSetupImage()
	{
		GameObject.Find("panel_Stages_SwitcherSetupImage").GetComponent<Image>().overrideSprite
			= (Sprite)Resources.Load("images/menu/switcherSetups/" + currentPage.ToString(), typeof(Sprite));
	}


	/// <summary>
	/// Sets the unique flashing object based on the the "level buttons" displayed.
	/// </summary>
	public void SetFlashingGameObject()
	{
		GameObject.Find("panel_Stages_btn_Prev").GetComponent<Image>().color = greyColor;
		GameObject.Find("panel_Stages_btn_Next").GetComponent<Image>().color = greyColor;

		// If player has unlocked all levels.
		if(globalData.Player.Level >= (levelsPerPage * globalData.SwitcherSetupMapping.Count))
		{
			Debug.Log("All levels completed. No flashing button set...");
			return;
		}
		// Else if the first page.
		else if (currentPage == 0 && globalData.Player.Level < levelsPerPage)
		{
			SwitchCoroutine(currentLevelButton);
		}
		// Else if the "currentLevel" (player's max unlocked level) is on screen.
		else if (currentPage != 0 && globalData.Player.Level >= (levelsPerPage * currentPage) 
			&& globalData.Player.Level < (levelsPerPage * (currentPage + 1))) 
		{
			SwitchCoroutine(currentLevelButton);
		}
		// Else the "previous page" or "next page" buttons need to flash.
		else
		{
			// Find all "level buttons" on the page or in the scene by tag.
			GameObject[] gos = GameObject.FindGameObjectsWithTag("level_button");

			// If last button on page's text says "LOCKED" then make the "previous page" button blink.
			if (gos[gos.Length-1].GetComponentInChildren<Text>().text == "LOCKED")
			{
				SwitchCoroutine(GameObject.Find("panel_Stages_btn_Prev"));
			}
			else // Else make the "next page" button blink.
			{
				SwitchCoroutine(GameObject.Find("panel_Stages_btn_Next"));
			}
		}
	}
	/// <summary>
	/// Stops all coroutines and starts a new one
	/// </summary>
	public void SwitchCoroutine(GameObject gameObjectToMakeBlink)
	{
		StopAllCoroutines();
		StartCoroutine(BlinkText_CurrentLevel(gameObjectToMakeBlink));
	}
	/// <summary>
	/// Makes a game object, that is passed in, blink 2 different colors.
	/// </summary>
	public IEnumerator BlinkText_CurrentLevel(GameObject gameObjectToMakeBlink)
	{
		while (true)
		{
			gameObjectToMakeBlink.GetComponent<Image>().color = goldColor;
			yield return new WaitForSeconds(.5f);

			gameObjectToMakeBlink.GetComponent<Image>().color = greyColor;
			yield return new WaitForSeconds(.5f);
		}
	}

}
