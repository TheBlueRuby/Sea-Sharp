using Godot;
using System;
using System.IO;

namespace SeaSharp {
	public partial class MainMenu : Control {
		private void OnPlayButtonPress() {
			GetTree().ChangeSceneToFile(Utils.Paths.Resources.GameLoop);
		}


		private void OnQuitButtonPress() {
			GetTree().Quit();
		}

		private void OnResetButtonPress() {
			if (Godot.FileAccess.FileExists(Utils.Paths.Resources.Save)) {
				DirAccess.Open(Utils.Paths.Resources.SaveDir).Remove(Utils.Paths.Resources.SaveFile);
			}
		}
	}
}
