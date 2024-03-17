using Godot;
using System;

public partial class KingCrab : CharacterBody2D {
	enum KingCrabState {
		MovingLeft,
		MovingRight,
		Jumping,
	}

	private const int jumpVel = -300;
	private KingCrabState state;
	private RandomNumberGenerator rng;

	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	[Export]
	public int maxHealth = 10;
	public int health;

	public GpuParticles2D particles;
	public Sprite2D sprite;
	private ProgressBar healthBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		rng = new RandomNumberGenerator();
		rng.Randomize();

		health = maxHealth;

		particles = GetNode<GpuParticles2D>("BossParticles");
		sprite = GetNode<Sprite2D>("Sprite2D");
		healthBar = GetNode<ProgressBar>("HealthBar");
	}

	public override void _PhysicsProcess(double delta) {
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
		GD.Print(health);
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


	private void OnHitPlayer(Node2D body) {
		if (body is Player player) {
			player.Hit(33);
		}
	}

	public void Hit() {
		health--;
	}

	private async void Die() {

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
}
