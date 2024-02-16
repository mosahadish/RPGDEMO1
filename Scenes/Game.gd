extends Node

func _notification(what):
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		$PlayerUI/Inventory.clear_inventory();
		get_tree().quit()
