#version 330

in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_LightPos;
in vec3 v_FragPos;

uniform sampler2D s_texture;
uniform vec3 v_diffuse;	// OBJ NEW

out vec4 Color;
 
void main()
{
	vec4 lightColor = vec4(1,1,1,1);
	vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 0.0);
	vec4 lightSpec = vec4(0.1,0.1,0.1,0.0);
	float fSpecularPower = 10.0;

	vec3 norm = normalize(v_Normal);
	vec3 lightDir = normalize(v_LightPos - v_FragPos);
	float diff = max(dot(norm,lightDir),0.0);
	vec4 fvBaseColour = texture2D(s_texture, v_TexCoord);
	vec3 diffuse = diff * fvBaseColour.xyz;
	Color = vec4(diffuse,1.0);
}