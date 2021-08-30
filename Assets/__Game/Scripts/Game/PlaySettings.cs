using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace FG {
	public class PlaySettings : MonoBehaviour {
		public static int BoardSize { get; set; } = 9;
		public static int SlotsToWin { get; set; } = 3;
		public static string PlayerOneName { get; set; } = "Player one";
		public static string PlayerTwoName { get; set; } = "Player two";

		private void Update()
		{
			//Number Spinners in Menu Scene Renamed to Board & Slots respectively
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Menu_Scene"))
			{
				GameObject.Find("Slots").GetComponent<NumberSpinner>().maximumValue = BoardSize;
			}
			
		}
	}
	

	
}
