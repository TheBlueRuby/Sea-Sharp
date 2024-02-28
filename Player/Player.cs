using Godot;

public partial class Player : CharacterBody2D {
	private const float speed = 150.0f;
	private const float jump_vel = -300.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private CollisionShape2D hitbox;
	private Sprite2D texture;

	private CompressedTexture2D front_sprite;
	private CompressedTexture2D side_sprite;
	private ConvexPolygonShape2D front_hitbox;
	private ConvexPolygonShape2D side_hitbox;

	public override void _Ready() {
		// Load the hitbox and texture.
		hitbox = GetNode<CollisionShape2D>("Hitbox");
		texture = hitbox.GetNode<Sprite2D>("Texture");

		// Load the front and side sprites and hitboxes.
		front_sprite = GD.Load<CompressedTexture2D>("res://Player/front.png");
		side_sprite = GD.Load<CompressedTexture2D>("res://Player/side.png");
		front_hitbox = GD.Load<ConvexPolygonShape2D>("res://Player/Collision/player_front.tres");
		side_hitbox = GD.Load<ConvexPolygonShape2D>("res://Player/Collision/player_side.tres");
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;

		// Add gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionPressed("move_jump") && IsOnFloor())
			velocity.Y = jump_vel;

		// Get the input direction and handle the movement/deceleration.
		float direction = Input.GetAxis("move_left", "move_right");
		if (direction != 0) {
			velocity.X = direction * speed;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
		}

		// Set the facing sprites.
		if (velocity.X == 0) {
			SetFacing("front");
		} else {
			SetFacing(velocity.X > 0 ? "right" : "left");
		}

		// Update player
		Velocity = velocity;
		MoveAndSlide();
	}

	public void SetFacing(string side) {
		Vector2 hbScale = hitbox.Scale;
		switch (side) {
			case "front":
				hbScale.X = 1;
				texture.Texture = front_sprite;
				hitbox.Shape = front_hitbox;
				break;
			case "left":
				// Flips the right facing sprite to face left.
				hbScale.X = -1;
				texture.Texture = side_sprite;
				hitbox.Shape = side_hitbox;
				break;
			case "right":
				hbScale.X = 1;
				texture.Texture = side_sprite;
				hitbox.Shape = side_hitbox;
				break;
			default:
				break;
		}
		hitbox.Scale = hbScale;
	}

	private void OnPickupAreaBodyEntered(Node2D body)
	{
		if (body is Pickup pickup)
		{
			switch (pickup.pickupType) {
				case "test":
					GD.Print("Test pickup collected");
					break;
				case "health":
					// Add health to the player
					break;
				case "ammo":
					// Add ammo to the player
					break;
				case "healthMax":
					// Increase the player's max health
					break;
				case "ammoMax":
					// Increase the player's max ammo
					break;
				default:
					break;
			}

			// Spawn particles and delete the pickup object
			pickup.Collect();
		} else {
			GD.Print("Not a pickup");
		}
	}
}
