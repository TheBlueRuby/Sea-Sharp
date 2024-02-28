using Godot;
using System;

public partial class CacheParticles : CanvasLayer
{
	ParticleProcessMaterial pickupParticles; // Particles for pickups
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pickupParticles = GD.Load<ParticleProcessMaterial>("res://Pickups/ParticleEffects/PickupParticles.tres");

		// List should be updated every new particle added
		ParticleProcessMaterial[] particleProcessMaterials = {
			pickupParticles
		};
		
		// Looping through array and loading the shaders
		foreach (ParticleProcessMaterial particleProcessMaterial in particleProcessMaterials)
		{
			GpuParticles2D particlesInstance = new() {
				ProcessMaterial = particleProcessMaterial,
				OneShot = true,
				Emitting = true
			};
			AddChild(particlesInstance);
		}
	}
}
