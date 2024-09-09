shader_type canvas_item;
uniform float scroll_speed = 6.0f;
uniform vec4 line_color : source_color = vec4(1.0f, 0.044f, 0.066f, 1.0f);
uniform float line_size : hint_range(-1.0f, 1.0f);
uniform float rotation : hint_range(0, 360);
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	
	vec4 color = COLOR;
	float r = radians(rotation);
	
	float xcomponent = sin(r);
	float ycomponent = cos(r);
	float lineUV = UV.x * 20.0 * xcomponent + UV.y * 20.0 * ycomponent;
	
	float a = sin(TIME*-scroll_speed+lineUV*2.0f);
	if (a > line_size){
		a = 0.0f;
	}
	else{
		a = 1.0f;
	}
	color.a *= a;
	COLOR = color * line_color;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
