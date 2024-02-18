extends Node

var items: Dictionary
var scenes: Dictionary
var enemies: Dictionary
var projectiles: Dictionary
var sound_effects: Dictionary

const DIR_ENEMIES_PATH: String = "res://Scenes/Models/Enemies/"
const DIR_ITEMS_PATH: String = "res://Scenes/Items/"
const DIR_PROJECTILES_PATH: String = "res://Scenes/Projectiles/"
const DIR_SOUND_PATH: String = "res://Resources/SoundEffects/"


func _ready():
	items = get_dir_contents(DIR_ITEMS_PATH, ".tscn")
	#enemies = get_dir_contents(DIR_ENEMIES_PATH, ".tscn")
	#projectiles = get_dir_contents(DIR_PROJECTILES_PATH, ".tscn")
	#sound_effects = get_dir_contents(DIR_SOUND_PATH, ".mp3")
	#sound_effects.merge(get_dir_contents(DIR_SOUND_PATH, ".wav"))
	
func FetchProjectile(proj_name: String):
	if projectiles.has(proj_name):
		var proj = projectiles[proj_name].instantiate()
		return proj
	
func FetchItem(item_name: String):
	if items.has(item_name):
		var item = items[item_name].instantiate()
		return item

func FetchEnemyScene(enemy_scene: String):
	if enemies.has(enemy_scene):
		return enemies[enemy_scene]

func FetchSound(sound: String):
	if sound_effects.has(sound):
		return sound_effects[sound]
	else:
		return null

func get_dir_contents(path, files_suffix):
	var dir = DirAccess.open(path)
	return iterate_over_dir(dir, path, files_suffix)

func iterate_over_dir(dir: DirAccess,path, files_suffix):
	var ret: Dictionary = {}
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if dir.current_is_dir():
				var new_dir = DirAccess.open(path+file_name)
				ret.merge(iterate_over_dir(new_dir,path+file_name+"/",files_suffix))
			else:
				file_name = file_name.replace('.remap', '') 
				file_name = file_name.replace(".import", "") 
				if file_name.ends_with(files_suffix):
					print("Loading file: " + file_name)
					ret[file_name.left(-files_suffix.length())] = load(path+file_name)
			file_name = dir.get_next()
		return ret
	else:
		print("An error occurred when trying to access the path.")
