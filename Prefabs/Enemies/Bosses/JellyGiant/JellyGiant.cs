using Godot;
using System;

public partial class JellyGiant : CharacterBody2D {
	[Export]
	public int maxHealth = 10;
	public int health;

	public GpuParticles2D particles;
	public Sprite2D sprite;
	private ProgressBar healthBar;

	private Node MetSys;
	private Node MetSysCompat;

	private float floatingProgress;

	/// <summary>
	/// Initialization function
	/// </summary>
	public override void _Ready() {
		health = maxHealth;

		particles = GetNode<GpuParticles2D>("BossParticles");
		sprite = GetNode<Sprite2D>("Sprite2D");
		healthBar = GetNode<ProgressBar>("HealthBar");

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
		floatingProgress += (float)delta;
		Vector2 velocity = Velocity;
		
		// Float up and down
		velocity.Y = (float)Math.Sin(floatingProgress) * 5;
		// GD.Print(Velocity.Y + " - " + floatingProgress);

		Velocity = velocity;
		MoveAndSlide();

		healthBar.Value = ((float)health / (float)maxHealth) * 100;
		// GD.Print(health);
		if (health <= 0) {
			Die();
		}
	}

	/// <summary>
	/// Handles the event of hitting the player.
	/// </summary>
	/// <param name="body">The node that was hit.</param>
	private void OnHitPlayer(Node2D body) {
		if (body is Player player) {
			player.Hit(10);
		}
	}
	
	/// <summary>
	/// When a player beam hits, decrement health.
	/// </summary>
	public void Hit() {
		health--;
	}

	/// <summary>
	/// Kills the boss by hiding it and disabling collision.
	/// Also stores it in MetSys so it won't respawn if the room is re-entered
	/// </summary>
	private async void Die() {

		// Hide the texture so particles are visible
		sprite.Visible = false;
		healthBar.Visible = false;

		// Spawn a set of particles
		particles.Emitting = true;

		// Disable collision
		GetNode<Area2D>("HurtBox").ProcessMode = ProcessModeEnum.Disabled;
		GetNode<CollisionShape2D>("MovementHitbox").Disabled = true;

		// Waits until particles are done emitting
		await ToSignal(GetTree().CreateTimer(3), "timeout");

		// Store MetSys Object, ran later to avoid a crash
		MetSysCompat.Call("store_obj", this);

		QueueFree();
	}
}
