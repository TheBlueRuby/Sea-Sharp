using System;
using Godot;

public partial class PickupParticles : GpuParticles2D {
	public void Burst() {
		Restart();
		SetDeferred("Emitting", true);
		Wait(0.1f);
		SetDeferred("Emitting", false);
		Wait(3f);
	}

	public async void Wait(float time) {
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
	}
}
