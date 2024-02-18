extends Area3D

signal picked_up_item(Item);

var item: PickUpItem;
var items_in_reach: Array[PickUpItem];
var item_to_pick: PickUpItem = null;


var connected_input
var pick_up_btn_bind = "(No input detected)";

@export var label: Label;

const PS4: String = "Sony";
const PS4_INPUT_MAP_NAME = "PS4 Controller";


func _ready():
	if (InputMap.has_action("PickUp")):
		var event = InputMap.action_get_events("PickUp");
		connected_input = handle_input_connections();
		pick_up_btn_bind = find_input_btn_name(event);


#Return Keyboard if no Joypads are connected
func handle_input_connections():
	var connected_joypads = Input.get_connected_joypads();
	var connected_joypad;
	if (connected_joypads.is_empty() == false):
		connected_joypad = connected_joypads[0];
		if (Input.get_joy_name(connected_joypad) == PS4_INPUT_MAP_NAME):
			return PS4;
	else:
		return "Keyboard";


func find_input_btn_name(event):
	event = event[0].as_text();
	var split_event_buttons = event.split(",");
	
	for s in split_event_buttons:
		if (s.contains(connected_input)):
			if (s.split(" ")[-1] == "Cross"):
				return "X";
	return "";


func _process(_delta) -> void:
	find_closest_item();
	
	if (Input.is_action_just_pressed("PickUp")):
		if (item_to_pick != null):
			picked_up_item.emit(item_to_pick.GetItem());
			
	if (item_to_pick != null):
		label.text = pick_up_btn_bind + ": Pick Up Item";
		label.show();
	else: 
		label.hide();
		label.text = "";


func _on_area_entered(area):
	item = area.get_parent();
	items_in_reach.append(item);


func _on_area_exited(area):
	item = area.get_parent();
	if (items_in_reach.has(item)):
		items_in_reach.erase(item);
		if (item == item_to_pick):
			item_to_pick = null;


func find_closest_item():
	var shortest_distance = 9999;
	var dist;
	for itm in items_in_reach:
		dist = position.distance_to(itm.position);
		if (dist < shortest_distance):
			shortest_distance = dist;
			item_to_pick = itm;


#func _on_body_entered(body):
	#item = body
	#items_in_reach.append(item);
#
#
#func _on_body_exited(body):
	#item = body
	#if (items_in_reach.has(item)):
		#items_in_reach.erase(item);
		#if (item == item_to_pick):
			#item_to_pick = null;
