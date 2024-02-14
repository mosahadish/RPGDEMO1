extends Item
class_name WeaponItem

@export var DAMAGE: float
@export_enum("RANGED_2H", "MELEE_2H", "MELEE_1H", "MAGIC_1H", "OFFHAND") var WEAPON_TYPE: String
@export_enum("Bow", "Sword", "Staff", "Shield", "Greatsword") var SUB_TYPE: String
@export var LIGHT_STAMINA_CONS: float
@export var HEAVY_STAMINA_CONS: float
@export var effect: Node3D
@export var FireAttunement: Node3D;
@export var LightningAttunement: Node3D;

var bodies: Array[Actor]
var can_damage: bool = false
var can_block: bool = false



@export var hit_area: Area3D #The damaging area of the weapon, set monitoring off

func _ready():
	if has_node("HitArea"):
		if (hit_area.body_entered.get_connections().is_empty()):
			hit_area.body_entered.connect(_body_entered_hit_area)
		if (hit_area.body_exited.get_connections().is_empty()):
			hit_area.body_exited.connect(_body_exited_hit_area)
	#if wielder is Enemy: #Enemies can't hit each other
		#hit_area.set_collision_mask_value(2, false)


func _body_entered_hit_area(body):
	if body is Actor and body != wielder:
		bodies.append(body)
		if can_damage:
			#if (wielder is Player):
				#Mostly for camera shake
				#Events.player_hit_enemy.emit();
			for body_to_hit in bodies:
				body_to_hit.onHit(DAMAGE, wielder.global_position, SUB_TYPE)
			can_damage = false


func _body_exited_hit_area(body):
	if body is Actor:
		if bodies.has(body):
			bodies.erase(body)


func _switch_can_damage(weapon_node):
	bodies.clear();
	if weapon_node != null:
		if weapon_node.has_node("HitArea"):
			hit_area.set_deferred("monitoring", true)
	can_damage = true


func set_attunement_effect(attun: String):
	if (attun == "None"):
		FireAttunement.hide();
		LightningAttunement.hide();
	elif (attun == "Lightning"):
		FireAttunement.hide();
		LightningAttunement.show();
	else:
		FireAttunement.show();
		LightningAttunement.hide();
