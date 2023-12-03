extends Control

@export var char_per_seconds: float = 60

@onready var text_entry: Label = $TextEntry
@onready var speak_delay: Timer = $SpeakDelay
@onready var face_icon: TextureRect = $FaceIcon
@onready var dialog_sound: AudioStreamPlayer = $DialogSound
@onready var continue_dialog_arrow: TextureRect = $ContinueDialogArrow
@onready var continue_dialog_arrow_animation: AnimationPlayer = $ContinueDialogArrow/AnimationPlayer
@onready var accept_timer: Timer = $AcceptTimer
@onready var dialog

@export var outside_dialog_box: OutsideDialogBox
@export var niko: CharacterBody2D

var _current_dialog_index: int = 0
var _conversation_finished: bool = false
var _dialog_paused: bool  = false

signal dialog_finished


func _ready() -> void:
	visible = false
	outside_dialog_box.connect("dialog_finished", _niko_dialog_finished)


func play(dialog_array) -> void:
	_current_dialog_index = 0
	
	dialog = dialog_array
	_set_next_conversation()


func _set_next_conversation() -> void:
	if _current_dialog_index > len(dialog) - 1:
		speak_delay.stop()
		visible = false
		dialog_finished.emit()
		return
	
	var character_name = dialog[_current_dialog_index][0].split("/")[0]
	
	if character_name == "niko" && niko.IsOutside:
		outside_dialog_box.play([dialog[_current_dialog_index]])
		
		_current_dialog_index += 1
		return
	else:
		visible = true
	
	text_entry.visible_characters = 0
	accept_timer.wait_time = float(dialog[_current_dialog_index][2])
	text_entry.text = dialog[_current_dialog_index][1]
	
	if character_name == "en" or character_name == "virus":
		dialog_sound.stream = load("res://assets/sounds/robot_dialog.wav")
	else:
		dialog_sound.stream = load("res://assets/sounds/normal_dialog.wav")
	
	face_icon.texture = load("res://assets/textures/faces/%s.png" % dialog[_current_dialog_index][0])
	
	_current_dialog_index += 1
	
	speak_delay.wait_time = text_entry.text.length() / (text_entry.text.length() * char_per_seconds)
	speak_delay.start()


func _niko_dialog_finished() -> void:
	_set_next_conversation()


func _process(_delta: float) -> void:
	if _conversation_finished:
		continue_dialog_arrow.visible = true
		continue_dialog_arrow_animation.play("up_and_down")
	else:
		continue_dialog_arrow.visible = false
		continue_dialog_arrow_animation.stop()
	
	if Input.is_action_just_pressed("ui_accept") && _conversation_finished:
		_set_next_conversation()
		_conversation_finished = false
	
	if Input.is_action_just_pressed("ui_accept") && _dialog_paused:
		speak_delay.start()
		_dialog_paused = false


func _on_speak_delay_timeout() -> void:
	text_entry.visible_characters += 1
	
	if text_entry.visible_characters < text_entry.text.length() - 4:
		if !dialog_sound.playing:
			dialog_sound.play()
	
	if text_entry.visible_characters == text_entry.text.length():
		speak_delay.stop()
		accept_timer.start()


func _on_accept_timer_timeout() -> void:
	_conversation_finished = true
