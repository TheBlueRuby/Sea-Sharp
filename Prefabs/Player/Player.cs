using System;
using Godot;
using static SeaSharp.Inventory;

namespace SeaSharp {
	public partial class Player : CharacterBody2D {
		private const float speed = 150.0f;
		private const float jump_vel = -350.0f;

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		private CollisionShape2D hitbox;
		private AnimatedSprite2D texture;

		private RectangleShape2D hb_front;
		private RectangleShape2D hb_side;
		private RectangleShape2D hb_flpr;

		private const float beamCooldown = 0.25f;
		private float beamCooldownTimer = 0f;
		private PackedScene bubbleBeam;
		private PackedScene heatBeam;
		private PackedScene iceBeam;
		private BeamTypes[] changeOrder = new BeamTypes[] { BeamTypes.None, BeamTypes.BubbleBeam, BeamTypes.HeatBeam, BeamTypes.IceBeam };

		private const float invSeconds = 1f;
		private float invTimer;

		private bool usingFlipper;
		private bool firstRun = false;

		private Node MetSys;
		private Node MetSysCompat;

		private bool shouldDarken = false;
		private CanvasItemMaterial waterMaterial;

		public int MaxHealth { get; set; } = 100;
		public int Health { get; set; } = 100;
		public Inventory Inventory { get; set; } = new();

		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			// Load the hitbox and texture.
			hitbox = GetNode<CollisionShape2D>("Hitbox");
			texture = hitbox.GetNode<AnimatedSprite2D>("Texture");
			texture.Play();

			// Load the front and side sprites and hitboxes.
			hb_front = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_front.tres");
			hb_side = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_side.tres");

			// Load the flipper sprites and hitboxes.
			hb_flpr = GD.Load<RectangleShape2D>("res://Prefabs/Player/Collision/player_side_flpr.tres");

			// Load beams
			bubbleBeam = GD.Load<PackedScene>("res://Prefabs/Player/Beams/BubbleBeam.tscn");
			heatBeam = GD.Load<PackedScene>("res://Prefabs/Player/Beams/HeatBeam.tscn");
			iceBeam = GD.Load<PackedScene>("res://Prefabs/Player/Beams/IceBeam.tscn");

			// MetSys
			MetSys = GetTree().Root.GetNode("MetSys");
			MetSysCompat = GetTree().Root.GetNode("MetSysCompat");

			// Water Overlay Material
			waterMaterial = GD.Load<CanvasItemMaterial>("res://Prefabs/Camera/Water Overlay/watermaterial.tres");
		}


		/// <summary>
		/// Runs every physics update frame
		/// </summary>
		/// <param name="delta">The elapsed time between now and the last frame</param>
		public override void _PhysicsProcess(double delta) {
			// put here because it isnt set by the time ready is called
			if (firstRun) {
				GD.Print("First Run!");
				firstRun = false;
				PackedScene dialogPopup = GD.Load<PackedScene>("res://Menus/DialogPopup.tscn");

				DialogPopup dialogPopupInstance = dialogPopup.Instantiate<DialogPopup>();
				dialogPopupInstance.StringType = "speech";
				dialogPopupInstance.StringId = "introText";
				GetTree().Root.GetNode("GameLoop/HUD").AddChild(dialogPopupInstance);

				GetTree().Root.GetNode<PauseHandler>("GameLoop/PauseHandler").SetPaused(true);
			}

			Vector2 velocity = Velocity;

			usingFlipper = Input.IsActionPressed("move_flipper") && Inventory.HasItem(ItemTypes.Flippers);
			if (usingFlipper) {
				velocity = FlipperMovement(delta, velocity);
			} else {
				velocity = Movement(delta, velocity);
			}
			// Update player
			Velocity = velocity;
			MoveAndSlide();

			// Set sprites
			SetSprite(Velocity.X, usingFlipper);

			if (beamCooldownTimer > 0) {
				beamCooldownTimer -= (float)delta;
			}
			CheckFire();

			if (GetNode<Area2D>("AnglerArea").HasOverlappingBodies()) {
				OnAnglerAreaEntered(GetNode<Area2D>("AnglerArea").GetOverlappingBodies()[0]);
			}

			if (shouldDarken) {
				waterMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Mul;
			} else {
				waterMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Add;
			}

			// Update health bar
			GetTree().Root.GetNode<ProgressBar>("GameLoop/HUD/HealthBar").Value = ((float)Health / (float)MaxHealth) * 100;

			// Count down I-Frames
			if (invTimer > 0f) {
				invTimer -= (float)delta;
			}
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
			if (Input.IsActionPressed("move_jump") && (IsOnFloor() || Inventory.HasItem(ItemTypes.Propeller))) {
				velocity.Y = jump_vel;
			}

			// Get the input direction and handle the movement/deceleration.
			float direction = Input.GetAxis("move_left", "move_right");
			if (direction != 0) {
				velocity.X = direction * speed;
			} else {
				velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			}

			return velocity;
		}

		/// <summary>
		/// Altered movement for when the flipper is equipped
		/// </summary>
		/// <param name="delta">Time since last frame in seconds</param>
		/// <param name="velocity">Current velocity</param>
		/// <returns>New velocity</returns>
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

