using Godot;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public partial class DialogPopup : Control {
	[Export]
	public string stringType = "items";
	[Export]
	public string stringId = "AnglerCap";

	private CompressedTexture2D icon;

	private DialogEntry dialogEntry;

	private bool currentlyPrinting = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		dialogEntry = new("res://strings.json", stringType, stringId);

		icon = GD.Load<CompressedTexture2D>(dialogEntry.iconPath);
		GetNode<TextureRect>("Background/Icon").Texture = icon;
		PrintPage(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// GD.Print(dialogEntry.pagesDone[0] +" "+ dialogEntry.pagesDone[1]);

		if (!dialogEntry.pagesDone.Last()) {
			for (int i = 0; i < dialogEntry.pages.Count; i++) {
				if (dialogEntry.pagesDone[i] || currentlyPrinting) {
					continue;
				}

				if (Input.IsActionJustPressed("ui_accept")) {
					PrintPage(i);
				}
			}
		} else {
			if (Input.IsActionJustPressed("ui_accept")) {
				GetTree().Root.GetNode<PauseHandler>("GameLoop/PauseHandler").SetPaused(false);
				QueueFree();
			}

		}

	}

	public async void PrintPage(int pageNum) {
		currentlyPrinting = true;
		dialogEntry.pagesDone[pageNum] = await TextBuild(dialogEntry.pages[pageNum], dialogEntry.pagesDone[pageNum]);
		currentlyPrinting = false;
	}

	private async Task<bool> TextBuild(string text, bool done) {
		if (!done) {
			GetNode<Label>("Background/Text").Text = "";
			for (int i = 0; i < text.Length; i++) {
				if (Input.IsActionPressed("ui_accept")) {
					GetNode<Label>("Background/Text").Text = text;
					break;
				}
				GetNode<Label>("Background/Text").Text += text[i];
				// GD.Print(text[i]);
				await ToSignal(GetTree().CreateTimer(0.05F), "timeout");
			}
		}
		await ToSignal(GetTree().CreateTimer(0.05F), "timeout");
		return true;
	}
}
