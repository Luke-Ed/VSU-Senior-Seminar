extends Node


# Declare member variables here. Examples:
# var a = 2
var Background_music = AudioStreamPlayer.new()


# Called when the node enters the scene tree for the first time.
func _ready():
	self.add_child(Background_music)
	Background_music.stream = load ("res://Sounds/test.wav")
	Background_music.set_volume_db(-10);
	Background_music.play()
	# Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
