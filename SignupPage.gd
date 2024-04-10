extends Control

var login_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	login_scene = load("res://loginUI.tscn")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_goto_login_pressed():
	get_tree().change_scene_to_packed(login_scene)

func _on_sign_up_button_pressed():
	get_tree().change_scene_to_packed(login_scene)
