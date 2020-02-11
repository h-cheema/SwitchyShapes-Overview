using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

/// <summary>
/// Determines and sets game information for a level before its scene is loaded.
/// Sort of a pre-initialization for each level.
/// </summary>
public class LevelPreprocessor : MonoBehaviour
{
	private GlobalData globalData;

	private int[] sequenceArray;
	private string[] currentLevel;
	private int lineNumber;

	private float waitTime; // default = 2
	private int lineCount;

	/// <summary>
	/// Holds arrays that correspond to sequential lines of shapes.
	/// </summary>
	private List<int[]> lineHolder = new List<int[]>();

	// A "column" is a container which holds all components of a given catcher system (spawner, catcher, button, verifier, hitbox, etc).
	private GameObject column1;
	private GameObject column2;
	private GameObject column3;
	private GameObject column4;

	void Start()
	{
		globalData = GameObject.FindGameObjectWithTag("global_data").GetComponent<GlobalData>();

		lineCount = GameObject.FindGameObjectWithTag("spawner_holder_tag").GetComponent<FlowObject_Spawner_Holder>().LineHolder.Count;

		// Tranforma positions of the in-game switcher columns.
		float[] column_x_positions = { -6.7f, -4f, -1.4f, 1.21f };

		// I originally used a foreach loop for this but it didn't work on Android builds, 
		// so this is a reasonable substitute that will very likely never need to change.
		// I can make an efficient system if I need that type of dynamic functionality.
		column1 = GameObject.Find("Column1");
		column2 = GameObject.Find("Column2");
		column3 = GameObject.Find("Column3");
		column4 = GameObject.Find("Column4");


		// Sets up the switchers in order. D = Down, U = Up. The letters correspond to the switchers from left to right.
		switch (globalData.SwitcherSetup)
		{
			case "DD": 
				GameObject.FindGameObjectWithTag("button_changer1").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher1").SetActive(false);
				GameObject.FindGameObjectWithTag("button_changer4").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher4").SetActive(false);
				
				globalData.CurrentStageIndex = 1;
				break;

			case "DDD":
				// Disable the first column, leaving columns 2 to 4 enabled.
				GameObject.FindGameObjectWithTag("button_changer1").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher1").GetComponent<SpriteRenderer>().enabled = false;

				column1.transform.position = new Vector3(column_x_positions[0], column1.transform.position.y, column1.transform.position.z);
				column2.transform.position = new Vector3(column_x_positions[1], column2.transform.position.y, column2.transform.position.z);
				column3.transform.position = new Vector3(column_x_positions[2], column3.transform.position.y, column3.transform.position.z);
				column4.transform.position = new Vector3(column_x_positions[3], column4.transform.position.y, column4.transform.position.z);

				globalData.CurrentStageIndex = 2;
				break;


			case "DDDD":
				// Default setup. No changes required.
				globalData.CurrentStageIndex = 3;
				break;


			case "DU":
				GameObject.FindGameObjectWithTag("button_changer1").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher1").SetActive(false);
				GameObject.FindGameObjectWithTag("button_changer4").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher4").SetActive(false);

				FlipSwitcher("3");

				globalData.CurrentStageIndex = 4;
				break;


			case "DUD":
				// Disable the first column, leaving columns 2 to 4 enabled.
				GameObject.FindGameObjectWithTag("button_changer1").SetActive(false);
				GameObject.FindGameObjectWithTag("catcher1").GetComponent<SpriteRenderer>().enabled = false;

				column1.transform.position = new Vector3(column_x_positions[0], column1.transform.position.y, column1.transform.position.z);
				column2.transform.position = new Vector3(column_x_positions[1], column2.transform.position.y, column2.transform.position.z);
				column3.transform.position = new Vector3(column_x_positions[2], column3.transform.position.y, column3.transform.position.z);
				column4.transform.position = new Vector3(column_x_positions[3], column4.transform.position.y, column4.transform.position.z);

				FlipSwitcher("3");

				globalData.CurrentStageIndex = 5;
				break;


			case "DUDU":
				FlipSwitcher("2");
				FlipSwitcher("4");

				globalData.CurrentStageIndex = 6;
				break;
		}

		// Once the columns are set and arragned correctly, the data of the level is processed using InitLevel().
		InitLevel();
	}

