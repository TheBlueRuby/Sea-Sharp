using Godot;
using System;

namespace SeaSharp {
	public partial class Sprites : Node2D {
		private AnimatedSprite2D head;
		private AnimatedSprite2D body;
		private AnimatedSprite2D legs;

		private string animation;

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
			head = GetNode<AnimatedSprite2D>("Head");
			body = GetNode<AnimatedSprite2D>("Body");
			legs = GetNode<AnimatedSprite2D>("Legs");
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



	}
}
