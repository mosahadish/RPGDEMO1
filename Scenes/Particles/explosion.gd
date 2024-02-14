extends Node3D
@export var sparks: GPUParticles3D
@export var smoke: GPUParticles3D
@export var main_fire : GPUParticles3D
@export var light :OmniLight3D


func one_shot():
	sparks.emitting = true
	smoke.emitting = true
	main_fire.emitting = true
	light.visible = true
	await get_tree().create_timer(main_fire.lifetime).timeout
	light.visible = false
	

func get_time():
	return main_fire.lifetime

#
#func _on_area_3d_body_entered(body):
	#one_shot()
