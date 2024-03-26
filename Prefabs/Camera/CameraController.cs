using Godot;

public partial class CameraController : Camera2D {
	[Export]
	private Node2D target;

	// Stillzone dimensions
	private const int stillWidth = 32;
	private const int stillHeight = 64;

	// Camera speed
	private const int speed = 150;
	private const int lerpFactor = 5;

	// Zoom speed
	private Vector2 zoomAmount = Vector2.One * 0.01f;

	// Zoom limits
	private Vector2 minZoom = Vector2.One;
	private Vector2 maxZoom = Vector2.One * 2;

	/// <summary>
	/// Called every frame update.
	/// </summary>
	/// <param name="delta">Time elapsed since previous frame in seconds</param>
	public override void _Process(double delta) {
		float d = (float)delta;
		Vector2 tPos = target.Position;
		Vector2 pos = Position;

		// If the target is outside the still zone, move the camera
		if (pos.X > (tPos.X + stillWidth)) {
			pos.X -= speed * d;
		}
		if (pos.X < (tPos.X - stillWidth)) {
			pos.X += speed * d;
		}

		if (pos.Y > (tPos.Y + stillHeight)) {
			pos.Y -= speed * d;
		}
		if (pos.Y < (tPos.Y - stillHeight)) {
			pos.Y += speed * d;
		}

		// If the target is not centered, smoothly recenter
		if (pos != tPos) {
			pos = pos.Lerp(tPos, lerpFactor * d);
		}

		Position = pos;

		// Camera movement
		if (Input.IsActionPressed("zoom_in") && Zoom < maxZoom) {
			Zoom += zoomAmount;
		}
		if (Input.IsActionPressed("zoom_out") && Zoom > minZoom) {
			Zoom -= zoomAmount;
		}
		if (Input.IsActionPressed("zoom_reset")) {
			Zoom = Vector2.One;
		}
	}
}
