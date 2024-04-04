using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public partial class PickupText : Control {
	[Export]
	public string itemType = "AnglerCap";

	private CompressedTexture2D itemIcon;

	private const string itemList = "res://Prefabs/Pickups/ItemDescriptions.json";

	private bool descDone = false;
	private bool bindDone = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		itemIcon = GD.Load<CompressedTexture2D>(GetItemIcon(itemList));

		GetNode<TextureRect>("Background/ItemIcon").Texture = itemIcon;

		PrintDesc();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (bindDone) {
			if (Input.IsActionJustPressed("ui_accept")) {
				GetTree().Root.GetNode<PauseHandler>("GameLoop/PauseHandler").SetPaused(false);
				QueueFree();
			}

		} else if (descDone) {
			if (Input.IsActionJustPressed("ui_accept")) {
				PrintBind();
			}
		}
	}

	private async void PrintDesc() {
		descDone = await TextBuild(GetItemDesc(itemList), descDone);
	}
	private async void PrintBind() {
		bindDone = await TextBuild(GetItemBind(itemList), bindDone);
	}

	private JsonNode GetItemFromJson(string file) {
		string fileContents = FileAccess.Open(file, FileAccess.ModeFlags.Read).GetAsText();
		JsonNode fileAsJson = JsonNode.Parse(fileContents);
		JsonNode itemsArray = fileAsJson!["items"];
		JsonNode item = null;

		for (int i = 0; i < itemsArray.AsArray().Count; i++) {
			if (itemsArray[i]!["id"]!.GetValue<string>() == itemType) {
				item = itemsArray[i];
			}
		}
		if (item != null) {
			return item;
		} else {
			return null;
		}

	}


	private string GetItemIcon(string file) {
		return GetItemFromJson(file)!["icon"]!.GetValue<string>();
	}
	private string GetItemDesc(string file) {
		return GetItemFromJson(file)!["desc"]!.GetValue<string>();
	}
	private string GetItemBind(string file) {
		string bind = GetItemFromJson(file)!["bind"]!.GetValue<string>();

		if (bind == "None") {
			return "This item is used automatically.";
		}

		return $"Press {bind} to use!";
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
		return true;
	}
}
