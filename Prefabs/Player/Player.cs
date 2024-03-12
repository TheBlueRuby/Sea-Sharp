using System;
using Godot;
using static Inventory;

public partial class Player : CharacterBody2D {
	private const float speed = 150.0f;
	private const float jump_vel = -350.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private CollisionShape2D hitbox;
	private Sprite2D texture;

	private CompressedTexture2D spr_front;
	private CompressedTexture2D spr_side;
	private RectangleShape2D hb_front;
	private RectangleShape2D hb_side;

	private CompressedTexture2D spr_flpr;
	private RectangleShape2D hb_flpr;

	public Inventory inventory = new();

	private bool usingFlipper;

	public override void _Ready() {
		// Load the hitbox and texture.
		hitbox = GetNode<CollisionShape2D>("Hitbox");
		texture = hitbox.GetNode<Sprite2D>("Texture");

		// Load the front and side sprites and hitboxes.
		spr_front = GD.Load<CompressedTexture2D>("res://Prefabs/Player/front.png");
		spr_side = GD.Load<CompressedTexture2D>("res://Prefabs/Player/side.png");
		hb_front = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_front.tres");
		hb_side = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_side.tres");

		// Load the flipper sprites and hitboxes.
		spr_flpr = GD.Load<CompressedTexture2D>("res://Prefabs/Player/side_flpr.png");
		hb_flpr = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_side_flpr.tres");
	}

	/// <summary>
	/// Runs every physics update frame
	/// </summary>
	/// <param name="delta">The elapsed time between now and the last frame</param>
	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;

		usingFlipper = Input.IsActionPressed("move_flipper") && inventory.HasItem(ItemTypes.Flippers);
		if (usingFlipper) {
			velocity = FlipperMovement(delta, velocity);
		} else {
			velocity = Movement(delta, velocity);
		}
		// Update player
		Velocity = velocity;
		MoveAndSlide();
	}

	/// <summary>
	/// Handles the movement of the player based on input and delta time.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	/// <param name="velocity">The current velocity of the player.</param>
	/// <returns>The updated velocity of the player.</returns>
	private Vector2 Movement(double delta, Vector2 velocity) {
		// Add gravity.
		if (!IsOnFloor()) {
			velocity.Y += gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionPressed("move_jump") && IsOnFloor()) {
			velocity.Y = jump_vel;
		}

		// Get the input direction and handle the movement/deceleration.
		float direction = Input.GetAxis("move_left", "move_right");
		if (direction != 0) {
			velocity.X = direction * speed;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
		}

		// Set the facing sprites.
		if (velocity.X == 0) {
			SetSprite("front", false);
		} else {
			SetSprite(velocity.X > 0 ? "right" : "left", false);
		}

		return velocity;
	}

	private Vector2 FlipperMovement(double delta, Vector2 velocity) {
		// Add gravity.
		if (!IsOnFloor()) {
			velocity.Y += gravity * (float)delta * 0.5f;
		}

		// Handle Jump.
		if (Input.IsActionPressed("move_jump") && IsOnFloor()) {
			velocity.Y = jump_vel * 0.75f;
		}

		// Get the input direction and handle the movement/deceleration.
		float direction = Input.GetAxis("move_left", "move_right");
		if (direction != 0) {
			velocity.X = direction * speed;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, 5f);
		}

		// Set the facing sprites.
		SetSprite(velocity.X > 0 ? "right" : "left", true);


		return velocity;

	}

	/// <summary>
	/// Sets the sprite and hitboxes based on movement
	/// </summary>
	/// <param name="side">The direction the player is facing</param>
	public void SetSprite(string side, bool flipper) {
		Vector2 hbScale = hitbox.Scale;

		if (side == "left") {
			// Flips the right facing sprite to face left.
			hbScale.X = -1;
		} else {
			hbScale.X = 1;
		}

		if (flipper) {
			texture.Texture = spr_flpr;
			hitbox.Shape = hb_flpr;

		} else {
			if (side == "front") {
				texture.Texture = spr_front;
				hitbox.Shape = hb_front;
			} else {
				texture.Texture = spr_side;
				hitbox.Shape = hb_side;
			}
		}
		hitbox.Scale = hbScale;
	}

	/// <summary>
	/// Called when an item on the "Pickup" collision layer collides with the player
	/// </summary>
	/// <param name="body">The object that collided with the player</param>
	private void OnPickupAreaBodyEntered(Node2D body) {
		if (body is not Collectible collectible) {
			GD.Print("Not collectible");
			return;
		}

		switch (collectible) {
			case Pickup pickup:
				CollectPickup(pickup);
				break;

			case BeamPickup beam:
				inventory.ModifyBeams(beam.Type, true);
				GD.Print(inventory.BeamsOwned.PrintArray());
				break;

			case ItemPickup item:
				inventory.ModifyItems(item.Type, true);
				GD.Print(inventory.ItemsOwned.PrintArray());
				break;

			default:
				break;
		}

		// Save the game
		GetTree().Root.GetNode("GameLoop").Call("save_game");

		// Spawn particles and delete the pickup object
		collectible.Collect();

	}

	/// <summary>
	/// Called to collect the pickup and apply its effects
	/// </summary>
	/// <param name="pickup">A copy of the pickup's data</param>
	private static void CollectPickup(Pickup pickup) {
		switch (pickup.Type) {
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
	}

	public string GetInventory() {
		return $"{inventory.ItemsOwned.PrintArray()}-{inventory.BeamsOwned.PrintArray()}-{inventory.ActiveBeam}-{inventory.ActiveItems.PrintArray()}";
	}

	
	public void SetInventory(string saveData) {
		string[] saveFields = saveData.Split("-");
		inventory.ItemsOwned = BitArray.FromString(saveFields[0]);
		inventory.BeamsOwned = BitArray.FromString(saveFields[1]);
		inventory.ActiveBeam = (BeamTypes)Enum.Parse(typeof(BeamTypes), saveFields[2]);
		inventory.ActiveItems = BitArray.FromString(saveFields[3]);
	}
}
