using Godot;

public partial class CameraController : Camera2D {
	[Export]
	private Node2D target;

	private const int stillWidth = 32;
	private const int stillHeight = 64;

	private const int speed = 150;
	private const int lerpFactor = 5;

	private Vector2 zoomAmount = Vector2.One * 0.01f;

	private Vector2 minZoom = Vector2.One * 0.75f;
	private Vector2 maxZoom = Vector2.One * 2;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		float d = (float)delta;
		Vector2 tPos = target.Position;
		Vector2 pos = Position;

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

		if (pos != tPos) {
			pos = pos.Lerp(tPos, lerpFactor * d);
		}

		Position = pos;

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
