extends Control

var mainUI: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	mainUI = load("res://mainUI.tscn")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_backbutton_pressed():
	get_tree().change_scene_to_packed(mainUI)
