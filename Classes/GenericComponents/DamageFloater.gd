extends Node3D
class_name DamageFloater

@export var label: Label3D
@export var animation: AnimationPlayer

func display_damage(damage: float, pos: Vector3):
	label.text = str(damage)
	var start_pos = Vector3(pos.x, pos.y, pos.z)
	animation.play("FloatDamage")
	var tween = get_tree().create_tween()
	var end_pos = Vector3(randf_range(-1.0,1.0),randf_range(1,2), randf_range(-0.2,0.7)) + start_pos
	var tween_length = animation.get_animation("FloatDamage").length
	
	tween.tween_property(self,"global_position",end_pos,tween_length).from(start_pos)


func remove() -> void:
	animation.stop()
	queue_free()
