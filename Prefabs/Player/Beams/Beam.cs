using Godot;
using System;

namespace SeaSharp {
	public partial class Beam : CharacterBody2D {
		private float speed = 300.0f;
		private int facing;
		private bool canPhase = false;

		public Sprite2D texture { get; set; }

		/// <summary>
		/// Class init function
		/// </summary>
		/// <param name="_facing">Direction of the beam</param>
		/// <param name="_position">Starting position</param>
		/// <param name="_canPhase">If the beam can phase through walls</param>
		public void Start(int _facing, Vector2 _position, bool _canPhase = false) {
			facing = _facing;
			GlobalPosition = _position;
			canPhase = _canPhase;
			if (facing != 0) {
				Velocity = new Vector2(facing * speed, 0);
			} else {
				Velocity = Vector2.Up * speed;
			}
		}


		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			texture ??= GetNode<Sprite2D>("Sprite2D");

			switch (facing) {
				case -1:
					texture.FlipH = true;
					break;
				case 0:
					texture.Rotate((float)Math.PI / 2); // 90 degrees in radians
					break;
			}


			if (canPhase) {
				CollisionMask = 36; // only enemies and doors, no tiles
			}
		}

		public override void _PhysicsProcess(double delta) {
			var collision = MoveAndCollide(Velocity * (float)delta);

			if (collision != null) {
				if (collision.GetCollider().HasMethod("Hit")) {
					collision.GetCollider().Call("Hit");
				}
				QueueFree();
			}
		}
	}
}
