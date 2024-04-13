using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace SeaSharp {
	public partial class DialogPopup : Control {
		[Export]
		public string StringType { get; set; } = "items";
		[Export]
		public string StringId { get; set; } = "AnglerCap";

		private CompressedTexture2D icon;

		private DialogEntry dialogEntry;

		private bool currentlyPrinting = false;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready() {
			dialogEntry = new(Utils.Paths.Resources.Strings, StringType, StringId);

			icon = GD.Load<CompressedTexture2D>(dialogEntry.IconPath);
			GetNode<TextureRect>("Background/Icon").Texture = icon;
			PrintPage(0);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta) {

			if (!dialogEntry.PagesDone.Last()) {
				for (int i = 0; i < dialogEntry.Pages.Count; i++) {
					if (dialogEntry.PagesDone[i] || currentlyPrinting) {
						continue;
					}

					if (Input.IsActionJustPressed("ui_accept")) {
						PrintPage(i);
					}
				}
			} else {
				if (Input.IsActionJustPressed("ui_accept")) {
					GetTree().Root.GetNode<PauseHandler>(Utils.Paths.SceneTree.PauseHandler).SetPaused(false);
					QueueFree();
				}

			}

		}

		/// <summary>
		/// Prints a dialog page based on the provided page number.
		/// </summary>
		/// <param name="pageNum">The number of the page to print</param>
		public async void PrintPage(int pageNum) {
			currentlyPrinting = true;
			dialogEntry.PagesDone[pageNum] = await TextBuild(dialogEntry.Pages[pageNum], dialogEntry.PagesDone[pageNum]);
			currentlyPrinting = false;
		}

		/// <summary>
		/// Creates a typewriter effect for the dialog boxes
		/// </summary>
		/// <param name="text">Text to print</param>
		/// <param name="done">If the text has been printed</param>
		/// <returns>Returns <c>true</c> when the text is finished</returns>
		private async Task<bool> TextBuild(string text, bool done) {
			if (!done) {
				GetNode<Label>("Background/Text").Text = "";
				for (int i = 0; i < text.Length; i++) {
					if (Input.IsActionPressed("ui_accept")) {
						GetNode<Label>("Background/Text").Text = text;
						break;
					}
					GetNode<Label>("Background/Text").Text += text[i];
					await ToSignal(GetTree().CreateTimer(0.05F), "timeout");
				}
			}
			await ToSignal(GetTree().CreateTimer(0.05F), "timeout");
			return true;
		}
	}
}
