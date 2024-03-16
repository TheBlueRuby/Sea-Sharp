using Godot;
using System;

public partial class Crab : WalkingEnemy {
	int speed = 100;
	int jumpVel = 300;
	int maxSeeDist = 256;

	public override void _Ready() {
		Init(speed, jumpVel, maxSeeDist);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
	}
}