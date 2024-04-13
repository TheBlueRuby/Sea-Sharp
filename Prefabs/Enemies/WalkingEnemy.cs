using Godot;
using System;

namespace SeaSharp {
	public partial class WalkingEnemy : CharacterBody2D {
		[Export]
		public int SPEED { get; set; } = 100;

		[Export]
		public int JUMP_VEL { get; set; } = 450;

		[Export]
		public int MAX_SEE_DIST { get; set; } = 256;

		[Export]
		public int DAMAGE { get; set; }

		[Export]
		public Node2D target { get; set; }

		private NavigationAgent2D navAgent;
		private RayCast2D playerScanner;

		private float gravity;

		private bool canSeePlayer = false;

		public override void _Ready() {
			Init();
		}

		/// <summary>
		/// Initialization function
		/// </summary>
		public void Init(int speed = 100, int jumpVel = 450, int maxSeeDist = 256, int damage = 5) {
			navAgent = GetNode<NavigationAgent2D>("NavAgent");
			playerScanner = GetNode<RayCast2D>("LineOfSight");
			target ??= GetTree().Root.GetNode<Node2D>(Utils.Paths.SceneTree.Player);

			gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");


			SPEED = speed;
			JUMP_VEL = jumpVel;
			MAX_SEE_DIST = maxSeeDist;
			DAMAGE = damage;
		}

		/// <summary>
		/// Called every physics update.
		/// </summary>
		/// <param name="delta">Time elapsed since previous frame in seconds</param>
		public override void _PhysicsProcess(double delta) {
			Vector2 dir = ToLocal(navAgent.GetNextPathPosition()).Normalized();
			Vector2 velocity = Velocity;
			velocity.X = dir.X * SPEED;

			if (!IsOnFloor()) {
				velocity.Y += gravity * (float)delta;
			} else {
				velocity.Y += JUMP_VEL * dir.Y;
			}

			Velocity = velocity;

			MoveAndSlide();
		}

		/// <summary>
		/// Method to make the path by setting the navAgent's TargetPosition to the player's GlobalPosition.
		/// </summary>
		private void MakePath() {
			navAgent.TargetPosition = target.GlobalPosition;
		}

		/// <summary>
		/// Scan for player and make a new path every few seconds
		/// </summary>
		private void _OnTimerTimeout() {
			ScanForPlayer();
			if (canSeePlayer) {
				MakePath();
			}
		}

		/// <summary>
		/// If there is a line of sight to the player and within visual range, update canSeePlayer
		/// </summary>  
		private void ScanForPlayer() {
			canSeePlayer = false;
			playerScanner.TargetPosition = playerScanner.ToLocal(target.GlobalPosition);

			if (playerScanner.IsColliding() && playerScanner.GetCollider() == target) {
				Vector2 playerDist = playerScanner.GetCollisionPoint() - playerScanner.GlobalPosition;
				if (playerDist.Length() < MAX_SEE_DIST) {
					canSeePlayer = true;
				}

			}
		}

		/// <summary>
		/// When hit by a player beam, delete
		/// </summary>
		private void Hit() {
			QueueFree();
		}

		/// <summary>
		/// Handles the event of hitting the player.
		/// </summary>
		/// <param name="body">The node that was hit.</param>
		private void OnHitPlayer(Node2D body) {
			if (body is Player player) {
				player.Hit(DAMAGE);
			}
		}
	}

}
