#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec3 a_Normal;

uniform mat4 ModelViewProjMat;
uniform mat4 ModelViewMat;
uniform mat4 ModelMat;
uniform vec3 EyePosition;

out vec2 v_TexCoord;
out vec3 v_Normal;
out vec3 v_LightDir;
out vec3 v_ViewDir;

void main()
{
	gl_Position = ModelViewProjMat * vec4(a_Position, 1.0);
	vec4 lightPos = ModelMat * vec4(0,10,10,1.0);  //Need Lights and Object in same space, I chose World
												//as objects vertices will be rotated if so.
	vec4 fvObjectPosition = ModelMat * vec4(a_Position,1.0);
	v_ViewDir = -fvObjectPosition.xyz;
	v_LightDir = lightPos.xyz - fvObjectPosition.xyz;
	v_TexCoord = a_TexCoord;
	v_Normal = a_Normal;
}