			return velocity;

		}

		/// <summary>
		/// Sets the sprite and hitboxes based on movement
		/// </summary>
		/// <param name="xVelocity">The player's horizontal velocity</param>
		/// <param name="flipper">Whether the player has the flipper equipped</param>
		public void SetSprite(float xVelocity, bool flipper) {
			Vector2 hbScale = hitbox.Scale;

			if (xVelocity < 0) {
				// Flips the right facing sprite to face left.
				hbScale.X = -1;
			} else if (xVelocity > 0) {
				hbScale.X = 1;
			}
			hitbox.Scale = hbScale;

			if (flipper) {
				texture.Animation = "Flipper";
				hitbox.Shape = hb_flpr;
				return;

			}
			if (Math.Abs(xVelocity) > 1) {
				texture.Animation = "Walk";
			} else if (texture.Animation != "Idle") { // Don't immediately go from idle to sideways, require player input first
				texture.Animation = "Side";
			}
			hitbox.Shape = hb_side;

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
					Inventory.ModifyBeams(beam.Type, true);
					if (Inventory.ActiveBeam == BeamTypes.None) {
						Inventory.SetActiveBeam(beam.Type);
					}
					GD.Print(Inventory.BeamsOwned.PrintArray());
					break;

				case ItemPickup item:
					Inventory.ModifyItems(item.Type, true);
					Inventory.SetActiveItem(item.Type, true);
					GD.Print(Inventory.PrintOwnedItems());
					break;

				default:
					break;
			}

			// Spawn particles and delete the pickup object
			collectible.Collect();

			// Save the game
			GetTree().Root.GetNode("GameLoop").Call("save_game");

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

		/// <summary>
		/// Gets the inventory data as a formatted string.
		/// </summary>
		/// <returns>A formatted string representing the inventory data.</returns>
		public string GetInventory() {
			return Inventory.GetInventory();
		}


		/// <summary>
		/// Sets the inventory data based on the provided formatted string.
		/// </summary>
		/// <param name="saveData">A formatted string representing the inventory data.</param>
		public void SetInventory(string saveData) {
			Inventory.SetInventory(saveData);
		}

		/// <summary>
		/// Checks if the player can fire the weapon. If they can, spawn a beam.
		/// </summary>
		public void CheckFire() {
			// Shoot
			if (Input.IsActionPressed("wpn_attack") && beamCooldownTimer <= 0) {
				Beam beamInstance = null;
				// Select beam to spawn
				switch (Inventory.ActiveBeam) {
					case BeamTypes.BubbleBeam:
						beamInstance = bubbleBeam.Instantiate<Beam>();
						break;
					case BeamTypes.HeatBeam:
						beamInstance = heatBeam.Instantiate<Beam>();
						break;
					case BeamTypes.IceBeam:
						beamInstance = iceBeam.Instantiate<Beam>();
						break;
				}

				// Spawn beam
				if (beamInstance != null) {
					int beamDir = (int)hitbox.Scale.X;
					if (Input.IsActionPressed("move_jump")) {
						beamDir = 0;
					}
					beamInstance.Start(beamDir, GlobalPosition, Inventory.HasBeam(BeamTypes.PressureBeam));
					GetTree().Root.GetNode("GameLoop/Map").AddChild(beamInstance);
				}

				// Reset cooldown
				beamCooldownTimer = beamCooldown;
			}

			// Change weapon

			if (Input.IsActionJustPressed("wpn_change")) {
				ChangeWeapon();
			}
		}

		private void ChangeWeapon() {
			int index = Array.IndexOf(changeOrder, Inventory.ActiveBeam);
			bool newWeaponSelected = false;
			int repeats = 0;
			// Cycle through array until next available beam is selected
			while (!newWeaponSelected) {
				if (index >= changeOrder.Length) {
					index = 0;
				}
				if (Inventory.HasBeam(changeOrder[index])) {
					Inventory.ActiveBeam = changeOrder[index];
					newWeaponSelected = true;
				}
				index++;
				repeats++;
				if (repeats >= changeOrder.Length) {
					break;
				}
			}
		}

		/// <summary>
		/// Reduces the health of the player by the specified damage amount and starts an invincibility timer
		/// </summary>
		/// <param name="damage">The amount of damage to be applied to the player's health.</param>
		public void Hit(int damage) {
			if (invTimer <= 0) {
				Health -= damage;
				invTimer = invSeconds;
			}
		}

		/// <summary>
		/// Sets the global position to the specified position.
		/// </summary>
		/// <param name="position">The new position of the player</param>
		public void LoadPos(Vector2 position) {
			GlobalPosition = position;
		}

		/// <summary>
		/// Loads the health and max health of the player.
		/// </summary>
		/// <param name="newHealth">The new health value. Defaults to 100.</param>
		/// <param name="newMaxHealth">The new max health value. Defaults to 100.</param>
		public void LoadHealth(int newHealth = 100, int newMaxHealth = 100) {
			Health = newHealth;
			MaxHealth = newMaxHealth;
		}

		private void OnAnglerAreaEntered(Node2D body) {
			if (body is TileMap tileMap) {
				Vector2 localCoords = tileMap.ToLocal(GlobalPosition);
				Vector2I tileCoords = tileMap.LocalToMap(localCoords);
				Vector2I atlasCoords = tileMap.GetCellAtlasCoords(0, tileCoords);

				shouldDarken = false;
				if (atlasCoords.Y > 0) {
					shouldDarken = !Inventory.HasItem(ItemTypes.AnglerCap);
				}
			}
		}

	}
}
