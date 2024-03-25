using Godot;
using System;

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
	public int maxHealth = 10;
	public int health;

	public GpuParticles2D particles;
	public CollisionShape2D inflatedCollision;
	public CollisionShape2D deflatedCollision;
	private ProgressBar healthBar;

	private Node MetSys;
	private Node MetSysCompat;

	private Node2D player;

	private PackedScene drop;

	private const float maxCooldown = 2f;
	private float cooldown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		health = maxHealth;
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) {
		if (state == PufferfishState.Dead) {
			return;
		}

		GD.Print(state + " in " + cooldown);

		healthBar.Value = ((float)health / (float)maxHealth) * 100;
		// GD.Print(health);
		if (health <= 0) {
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

	private void ScanForPlayer() {
		if (PlayerInRange()) {
			state = PufferfishState.Inflating;
		}
		return;
	}


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


	private void Inflate() {
		inflatedCollision.SetDeferred("disabled", false);
		deflatedCollision.SetDeferred("disabled", true);

		inflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = true;
		deflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;

		isInflated = true;

		state = PufferfishState.Charging;
	}


	private void Deflate() {
		inflatedCollision.SetDeferred("disabled", true);
		deflatedCollision.SetDeferred("disabled", false);

		inflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = false;
		deflatedCollision.GetNode<Sprite2D>("Sprite2D").Visible = true;

		isInflated = false;

		state = PufferfishState.Idle;
	}

	private bool PlayerInRange() {
		if (Math.Abs(player.GlobalPosition.Y - GlobalPosition.Y) < 32) {
			return true;
		}
		return false;
	}

	private void Hit() {
		health--;
	}

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

	private void SpawnItem() {
		Node2D droppedItem = (Node2D)drop.Instantiate();
		droppedItem.GlobalPosition = GlobalPosition;
		CallDeferred("add_sibling", droppedItem);
	}

}
