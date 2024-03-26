using Godot;
using System;

public partial class Beam : CharacterBody2D {
	private float speed = 300.0f;
	private int facing;
	private bool canPhase = false;

	public Sprite2D texture;

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
		Velocity = new Vector2(facing * speed, 0);
	}

	
	/// <summary>
	/// Initialization function
	/// </summary>
	public override void _Ready() {
		texture ??= GetNode<Sprite2D>("Sprite2D");

		if (facing == 1) {
			texture.FlipH = false;
		} else if (facing == -1) {
			texture.FlipH = true;
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
