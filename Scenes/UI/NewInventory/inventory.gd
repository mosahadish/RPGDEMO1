extends Control
class_name Inventory

signal equipped_item(Item);
signal unequipped_item(Item);
signal drawn_weapon(Weapon);

@export var player: CharacterBody3D;

@export var inventory: GridContainer
@export var equipment: GridContainer
@export var hotbar: GridContainer

@export_category("Equipment Slot")
@export var head_slot: InvSlot
@export var body_slot: InvSlot
@export var legs_slot: InvSlot
@export var right_hand: InvSlot
@export var left_hand: InvSlot

@export var hot_bar1: InvSlot
@export var hot_bar2: InvSlot
@export var hot_bar3: InvSlot
@export var hot_bar4: InvSlot

var equip: Dictionary

var current_slot
var from_slot
var to_slot
var dragging: bool = false
var preview = null

func _ready():
	equip = {
	ConstNames.head_slot : head_slot,
	ConstNames.body_slot : body_slot,
	ConstNames.legs_slot : legs_slot,
	ConstNames.right_hand_slot : right_hand,
	ConstNames.left_hand_slot : left_hand
	}
	
	await owner.ready;
	
	drawn_weapon.connect(player.OnInventoryChangedWeapon);
	equipped_item.connect(player.OnInventoryEquippedItem);
	unequipped_item.connect(player.OnInventoryUnequippedItem);
	
	Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN)
	#var data = GameData;
	#add_item(GameData.FetchItem("greatsword")
	#add_item(data.FetchItem("Shield"));
	#add_item(data.FetchItem("Sword"))
	#add_item(data.FetchItem("Bow"))
	#add_item(GameData.FetchItem("cookie"))
	#add_item(GameData.FetchItem("helmet"))
	close()
	#open();

func _exit_tree():
	clear_inventory(); #clean up orphan nodes

func _process(_delta):
	handles_drag_and_drop()
	if (player.IsDodging()): return;
	if Input.is_action_just_pressed("ui_accept"):
		if not current_slot.is_empty():
			#check if current slot is an equip slot
			if current_slot in equip.values():
				#remove equipment and put it in first empty inventory slot
				for slot in inventory.get_children():
					if slot.is_empty():
						current_slot.move_item(slot)
						unequipped_item.emit(slot.item);
						#Events.unequip_item.emit(slot.item)
						break
			elif is_equipable(current_slot.item):
				#get the equip slot for the item
				var equip_slot = equip[current_slot.item.Type]
				#if it's not empty, remove the currently equipped item
				if not equip_slot.is_empty():
					unequipped_item.emit(equip_slot.item);
					#Events.unequip_item.emit(equip_slot.item)
				#equip the item in the correct slot
				equip_slot.equip_item(current_slot);
				equipped_item.emit(equip_slot.item);
				#Events.equip_item.emit(equip_slot.item)
			#elif is_consumable((current_slot.item)):
				#Events.attempted_item_use.emit(current_slot)


func is_equipable(item: Item):
	#check if it's an equipment
	#if item is ArmorItem:
		#return true
	if item is Weapon:
		return true
	#later add: check if it's proper level/armor class etc
	
#func is_consumable(item: Item):
	#if item is FoodItem:
		#return true

func _unhandled_input(event):
	if event is InputEventJoypadButton:
		if InputMap.action_has_event("open_inventory", event):
			if event.is_pressed():
				if not visible:
					open()
				else:
					close()
	if (event is InputEventJoypadButton):
		if (InputMap.event_is_action(event, "DrawRightWeapon")):
			if (event.pressed):
				if (equip[ConstNames.right_hand_slot].is_empty() == false):
					drawn_weapon.emit(equip[ConstNames.right_hand_slot].item);
		if (InputMap.event_is_action(event, "DrawLeftWeapon")):
			if (event.pressed):
				if (equip[ConstNames.left_hand_slot].is_empty() == false):
					drawn_weapon.emit(equip[ConstNames.left_hand_slot].item);

func clear_inventory():
	print("Freeing inventory:")
	for slot in inventory.get_children():
		if (slot.is_empty() == false):
			print(str(slot.item) + " freed");
			slot.item.queue_free();
	for slot in equipment.get_children():
		if (slot.is_empty() == false):
			print(str(slot.item) + " freed");
			slot.item.queue_free();
	for slot in hotbar.get_children():
		if (slot.is_empty() == false):
			print(str(slot.item) + " freed");
			slot.item.queue_free();

func add_item(item: Item):
	for slot in inventory.get_children():
		if slot.is_empty():
			slot.put_item(item)
			break


func open():
	show()
	inventory.get_child(0).grab_focus()
	current_slot = inventory.get_child(0)
	warp_mouse((current_slot.get_parent().position + current_slot.position - Vector2(40,-10)))
	set_process(true)
	get_viewport().gui_focus_changed.connect(_on_focus_changed)
	
	#Events.inventory_okpened.emit()


func close():
	hide()
	set_process(false)
	if get_viewport().gui_focus_changed.is_connected(_on_focus_changed):
		get_viewport().gui_focus_changed.disconnect(_on_focus_changed)
		
	#Events.inventory_closed.emit()

	
func handles_drag_and_drop():
	if Input.is_action_just_pressed("ui_select"):
		if not dragging:
			drag()
		else:
			drop()
	elif Input.is_action_just_pressed("ui_cancel"):
		if dragging:
			cancel_drag()
			
			
func drag():
	if not current_slot.is_empty():
		from_slot = current_slot
		from_slot.disabled = true
		dragging = true


func cancel_drag():
	from_slot.disabled = false
	dragging = false


func drop():
	to_slot = current_slot
	from_slot.disabled = false
	from_slot.move_item(to_slot)
	dragging = false


func _on_focus_changed(control:Control) -> void:
	if control != null:
		current_slot = control
		var mouse_pos = (current_slot.get_parent().position + current_slot.position - Vector2(40,-10))
		warp_mouse(mouse_pos)


#func _on_player_sheath_weapon(side: String):
	#if side == "right_weapon":
		#Events.player_current_weapon.emit(equip[ConstNames.right_hand_slot].item)
	#elif side == "left_weapon":
		#Events.player_current_weapon.emit(equip[ConstNames.left_hand_slot].item)
		
func get_right_hand():
	return equip[ConstNames.right_hand_slot]
	

func get_left_hand():
	return equip[ConstNames.left_hand_slot]
		
func _on_player_used_item(slot: InvSlot):
	slot.update_quantity(-1)
	
	
func _on_picked_up_item(new_item: Item):
	var item_parent = new_item.get_parent();
	if (item_parent != null):
		new_item.global_position = Vector3.ZERO;
		item_parent.remove_child(new_item);

	add_item(new_item);
