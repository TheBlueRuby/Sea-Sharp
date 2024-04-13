using Godot;
using System;

namespace SeaSharp {
	public partial class PauseButtons : Control {
		private Node2D GameLoop;

		public override void _Ready() {
			GameLoop = GetTree().Root.GetNode<Node2D>(Utils.Paths.SceneTree.GameLoop);
		}

		private void Resume() {
			GameLoop.GetNode<PauseHandler>("PauseHandler").TogglePaused();
		}

		private void Save() {
			GameLoop.Call("save_game");
		}


		private void Quit() {
			GameLoop.GetNode<PauseHandler>("PauseHandler").TogglePaused();
			GetTree().ChangeSceneToFile(Utils.Paths.Resources.Menus + "MainMenu.tscn");
		}


		private void SaveAndQuit() {
			Save();
			Quit();
		}

	}
}
