using Godot;
using System;

public partial class Crab : WalkingEnemy {
	int speed = 100;
	int jumpVel = 300;
	int maxSeeDist = 256;
	int damage = 5;

	public override void _Ready() {
		Init(speed, jumpVel, maxSeeDist, damage);
	}
}

