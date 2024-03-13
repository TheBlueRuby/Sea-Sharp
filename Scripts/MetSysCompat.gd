extends Node

func store_obj(object: Object) -> void:
	MetSys.store_object(object)

func register_obj_marker(object: Object) -> bool:
	MetSys.register_storable_object_with_marker(object)
	return MetSys.save_data.is_object_stored(object)

func save_game() -> void:
	Game.get_singleton().save_game()
