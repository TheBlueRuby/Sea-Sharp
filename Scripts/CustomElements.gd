@tool
extends MetroidvaniaSystem.CustomElementManager

func _init() -> void:
	# Metroid-like elevator shaft.
	register_element("elevator", draw_elevator)

func draw_elevator(canvas_item: CanvasItem, coords: Vector3i, pos: Vector2, size: Vector2, _data: String):
	# For elevator to display, both top and bottom cell must be discovered.
	if not MetSys.is_cell_discovered(coords + Vector3i(0, -1, 0)) and not MetSys.is_cell_discovered(coords + Vector3i(0, size.y, 0)):
		return
	
	var elevator_texture := preload("res://Sprites/Elevator.png")
	# Draw the texture using the element's rect and default bolder color as modulate.
	canvas_item.draw_texture_rect(elevator_texture, Rect2(
		pos + Vector2.RIGHT * (MetSys.CELL_SIZE.x * 0.5 - elevator_texture.get_width() * 0.5),
		Vector2(elevator_texture.get_width(), size.y)), true, MetSys.settings.theme.default_border_color)
