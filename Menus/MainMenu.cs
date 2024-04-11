using Godot;
using System;
using System.IO;

namespace SeaSharp {
	public partial class MainMenu : Control {
		private void OnPlayButtonPress() {
			GetTree().ChangeSceneToFile("res://GameLoop.tscn");
		}


		private void OnQuitButtonPress() {
			GetTree().Quit();
		}

		private void OnResetButtonPress() {
			if (Godot.FileAccess.FileExists("user://SeaSharpSave.sav")) {
				DirAccess.Open("user://").Remove("SeaSharpSave.sav");
			}
		}
	}
}
