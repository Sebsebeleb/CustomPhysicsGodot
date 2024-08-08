shader_type canvas_item;

uniform float scrolling_speed = 1.7f;

void fragment() {
	vec2 moving_uv = vec2(UV.x-(TIME * scrolling_speed), UV.y);
	vec4 color = vec4(COLOR.r, COLOR.g, COLOR.b, 1);
	vec4 in_tex = texture(TEXTURE, moving_uv);
	vec4 rgb = vec4(0.67f, 1.23f, 1.4f, 1.0f);
	COLOR = in_tex * rgb;
}
