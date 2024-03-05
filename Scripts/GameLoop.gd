extends "res://addons/MetroidvaniaSystem/Template/Scripts/MetSysGame.gd"
class_name Game

var generated_rooms: Array[Vector3i]

func _ready():
	get_script().set_meta(&"singleton", self)
	MetSys.reset_state()
	set_player($Player)
	
	MetSys.set_save_data()
	
	room_loaded.connect(init_room, CONNECT_DEFERRED)
	load_room("SpawnRoom.tscn")
	add_module("RoomTransitions.gd")

func init_room():
	MetSys.get_current_room_instance().adjust_camera_limits($MainCamera)
