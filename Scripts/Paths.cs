namespace Utils {
	/// <summary>
	/// Class containing all hardcoded paths
	/// </summary>
	public static class Paths {
		/// <summary>
		/// Class containing paths in the scene tree.
		/// Used for <c>GetNode()</c> and <c>GetNodeOrNull()</c> calls 
		/// </summary>
		public static class SceneTree {
			public const string GameLoop = "GameLoop";
			public const string HUD = GameLoop + "/HUD";
			public const string Map = GameLoop + "/Map";
			public const string PauseMenu = GameLoop + "/PauseMenu";
			public const string PauseHandler = GameLoop + "/PauseHandler";
			public const string Player = GameLoop + "/Player";

			public const string MetSys = "MetSys";
			public const string MetSysCompat = "MetSysCompat";
		}

		/// <summary>
		/// Class containing paths in the filesystem.
		/// Used mainly in <c>GD.Load()</c>
		/// </summary>
		public static class Resources {
			public const string Root = "res://";
			public const string Prefabs = Root + "Prefabs/";
			public const string Strings = Root + "strings.json";
			public const string GameLoop = Root + "GameLoop.tscn";

			public const string Pickups = Prefabs + "Pickups/";
			public const string DefaultPickup = Pickups + "PickupBase.tscn";
			public const string PickupParticles = Pickups + "ParticleEffects/PickupParticles.tres";

			public const string Menus = Root + "Menus/";

			public const string PlayerPath = Prefabs + "Player/";

			public const string SaveDir = "user://";
			public const string SaveFile = "SeaSharpSave.sav";
			public const string Save = SaveDir + SaveFile;
		}
	}
}
