using Godot;
using System;

public partial class WalkingEnemy : CharacterBody2D {
	[Export]
	public int SPEED = 100;

	[Export]
	public int JUMP_VEL = 450;

	[Export]
	public int MAX_SEE_DIST = 256;

	[Export]
	public int DAMAGE;

	[Export]
	public Node2D player;

	private NavigationAgent2D navAgent;
	private RayCast2D playerScanner;

	private float gravity;

	private bool canSeePlayer = false;

	public override void _Ready() {
		Init();
	}

	public void Init(int speed = 100, int jumpVel = 450, int maxSeeDist = 256, int damage = 5) {
		navAgent = GetNode<NavigationAgent2D>("NavAgent");
		playerScanner = GetNode<RayCast2D>("LineOfSight");
		player ??= GetTree().Root.GetNode<Node2D>("GameLoop/Player");

		gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");


		SPEED = speed;
		JUMP_VEL = jumpVel;
		MAX_SEE_DIST = maxSeeDist;
		DAMAGE = damage;
	}

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

	private void MakePath() {
		navAgent.TargetPosition = player.GlobalPosition;
	}

	private void _OnTimerTimeout() {
		ScanForPlayer();
		if (canSeePlayer) {
			MakePath();
		}
	}

	private void ScanForPlayer() {
		canSeePlayer = false;
		playerScanner.TargetPosition = playerScanner.ToLocal(player.GlobalPosition);

		if (playerScanner.IsColliding()) {
			if (playerScanner.GetCollider() == player) {
				Vector2 playerDist = playerScanner.GetCollisionPoint() - playerScanner.GlobalPosition;
				if (playerDist.Length() < MAX_SEE_DIST) {
					canSeePlayer = true;
				}
			}
		}
	}

	private void Hit() {
		QueueFree();
	}

	private void OnHitPlayer(Node2D body) {
		if (body is Player player) {
			player.Hit(DAMAGE);
		}
	}
}

