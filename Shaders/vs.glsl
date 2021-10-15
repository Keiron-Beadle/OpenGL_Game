#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec3 a_Normal;

uniform mat4 ModelViewProjMat;
uniform mat4 ModelViewMat;
uniform mat4 ViewMat;
uniform mat4 InvModelView;
uniform vec3 EyePosition;

out vec2 v_TexCoord;
out vec3 v_Normal;
out vec3 v_LightPos[4];
out vec3 v_FragPos;

void main()
{
	vec4 lightPositions[4];
	lightPositions[0] = vec4(0.0,10.0,50.0,0.0);
	lightPositions[1] = vec4(0.0,10.0,-50.0,0.0);
	lightPositions[2] = vec4(50.0,10.0,0.0,0.0);
	lightPositions[3] = vec4(-50.0,10.0,0.0,0.0);

	gl_Position = ModelViewProjMat * vec4(a_Position, 1.0);
	v_FragPos = vec3(ViewMat * vec4(a_Position,1.0));
	for (int i = 0; i < 4; i++)
		v_LightPos[i] = vec3((ViewMat * lightPositions[i]));
	v_TexCoord = a_TexCoord;
	v_Normal = normalize(vec3( transpose(inverse(ModelViewMat)) * vec4(a_Normal,1.0) ));
}