using Godot;
using System;

namespace SeaSharp {
	public partial class Sprites : Node2D {
		private Player player;

		private const string spriteDirectory = "res://Prefabs/Player/MovementSprites/";
		private SpriteFrames headDefault;
		private SpriteFrames bodyDefault;
		private SpriteFrames legsDefault;

		private SpriteFrames headAngler;
		private SpriteFrames bodyPressure;
		private SpriteFrames legsFlippers;

		private AnimatedSprite2D head;
		private AnimatedSprite2D body;
		private AnimatedSprite2D legs;

		private string animation = "Idle";

		public string Animation {
			get => animation;
			set {
				animation = value;
				head.Animation = animation;
				body.Animation = animation;
				legs.Animation = animation;
				if (animation == "Flipper") {
					RotationDegrees = 90;
				} else {
					RotationDegrees = 0;
				}
			}
		}

		public override void _Ready() {
			player = GetTree().Root.GetNode<Player>(Utils.Paths.SceneTree.Player);

			head = GetNode<AnimatedSprite2D>("Head");
			body = GetNode<AnimatedSprite2D>("Body");
			legs = GetNode<AnimatedSprite2D>("Legs");

			headDefault = GD.Load<SpriteFrames>(spriteDirectory + "Head/default.tres");
			bodyDefault = GD.Load<SpriteFrames>(spriteDirectory + "Body/default.tres");
			legsDefault = GD.Load<SpriteFrames>(spriteDirectory + "Legs/default.tres");

			headAngler = GD.Load<SpriteFrames>(spriteDirectory + "Head/angler.tres");
			bodyPressure = GD.Load<SpriteFrames>(spriteDirectory + "Body/pressure.tres");
			legsFlippers = GD.Load<SpriteFrames>(spriteDirectory + "Legs/flipper.tres");
		}

		public void Play() {
			head.Play();
			body.Play();
			legs.Play();
		}

		public void Stop() {
			head.Stop();
			body.Stop();
			legs.Stop();
		}

		public void CheckItems() {
			var headNewAnim = headDefault;
			var bodyNewAnim = bodyDefault;
			var legsNewAnim = legsDefault;

			if (player.Inventory.HasItem(Inventory.ItemTypes.AnglerCap)) {
				headNewAnim = headAngler;
			}

			if (player.Inventory.HasItem(Inventory.ItemTypes.PressureSuit)) {
				bodyNewAnim = bodyPressure;
			}

			if (player.Inventory.HasItem(Inventory.ItemTypes.Flippers)) {
				legsNewAnim = legsFlippers;
			}

			if (head.SpriteFrames != headNewAnim) {
				head.SpriteFrames = headNewAnim;
			}

			if (body.SpriteFrames != bodyNewAnim) {
				body.SpriteFrames = bodyNewAnim;
			}

			if (legs.SpriteFrames != legsNewAnim) {
				legs.SpriteFrames = legsNewAnim;
			}

			Play();
		}

	}
}
