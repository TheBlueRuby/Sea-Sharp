using Godot;
using System;

namespace SeaSharp {
	public partial class KingCrab : CharacterBody2D {
		enum KingCrabState {
			MovingLeft,
			MovingRight,
			Jumping,
			Dead,
		}

		private const int jumpVel = -300;
		private KingCrabState state = KingCrabState.MovingLeft;
		private RandomNumberGenerator rng;

		private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		[Export]
		public int maxHealth { get; set; } = 10;
		public int health { get; set; }

		private GpuParticles2D particles;
		private Sprite2D sprite;
		private ProgressBar healthBar;

		private PackedScene drop;

		private Node MetSys;
		private Node MetSysCompat;


		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			rng = new RandomNumberGenerator();
			rng.Randomize();

			health = maxHealth;

			particles = GetNode<GpuParticles2D>("BossParticles");
			sprite = GetNode<Sprite2D>("Sprite2D");
			healthBar = GetNode<ProgressBar>("HealthBar");

			drop = GD.Load<PackedScene>("res://Prefabs/Pickups/Misc/AnglerCap.tscn");

			// Initialise the MetSys pointer
			MetSys = GetTree().Root.GetNode<Node>("MetSys");
			MetSysCompat = GetTree().Root.GetNode<Node>("MetSysCompat");

			// Set object owner for MetSys
			Owner = GetTree().Root.GetNode("GameLoop/Map");

			// Register as a MetSys object.
			// If the object has already been registered, delete.
			if ((bool)MetSysCompat.Call("register_obj_marker", this)) {
				QueueFree();
			}
		}

		/// <summary>
		/// Called every physics update.
		/// </summary>
		/// <param name="delta">Time elapsed since previous frame in seconds</param>
		public override void _PhysicsProcess(double delta) {
			if (state == KingCrabState.Dead) {
				return;
			}
			if (IsOnFloor() && rng.RandiRange(0, 50) == 0) {
				state = KingCrabState.Jumping;
			}

			switch (state) {
				case KingCrabState.MovingLeft:
					MoveLeft(delta);
					break;
				case KingCrabState.MovingRight:
					MoveRight(delta);
					break;
				case KingCrabState.Jumping:
					Attack(delta);
					break;
			}

			if (!IsOnFloor()) {
				Velocity += new Vector2(0f, gravity * (float)delta);
			}

			MoveAndSlide();

			healthBar.Value = ((float)health / (float)maxHealth) * 100;
			if (health <= 0) {
				Die();
			}
		}

		public void MoveLeft(double delta) {
			Vector2 velocity = Velocity;
			velocity.X = -100;
			Velocity = velocity;

			if (MoveAndCollide(velocity, true) != null) {
				state = KingCrabState.MovingRight;
			}
		}
		public void MoveRight(double delta) {
			Vector2 velocity = Velocity;
			velocity.X = 100;
			Velocity = velocity;

			if (MoveAndCollide(velocity, true) != null) {
				state = KingCrabState.MovingLeft;
			}
		}
		public void Attack(double delta) {
			Vector2 velocity = Velocity;
			velocity.X = 0;
			velocity.Y = jumpVel;
			state = KingCrabState.MovingLeft;
			Velocity = velocity;
		}

		/// <summary>
		/// Handles the event of hitting the player.
		/// </summary>
		/// <param name="body">The node that was hit.</param>
		private void OnHitPlayer(Node2D body) {
			if (body is Player player) {
				player.Hit(33);
			}
		}

		/// <summary>
		/// When a player beam hits, decrement health.
		/// </summary>
		public void Hit() {
			health--;
		}

		/// <summary>
		/// Kills the boss by hiding it and disabling collision, then spawns a drop.
		/// Also stores it in MetSys so it won't respawn if the room is re-entered
		/// </summary>
		private async void Die() {
			state = KingCrabState.Dead;
			// Store MetSys Object
			MetSysCompat.Call("store_obj", this);

			// Spawn item drop
			SpawnItem();

			// Hide the texture so particles are visible
			sprite.Visible = false;
			healthBar.Visible = false;

			// Spawn a set of particles
			particles.Emitting = true;

			// Disable collision
			GetNode<Area2D>("HurtBox").ProcessMode = ProcessModeEnum.Disabled;
			GetNode<CollisionPolygon2D>("MovementHitbox").Disabled = true;

			// Waits until particles are done emitting
			await ToSignal(GetTree().CreateTimer(3), "timeout");

			QueueFree();
		}

		/// <summary>
		/// Spawns a new item, sets its position, and adds it as a sibling node.
		/// </summary>
		private void SpawnItem() {
			Node2D droppedItem = (Node2D)drop.Instantiate();
			droppedItem.GlobalPosition = GlobalPosition;
			CallDeferred("add_sibling", droppedItem);
		}
	}
}
