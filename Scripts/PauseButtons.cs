using Godot;
using System;

namespace SeaSharp {
	public partial class PauseButtons : Control {
		// Handle to the GameLoop node
		private Node2D GameLoop;

		public override void _Ready() {
			GameLoop = GetTree().Root.GetNode<Node2D>(Utils.Paths.SceneTree.GameLoop);
		}

		/// <summary>
		/// Toggles the global pause state.
		/// </summary>
		private void Resume() {
			GameLoop.GetNode<PauseHandler>("PauseHandler").TogglePaused();
		}

		/// <summary>
		/// Saves the game.
		/// </summary>
		private void Save() {
			GameLoop.Call("save_game");
		}

		/// <summary>
		/// Unpauses the game and changes to the main menu.
		/// </summary>
		private void Quit() {
			GameLoop.GetNode<PauseHandler>("PauseHandler").TogglePaused();
			GetTree().ChangeSceneToFile(Utils.Paths.Resources.Menus + "MainMenu.tscn");
		}

		/// <summary>
		/// Saves and quits.
		/// </summary>
		private void SaveAndQuit() {
			Save();
			Quit();
		}
}
}
