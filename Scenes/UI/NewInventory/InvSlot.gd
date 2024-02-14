extends Button
class_name InvSlot

@export var label_quantity: Label
var item: Item = null
var empty: bool = true

@export_enum("PASSIVE", "HEAD", "BODY", "LEGS", "RIGHT_HAND", "LEFT_HAND") var slot_type: String = "PASSIVE"


func _ready():
	if slot_type == ConstNames.passive_slot:
		return
	else:
		text = slot_type


func update_quantity(addedQuant: int):
	item.Quantity += addedQuant;
	if (item.Quantity == 0):
		remove_item();
	#elif (item is FoodItem):
		#label_quantity.text = str(item.QUANTITY);


func put_item(added_item: Item):
	var item_info = added_item.Name + "\n";
	icon = added_item.Texture;
	item = added_item;
	#if (item is FoodItem):
		#label_quantity.text = str(item.QUANTITY);
		#item_info += "Restore: " + str(item.HEALTH_RESTORED) + " HP\n";
	#else:
	label_quantity.text = "";
	empty = false;
	
	if (item is Weapon):
		item_info += "Damage: " + str(item.Damage) + "\n";
	
	item_info += item.HoverText;
	set_tooltip_text(item_info);


func remove_item():
	icon = null
	item = null
	label_quantity.text = ""
	empty = true
	set_tooltip_text("") 
	if slot_type != ConstNames.passive_slot:
		text = slot_type

func is_empty():
	return empty


func move_item(to):
	if to.slot_type == ConstNames.passive_slot or to.slot_type == item.type:
		if to.is_empty():
			to.put_item(item)
			remove_item()
		else:
			if slot_type == ConstNames.passive_slot:
				var item_to_move = item
				put_item(to.item)
				to.put_item(item_to_move)


func equip_item(from):
	if from.item != null:
		if slot_type == from.item.Type:
	#swap items between slots
			var item_to_equip = from.item
			if not is_empty():
				from.put_item(item)
			else:
				from.remove_item()
			put_item(item_to_equip)
			


func get_preview():
	var preview_texture = TextureRect.new()
	
	preview_texture.texture = icon
	preview_texture.expand_mode = 1
	preview_texture.size = Vector2(30,30)
	
	var preview = Control.new()
	preview.add_child(preview_texture)
	
	return preview


