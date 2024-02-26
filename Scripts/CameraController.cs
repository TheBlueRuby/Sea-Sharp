using Godot;

public partial class CameraController : Camera2D {
	[Export]
	private Node2D target;

	private int stillWidth = 32;
	private int stillHeight = 64;

	private int speed = 150;
	private int lerpFactor = 5;

	private Vector2 zoomAmount = new(0.01f, 0.01f);

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

		if (Input.IsActionPressed("zoom_in")) {
			Zoom += zoomAmount;
		}
		if (Input.IsActionPressed("zoom_out")) {
			Zoom -= zoomAmount;
		}
		if (Input.IsActionPressed("zoom_reset")) {
			Zoom = Vector2.One;
		}
	}
}
