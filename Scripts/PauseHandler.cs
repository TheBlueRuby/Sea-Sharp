using Godot;
using System;
namespace SeaSharp {
	public partial class PauseHandler : Node {
		bool paused = false;
		bool uiVisible = false;

		public override void _Process(double delta) {
			// If the player pauses the game while not frozen
			if (Input.IsActionJustPressed("pause") && (paused == uiVisible)) {
				// Toggle the UI and whether the game is paused
				TogglePaused();
				SetUiVisible(paused);
			}
		}

		/// <summary>
		/// Toggles whether the game is paused
		/// </summary>
		public void TogglePaused() {
			SetPaused(!paused);
		}
		
		/// <summary>
		/// Updates the game's pause state
		/// </summary>
		/// <param name="newPaused">The new paused state</param>
		public void SetPaused(bool newPaused) {
			if (uiVisible && !newPaused) {
				SetUiVisible(newPaused);
			}
			paused = newPaused;
			GetTree().Paused = paused;

		}

		/// <summary>
		/// Sets the visibility of the pause menu.
		/// </summary>
		/// <param name="visible"> A boolean indicating whether the menu should be visible or not.</param>
		public void SetUiVisible(bool visible) {
			GetTree().Root.GetNode<CanvasLayer>(Utils.Paths.SceneTree.PauseMenu).Visible = visible;
			uiVisible = visible;
		}
	}
}
