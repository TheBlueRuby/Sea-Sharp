using Godot;
using System;

public partial class PickupText : Control {
	[Export]
	public string text = "";
	[Export]
	public CompressedTexture2D itemIcon;

	private bool textDone = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemIcon ??= GD.Load<CompressedTexture2D>("res://icon.svg");

		GetNode<TextureRect>("Background/ItemIcon").Texture = itemIcon;

		TextBuild();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (!textDone) {
			return;
		}
		if (Input.IsActionJustPressed("ui_accept")) {
			QueueFree();
		}
	}

	private async void TextBuild() {
		if (!textDone) {
			for (int i = 0; i < text.Length; i++) {
				if (Input.IsActionPressed("ui_accept")) {
					GetNode<Label>("Background/Text").Text = text;
					break;
				}
				GetNode<Label>("Background/Text").Text += text[i];
				await ToSignal(GetTree().CreateTimer(0.05F), "timeout");
			}
			await ToSignal(GetTree().CreateTimer(0.25F), "timeout");
			textDone = true;
		}
	}
}
