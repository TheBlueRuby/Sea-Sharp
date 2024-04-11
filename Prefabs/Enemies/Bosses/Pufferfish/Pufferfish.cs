using Godot;
using System;

namespace SeaSharp {
	public partial class Pufferfish : CharacterBody2D {
		private bool isInflated = false;
		private enum PufferfishState {
			Idle,
			Charging,
			Inflating,
			Deflating,
			Dead
		}

		private PufferfishState state = PufferfishState.Idle;

		[Export]
		public int MaxHealth { get; set; } = 10;
		public int Health { get; set; }

		private GpuParticles2D particles;
		private CollisionShape2D inflatedCollision;
		private CollisionShape2D deflatedCollision;
		private ProgressBar healthBar;

		private Node MetSys;
		private Node MetSysCompat;

		private Node2D player;

		private PackedScene drop;

		private const float maxCooldown = 2f;
		private float cooldown;

		private const int Damage = 33;

		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			Health = MaxHealth;
			cooldown = maxCooldown;

			particles = GetNode<GpuParticles2D>("BossParticles");
			inflatedCollision = GetNode<CollisionShape2D>("Inflated");
			deflatedCollision = GetNode<CollisionShape2D>("Deflated");
			healthBar = GetNode<ProgressBar>("HealthBar");

			player = GetTree().Root.GetNode<Node2D>("GameLoop/Player");
			drop = GD.Load<PackedScene>("res://Prefabs/Pickups/Misc/Propeller.tscn");

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
			if (state == PufferfishState.Dead) {
				return;
			}

			healthBar.Value = ((float)Health / (float)MaxHealth) * 100;
			if (Health <= 0) {
				Die();
			}

			if (cooldown > 0) {
				cooldown -= (float)delta;
				MoveAndSlide();
				return;
			}

			switch (state) {
				case PufferfishState.Idle:
					ScanForPlayer();
					break;
				case PufferfishState.Charging:
					Charge();
					break;
				case PufferfishState.Inflating:
					Inflate();
					break;
				case PufferfishState.Deflating:
					Deflate();
					break;
			}
			MoveAndSlide();

			if (Velocity.X < 0) {
				inflatedCollision.GetNode<Sprite2D>("Sprite2D").FlipH = true;
				deflatedCollision.GetNode<Sprite2D>("Sprite2D").FlipH = true;
			} else if (Velocity.X > 0) {
				inflatedCollision.GetNode<Sprite2D>("Sprite2D").FlipH = false;
				deflatedCollision.GetNode<Sprite2D>("Sprite2D").FlipH = false;
			}


			cooldown = maxCooldown;
		}

		/// <summary>
		/// Scans for the player and changes the state to "Inflating" if the player is in range.
		/// </summary>
		private void ScanForPlayer() {
			if (PlayerInRange()) {
				state = PufferfishState.Inflating;
			}
		}


		/// <summary>
		/// Charge the pufferfish towards the player's position then changes its state.
		/// </summary>
		private void Charge() {
			Vector2 velocity = Velocity;
			if (player.GlobalPosition.X < GlobalPosition.X) {
				velocity.X = -100;
			} else if (player.GlobalPosition.X > GlobalPosition.X) {
				velocity.X = 100;
			}
			Velocity = velocity;
			state = PufferfishState.Deflating;
		}

		/// <summary>
		/// Inflates the pufferfish and updates the sprite and collision
		/// Sets the state to Charging.
		/// </summary>
		private void Inflate() {
			inflatedCollision.SetDeferred("disabled", false);
			deflatedCollision.SetDeferred("disabled", true);

			inflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = true;
			deflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;

			isInflated = true;

			state = PufferfishState.Charging;
		}

		/// <summary>
		/// Deflates the pufferfish and updates the sprite and collision
		/// Sets the state to Idle.
		/// </summary>
		private void Deflate() {
			inflatedCollision.SetDeferred("disabled", true);
			deflatedCollision.SetDeferred("disabled", false);

			inflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;
			deflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = true;

			isInflated = false;

			state = PufferfishState.Idle;
		}

		/// <summary>
		/// Checks if the player is within a certain range.
		/// </summary>
		/// <returns>Whether the player is within attack range</returns>
		private bool PlayerInRange() {
			if (Math.Abs(player.GlobalPosition.Y - GlobalPosition.Y) < 32) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// When a player beam hits, decrement health.
		/// </summary>
		private void Hit() {
			Health--;
		}

		/// <summary>
		/// Kills the boss by hiding it and disabling collision, then spawns a drop.
		/// Also stores it in MetSys so it won't respawn if the room is re-entered
		/// </summary>
		private async void Die() {
			state = PufferfishState.Dead;

			// Spawn item drop
			SpawnItem();

			// Hide the texture so particles are visible
			inflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;
			deflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;

			healthBar.Visible = false;

			// Spawn a set of particles
			particles.Emitting = true;

			// Disable collision
			GetNode<Area2D>("HurtBox").ProcessMode = ProcessModeEnum.Disabled;
			inflatedCollision.SetDeferred("disabled", true);
			deflatedCollision.SetDeferred("disabled", true);

			// Waits until particles are done emitting
			await ToSignal(GetTree().CreateTimer(3), "timeout");

			// Store MetSys Object
			MetSysCompat.Call("store_obj", this);

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

		/// <summary>
		/// Handles the event of hitting the player.
		/// </summary>
		/// <param name="body">The node that was hit.</param>
		private void OnHitPlayer(Node2D body) {
			if (body is Player player) {
				player.Hit(Damage);
			}
		}
	}
}
