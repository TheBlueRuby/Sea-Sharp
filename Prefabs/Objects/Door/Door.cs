using System;
using Godot;

public partial class Door : Node2D {
	private StaticBody2D doorCollision;
	private Sprite2D doorSprite;

	private const float doorOpenTime = 2f;
	private float doorTimer = 0f;
	private bool playerInArea;
	private bool doorOpen;

	[Export]
	public string beamType;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		doorCollision = GetNode<StaticBody2D>("Collision");
		doorSprite = GetNode<Sprite2D>("Collision/Sprite");

		doorSprite.Texture = GD.Load<CompressedTexture2D>($"res://Prefabs/Objects/Door/{beamType}Door/{beamType}Door.png");
	}

	public override void _Process(double delta) {
		if (!doorOpen) {
			return;
		}
		if (doorTimer > 0f && playerInArea == false) {
			doorTimer -= (float)delta;
		} else if (!playerInArea) {
			CloseDoor();

		}
	}

	private void OnPlayerActivate(Node2D body) {
		playerInArea = true;
		OpenDoor();
	}

	private void OnPlayerDeactivate(Node2D body) {
		playerInArea = false;
	}


	private void OnBeamActivate(Node2D body) {
		// If beam is the same type as the door or the door is a proximity door
		if (body.SceneFilePath.Contains(beamType) || beamType == "Prox") {
			OpenDoor();
		}
	}

	public void OpenDoor() {
		// GD.Print("Opening");
		doorOpen = true;
		doorCollision.ProcessMode = ProcessModeEnum.Disabled;
		doorSprite.SetDeferred("visible", false);
		doorTimer = doorOpenTime;
	}

	public void CloseDoor() {
		// GD.Print("Closing");
		doorOpen = false;
		doorCollision.ProcessMode = ProcessModeEnum.Always;
		doorSprite.SetDeferred("visible", true);
	}

}
