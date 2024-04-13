using Godot;
using System;
using System.IO;

namespace SeaSharp {
	public partial class MainMenu : Control {
		/// <summary>
		/// Switches to the game loop
		/// </summary>
		private void OnPlayButtonPress() {
			GetTree().ChangeSceneToFile(Utils.Paths.Resources.GameLoop);
		}

		/// <summary>
		/// Quits the game
		/// </summary>
		private void OnQuitButtonPress() {
			GetTree().Quit();
		}

		/// <summary>
		/// Removes the save file
		/// </summary>
		private static void OnResetButtonPress() {
			if (Godot.FileAccess.FileExists(Utils.Paths.Resources.Save)) {
				DirAccess.Open(Utils.Paths.Resources.SaveDir).Remove(Utils.Paths.Resources.SaveFile);
			}
		}
	}
}