	/// <summary>
	/// Builds the lineHolder List from the text file level. 
	/// It extracts each char in each line of the text file. That array or chars
	/// </summary>
	public void InitLevel() 
	{
		//Debug.Log("Starting level reading and building...");
		try
		{
			lineNumber = 0;

			// Upon loading the scene, this script will use the globalData instance to dynamically load the correct level.
			currentLevel = globalData.LevelListStrings[globalData.CurrentLevelIndex]; // Selecting a specific file to load.

			for (int i = 0; i < currentLevel.Length; i++) // Foreach line (shapes row) in the text file (level).
			{
				sequenceArray = new int[4]; // One line of 4 chars.

				char[] storedCharShapes = currentLevel[i].ToCharArray(); // Char array of a single row.

				for (int j = 0; j < 4; j++) // j increments through each shape from the current line.
				{

					try
					{
						// Add shape to the array index one by one. Shapes stored as chars. "- 48" Converts from ASCII code to int.
						sequenceArray[j] = storedCharShapes[j] - 48; // Paralleling a char[] into an int[]
					}
					catch (IndexOutOfRangeException ex)
					{
						Debug.Log("Please check the level's text file.");
					}
				}

				//Debug.Log("Line saved in sequence array.");

				lineHolder.Add(sequenceArray); // List of ints the spawner uses.
				lineNumber++;
			}

		}
		catch (Exception ex)
		{
			Debug.Log("Exception in GlobalData.initLevel(): " + ex.Message);
		}

	}

	/// <summary>
	///  Prints a level list (parameter) and prints it out as a grid in the console.
	/// </summary>
	/// <param name="list">The List<int> to print</param>
	public void PrintLevelConsole(List<int[]> list)
	{
		string level = null;
		for (int i = 0; i < list.Count; i++)
		{
			int[] tempArray = list[i];
			level += tempArray[0] + "" + tempArray[1] + "" + tempArray[2] + "" + tempArray[3] + "\n";
		}
		Debug.Log("lineHolder: " + level);
	}

	/// <summary>
	/// Flips a "switcher setup" vertically.
	/// </summary>
	/// <param name="switcher">A GameObject called a "column" that holds a catcher, button and spawner.</param>
	public void FlipSwitcher(string switcher)
	{
		// Values needed for transformation
		float newSpanwerValue = -7f;
		float newCatcherValue = 14.16f;

		// spawner to move
		GameObject spawner = GameObject.FindGameObjectWithTag("spawner" + switcher);
		Transform spawnerTransform = spawner.GetComponent<Transform>();

		// catcher to move
		GameObject catcher = GameObject.FindGameObjectWithTag("catcher" + switcher);
		Transform catcherTransform = catcher.GetComponent<Transform>();

		// hitbox to move
		Transform hitbox = GameObject.FindGameObjectWithTag("hitbox" + switcher).GetComponent<Transform>();
		hitbox.GetComponent<Transform>().localPosition = new Vector3(hitbox.localPosition.x, 2.256f, hitbox.localPosition.z);

		// Moving the game objects
		spawnerTransform.localPosition = new Vector3(spawnerTransform.localPosition.x, newSpanwerValue, spawnerTransform.localPosition.z);
		catcherTransform.localPosition = new Vector3(catcherTransform.localPosition.x, newCatcherValue, catcherTransform.localPosition.z);

		// Debug.Log("Switcher " + switcher + " flipped.");
	}

	public List<int[]> LineHolder
	{
		get { return lineHolder; }
		set { lineHolder = value; }
	}

}

