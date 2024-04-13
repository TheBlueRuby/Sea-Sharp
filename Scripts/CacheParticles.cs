using Godot;
using System;

namespace SeaSharp {
	public partial class CacheParticles : CanvasLayer {
		ParticleProcessMaterial pickupParticles; // Particles for pickups


		public override void _Ready() {
			// Load shader data into variables
			pickupParticles = GD.Load<ParticleProcessMaterial>(Utils.Paths.Resources.PickupParticles);

			// List should be updated every new particle added
			ParticleProcessMaterial[] particleProcessMaterials = {
			pickupParticles
		};

			// Looping through array and loading the shaders into memory
			foreach (ParticleProcessMaterial particleProcessMaterial in particleProcessMaterials) {
				GpuParticles2D particlesInstance = new() {
					ProcessMaterial = particleProcessMaterial,
					OneShot = true,
					Emitting = true
				};
				AddChild(particlesInstance);
			}
		}
	}
}
