#version 330

in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;

uniform sampler2D s_texture;

out vec4 Color;
 
void main()
{
	Color = vec4(0,0,0,1.0);
	
	vec4 fvBaseColour = texture2D(s_texture, v_TexCoord);

	Color = fvBaseColour;
}