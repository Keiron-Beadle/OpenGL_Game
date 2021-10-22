#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec3 a_Normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 v_TexCoord;
out vec3 v_Normal;
out vec3 v_FragPos;

void main()
{
	v_FragPos = vec3(model * vec4(a_Position,1.0));
	v_TexCoord = a_TexCoord;
	v_Normal = normalize(vec3( transpose(inverse(model)) * vec4(a_Normal,1.0) ));
	gl_Position = projection * view  * vec4(v_FragPos, 1.0);
}