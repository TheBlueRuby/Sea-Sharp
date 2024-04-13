extends "res://addons/MetroidvaniaSystem/Template/Scripts/MetSysGame.gd"
class_name Game

const SaveManager = preload("res://addons/MetroidvaniaSystem/Template/Scripts/SaveManager.gd")
const SAVE_PATH = "user://SeaSharpSave.sav"


func _ready():
	var spawn_room = ""
	get_script().set_meta(&"singleton", self)
	MetSys.reset_state()
	set_player($Player)

	if FileAccess.file_exists(SAVE_PATH):
		# If save data exists, load it using MetSys SaveManager.
		var save_manager := SaveManager.new()
		save_manager.load_from_text(SAVE_PATH)

		var playerPos = save_manager.get_value("player_pos")
		if playerPos == null:
			playerPos = Vector2(160, 90)
		player.LoadPos(playerPos)

		player.LoadHealth(save_manager.get_value("health"), save_manager.get_value("max_health"))

		player.SetInventory(save_manager.get_value("inventory"))
		spawn_room = save_manager.get_value("current_room")

	else:
		# If no data exists, set empty one.
		player.LoadHealth(100, 100)
		player.firstRun = true
		MetSys.set_save_data()

	room_loaded.connect(init_room, CONNECT_DEFERRED)
	if spawn_room == "":
		print("spawnroom empty")
		spawn_room = "SpawnRoom.tscn"
		player.LoadPos(Vector2(160, 112))
	load_room(Game.get_uid_room(spawn_room))
	add_module("RoomTransitions.gd")


# Save game using MetSys SaveManager.
func save_game():
	var save_manager := SaveManager.new()
	save_manager.set_value("player_pos", player.global_position)
	save_manager.set_value("health", player.Health)
	save_manager.set_value("max_health", player.MaxHealth)
	save_manager.set_value("current_room", MetSys.get_current_room_name())
	save_manager.set_value("inventory", player.GetInventory())
	save_manager.save_as_text(SAVE_PATH)


func init_room():
	MetSys.get_current_room_instance().adjust_camera_limits($MainCamera)


static func get_singleton() -> Game:
	return (Game as Script).get_meta(&"singleton") as Game


# If room is saved with UID then grab the room's name for loading
static func get_uid_room(uid: String) -> String:
	if not uid.begins_with(":"):
		return uid

	return ResourceUID.get_id_path(ResourceUID.text_to_id(uid.replace(":", "uid://"))).trim_prefix(
		MetSys.settings.map_root_folder
	)
