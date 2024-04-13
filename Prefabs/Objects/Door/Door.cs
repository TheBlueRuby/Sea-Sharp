using System;
using Godot;

namespace SeaSharp {
	public partial class Door : Node2D {
		private StaticBody2D doorCollision;
		private Sprite2D doorSprite;

		private const float doorOpenTime = 2f;
		private float doorTimer = 0f;
		private bool playerInArea;
		private bool doorOpen;

		[Export]
		public string beamType { get; set; }


		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			doorCollision = GetNode<StaticBody2D>("Collision");
			doorSprite = GetNode<Sprite2D>("Collision/Sprite");

			doorSprite.Texture = GD.Load<CompressedTexture2D>(Utils.Paths.Resources.Prefabs + $"Objects/Door/{beamType}Door/{beamType}Door.png");
		}


		/// <summary>
		/// Called every frame update.
		/// </summary>
		/// <param name="delta">Time elapsed since previous frame in seconds</param>
		public override void _Process(double delta) {
			if (!doorOpen) {
				return;
			}
			if (doorTimer > 0f && playerInArea) {
				doorTimer -= (float)delta;
			} else if (!playerInArea) {
				CloseDoor();

			}
		}
		/// <summary>
		/// Handles the event when the player activates the door.
		/// </summary>
		/// <param name="body">The player or beam node.</param>
		private void OnPlayerActivate(Node2D body) {
			if (body is not Player) {
				return;
			}
			// If this door is a pressure door and the player doesn't have a pressure suit, stay shut
			if (beamType == "Pressure" && !GetTree().Root.GetNode<Player>(Utils.Paths.SceneTree.Player).Inventory.HasItem(Inventory.ItemTypes.PressureSuit)) {
				return;
			}

			playerInArea = true;
			OpenDoor();
		}

		/// <summary>
		/// Called when the player leaves the door. Used to trigger the countdown
		/// </summary>
		/// <param name="body">The player node</param>
		private void OnPlayerDeactivate(Node2D body) {
			if (body is not Player) {
				return;
			}
			playerInArea = false;
		}

		/// <summary>
		/// Called when a beam activates the door. Opens the door if correct type
		/// </summary>
		/// <param name="body">The beam node</param>
		private void OnBeamActivate(Node2D body) {
			// If beam is the same type as the door or the door is a proximity door
			if (body.SceneFilePath.Contains(beamType) || beamType == "Prox") {
				OpenDoor();
			}
			if (beamType == "Pressure" && GetTree().Root.GetNode<Player>(Utils.Paths.SceneTree.Player).Inventory.HasItem(Inventory.ItemTypes.PressureSuit)) {
				OpenDoor();
			}
		}

		/// <summary>
		/// Opens the door by disabling the door's collision and updating the sprite
		/// </summary>
		public void OpenDoor() {
			doorOpen = true;
			doorCollision.ProcessMode = ProcessModeEnum.Disabled;
			doorSprite.SetDeferred("visible", false);
			doorTimer = doorOpenTime;
		}

		/// <summary>
		/// Closes the door by enabling the door's collision and updating the sprite
		/// </summary>
		public void CloseDoor() {
			doorOpen = false;
			doorCollision.ProcessMode = ProcessModeEnum.Inherit;
			doorSprite.SetDeferred("visible", true);
		}

	}
}
