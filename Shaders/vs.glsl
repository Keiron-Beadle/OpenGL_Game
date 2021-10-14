﻿#version 330

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
out vec3 v_LightPos;
out vec3 v_FragPos;

void main()
{
	gl_Position = ModelViewProjMat * vec4(a_Position, 1.0);
	v_FragPos = vec3(ViewMat * vec4(a_Position,1.0));
	v_LightPos = vec3((ViewMat * vec4(0.0,0.0,50.0,0.0)));
	v_TexCoord = a_TexCoord;
	v_Normal = normalize(vec3( transpose(inverse(ModelViewMat)) * vec4(a_Normal,1.0) ));
}