using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Each spawner uses this script to spawn objects vertically.
/// Side by side, the spawners collaborate to make a grid of shapes that make up the whole level.
/// </summary>
public class LevelSpawner.cs : MonoBehaviour
{
	private GlobalData globalData;

	private List<int[]> lineHolderCopy;

	private float heightGap;
	private float spawnYPos;

	private int lineNumber;

	// Flowing shapes to be instantiated in-game.
	/// <summary>
	/// Flowing star is non-catcher. Goal is to tap it in game to destroy it before it reaches the catchers.
	/// </summary>
	private GameObject flowing_star;
	private GameObject flowing_empty;
	private GameObject flowing_circle;
	private GameObject flowing_diamond;

	// Locations of shape spawners.
	private float[] spawnerPositions;
	Transform trSpawner1;
	Transform trSpawner2;
	Transform trSpawner3;
	Transform trSpawner4;

	// Unused.
	//public Image bomb_button_graphic;
	//public Bomb_Handler bomb_holder;
	//UnityEngine.Events.UnityAction bomb_holder_action;
	//GameObject shapes_runtime_parent;
	//Transform shapes_runtime_parent_transform;

	void Start()
	{
		globalData = GameObject.FindGameObjectWithTag("global_data").GetComponent<GlobalData>();
		lineNumber = 0;

		// Set flowing shapes to prefabs in the resources folder.
		try
		{
			flowing_circle = Resources.Load("prefabs/flowingCircle") as GameObject;
			flowing_diamond = Resources.Load("prefabs/flowingDiamond") as GameObject;
			flowing_star = Resources.Load("prefabs/flowingStar") as GameObject;
			flowing_empty = Resources.Load("prefabs/flowingEmpty") as GameObject;
		}
		catch (Exception ex)
		{
			Debug.LogError("Problem loading flowing shapes from prefabs in the resources folder. " + ex.Message);
		}

		// Holds arrays that correspond to sequential lines of shapes.
		lineHolderCopy = GameObject.FindGameObjectWithTag("spawner_holder_tag").GetComponent<FlowObject_Spawner_Holder>().LineHolder;

		// Gameobject where all flowing shapes are stored as children (keeps inspector clean and easy to manage).
		shapes_runtime_parent = GameObject.FindGameObjectWithTag("shapes_runtime_parent");
		shapes_runtime_parent_transform = shapes_runtime_parent.transform;

		//Get transorm positions of spawners.
		trSpawner1 = GameObject.FindGameObjectWithTag("spawner1").transform;
		trSpawner2 = GameObject.FindGameObjectWithTag("spawner2").transform;
		trSpawner3 = GameObject.FindGameObjectWithTag("spawner3").transform;
		trSpawner4 = GameObject.FindGameObjectWithTag("spawner4").transform;

		// Holds the y values of the spawners associated with each column.
		spawnerPositions = new float[4];
		spawnerPositions[0] = trSpawner1.position.y;
		spawnerPositions[1] = trSpawner2.position.y;
		spawnerPositions[2] = trSpawner3.position.y;
		spawnerPositions[3] = trSpawner4.position.y;
		spawnYPos = trSpawner1.position.y;

		//Starts from the end, stacking the columns upwards to the end of the level/beginning of the text file.
		lineNumber = lineHolderCopy.Count - 1;
		heightGap = globalData.HeightGap;

		// Spawn shapes.
		switch (globalData.SwitcherSetup)
		{
			case "DD":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, true, 0, "down");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "down");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "down");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, true, 0, "down");
					lineNumber--;
				}
				break;

			case "DDD":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, true, 0, "down");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "down");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "down");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, false, 0, "down");
					lineNumber--;
				}
				break;

			case "DDDD":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, false, 0, "down");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "down");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "down");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, false, 0, "down");
					lineNumber--;
				}
				break;

			// Reversed switchers.
			case "DU":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, true, 0, "down");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "down");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "up");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, true, 0, "down");
					lineNumber--;
				}
				break;

			case "DUD":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, true, 0, "up");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "down");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "up");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, false, 0, "down");
					lineNumber--;
				}
				break;

			case "DUDU":
				for (int i = lineHolderCopy.Count; i > 0; i--)
				{
					Spawner(lineNumber, 0, trSpawner1.position.y, trSpawner1, false, 0, "down");
					Spawner(lineNumber, 1, trSpawner2.position.y, trSpawner2, false, 0, "up");
					Spawner(lineNumber, 2, trSpawner3.position.y, trSpawner3, false, 0, "down");
					Spawner(lineNumber, 3, trSpawner4.position.y, trSpawner4, false, 0, "up");
					lineNumber--;
				}
				break;

		}

		// Sets listeners on all star gameObjects so they (pop) are destroyed upon clicking.
		foreach (GameObject star in GameObject.FindGameObjectsWithTag("FlowObject_Star"))
		{
			star.GetComponent<Button>().onClick.AddListener(() => { Destroy(star); Debug.Log("flowing star clicked and destroyed"); });
			star.GetComponent<Button>().onClick.AddListener(() => { FindObjectOfType<AudioManager>().GetComponent<AudioManager>().Play("sound_star"); });
			// Debug.Log("Star listeners set.");
		}

	}

	/// <summary>
	/// Spawns a column of shapes based on the column-index of the spawner.
	/// </summary>
	public void Spawner(int lineIndex, int spawnerPos, float spawnYPos, Transform transform, bool customOn, int customShape, string direction)
	{
		GameObject tempGO = null;

		if (customOn == false) // If column is enabled, spawn appropriate gameObject, otherwise spawn nothing.
		{

			// A line of the level's text file.
			int[] tempLine = lineHolderCopy[lineIndex];

			// Spawning a shape of a line (tempLine) at a specific column (spawnerPos) between 1 and 4, since there are max 4 columns.
			switch (tempLine[spawnerPos])
			{
				case 0:
					tempGO = Instantiate(flowing_empty, new Vector3(transform.position.x, spawnerPositions[spawnerPos], 1f), Quaternion.identity);
					break;
				case 1:
					tempGO = Instantiate(flowing_circle, new Vector3(transform.position.x, spawnerPositions[spawnerPos], 1f), Quaternion.identity);
					break;
				case 2:
					tempGO = Instantiate(flowing_diamond, new Vector3(transform.position.x, spawnerPositions[spawnerPos], 1f), Quaternion.identity);
					break;
				case 3:
					// Instantiate a flowingEmpty under the flowingStar to register the standard empty collision when the star is popped.
					GameObject go = Instantiate(flowing_empty, new Vector3(transform.position.x, spawnerPositions[spawnerPos], 1f), Quaternion.identity);
					switch (direction)
					{
						case "up":
							// Upward flowing shapes have a negative speed value.
							go.GetComponent<FlowHandler>().TrAmount = globalData.Speed * -1;
							break; 

						case "down":
							// Downward flowing shapes have a negative speed value.
							go.GetComponent<FlowHandler>().TrAmount = globalData.Speed; 
							break;

						default: 
							Debug.LogError("Flow speed not assigned."); 
							break;
					}
					go.transform.SetParent(shapes_runtime_parent.GetComponent<Transform>());

				// Instantiate the flowingStar
				tempGO = Instantiate(flowing_star, new Vector3(transform.position.x, spawnerPositions[spawnerPos], 1f), Quaternion.identity);
				break;

				default:
					Debug.Log("Problem instatiating/spawning flowing shape.");
					break;
			}

			if (direction == "up") // Upward flowing shapes have a negative speed value.
			{
				tempGO.GetComponent<FlowHandler>().TrAmount = globalData.Speed * -1;
				spawnerPositions[spawnerPos] -= heightGap;
				//Debug.Log("Direction of flowing shape = up. New speed: " + tempGO.GetComponent<FlowHandler>().TrAmount);
			}
			else if (direction == "down") // Downward flowing shapes have a negative speed value.
			{
				tempGO.GetComponent<FlowHandler>().TrAmount = globalData.Speed;
				spawnerPositions[spawnerPos] += heightGap;
				//Debug.Log("Direction of flowing shape = down. Speed: " + tempGO.GetComponent<FlowHandler>().TrAmount);
			}

			// Move the instantiated shape under the shapes_runtime_parent in the gameobject hierarchy.
			tempGO.transform.SetParent(shapes_runtime_parent.GetComponent<Transform>());

		}

	}

	// Prints the level in the console as a grid.
	public void PrintLevelConsole()
	{
		Debug.Log("Start of printLevelConsole");
		string level = null;
		for (int i = 0; i < lineHolderCopy.Count; i++)
		{
			level = lineHolderCopy[i][0] + "" + lineHolderCopy[i][1] + "" + lineHolderCopy[i][2] + "" + lineHolderCopy[i][3] + "\n";
		}
		Debug.Log(level);
		Debug.Log("End of printLevelConsole");
	}

}


