extends Node2D
@export var subViewport : SubViewport
@export var light : PointLight2D

@export var firstt : Texture2D
@export var secondt : Texture2D

var first : bool
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#bad()
	good()
	
func bad():
	first = !first
	if (first):
		light.texture = firstt
	else:
		light.texture = secondt
		

func good():
	await RenderingServer.frame_post_draw
	var viewport_texture : ViewportTexture = subViewport.get_texture()
	var image: Image = viewport_texture.get_image()
	print("Image is: " + str(image))
	var image_texture := ImageTexture.create_from_image(image)
	print("Created is: " + str(image_texture))
	if (image_texture == null):
		print("Null!")
	else:
		#image_texture.draw(firstt.get_rid(), Vector2(0,0))
		
		remove_child(light)
		light.texture = null
		light.texture = image_texture
		print(image_texture.get_size())
		print(image_texture.get_format())
		print("viewport type: " + str(typeof(image_texture)))
		print("it type: " + str(typeof(firstt)))
		print(light.texture.get_size())
		add_child(light)
