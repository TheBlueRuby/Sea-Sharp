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
	public CollisionShape2D inflated;
	public CollisionShape2D deflated;
	private ProgressBar healthBar;

	private Node MetSys;
	private Node MetSysCompat;

	private Node2D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		health = maxHealth;

		particles = GetNode<GpuParticles2D>("BossParticles");
		inflated = GetNode<CollisionShape2D>("Inflated");
		deflated = GetNode<CollisionShape2D>("Deflated");
		healthBar = GetNode<ProgressBar>("HealthBar");
		player = GetTree().Root.GetNode<Node2D>("GameLoop/Player");

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
	public override void _Process(double delta) {
		Vector2 velocity = Velocity;
		switch (state) {
			case PufferfishState.Dead:
				return;
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
		Velocity = velocity;
	}

	private void ScanForPlayer() {
		throw new NotImplementedException();
	}


	private void Charge() {
		throw new NotImplementedException();
		state = PufferfishState.Deflating;
	}


	private void Inflate() {
		throw new NotImplementedException();
		state = PufferfishState.Charging;
	}


	private void Deflate() {
		throw new NotImplementedException();
		state = PufferfishState.Idle;
	}

}
