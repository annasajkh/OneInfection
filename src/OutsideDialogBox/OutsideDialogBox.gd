extends Node2D
class_name OutsideDialogBox

@export var char_per_seconds: float = 60

@onready var text_entry: Label = $TextEntry
@onready var speak_delay: Timer = $SpeakDelay
@onready var dialog_sound: AudioStreamPlayer = $DialogSound
@onready var accept_timer: Timer = $AcceptTimer

@onready var dialog

var _current_dialog_index: int = 0
var _conversation_finished: bool = false
var _dialog_paused: bool  = false

signal dialog_finished
signal conversation_finished

func play(dialog_array) -> void:
	_current_dialog_index = 0
	
	dialog = dialog_array
	
	visible = true
	_set_next_conversation()


func _set_next_conversation() -> void:
	if _current_dialog_index > len(dialog) - 1:
		speak_delay.stop()
		visible = false
		dialog_finished.emit()
		return
	
	text_entry.visible_characters = 0
	
	accept_timer.wait_time = float(dialog[_current_dialog_index][2])
	text_entry.text = dialog[_current_dialog_index][1]
	dialog_sound.stream = load("res://assets/sounds/normal_dialog.wav")
	
	_current_dialog_index += 1
	
	speak_delay.wait_time = text_entry.text.length() / (text_entry.text.length() * char_per_seconds)
	speak_delay.start()


func _ready() -> void:
	visible = false


func _process(_delta: float) -> void:
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
