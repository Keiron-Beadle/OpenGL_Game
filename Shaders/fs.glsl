#version 330
 
in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_LightDir;
in vec3 v_ViewDir;
uniform sampler2D s_texture;
uniform vec3 v_diffuse;	// OBJ NEW

out vec4 Color;
 
void main()
{
	vec4 lightColor = vec4(1,1,1,1);
	vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 1.0);
	vec4 lightSpec = vec4(0.04,0.04,0.04,1.0);
	float fSpecularPower = 0.1;

	vec3 fvLightDirection = normalize(v_LightDir);
	vec3 fvNormal = normalize(v_Normal);
	float fNDotL = dot(fvNormal, fvLightDirection);

	vec3 fvReflection = normalize(((2.0*fvNormal)*fNDotL)-fvLightDirection);
	vec3 fvViewDirection = normalize(v_ViewDir);
	float fRDotV = max(0.0, dot(fvReflection, fvViewDirection.xyz));

	vec4 fvBaseColour = texture2D(s_texture, v_TexCoord);
	vec4 totalAmb = lightAmbient * fvBaseColour;
	vec4 totalDiffuse = lightColor * fNDotL * fvBaseColour;
	vec4 totalSpec = lightSpec * (pow(fRDotV, fSpecularPower));
	totalDiffuse.w = 1;
	Color = totalDiffuse;
	//Color = (totalAmb + vec4(totalDiffuse,0) + totalSpec);
    //Color = lightAmbient + (vec4(v_diffuse, 1) * texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0));  // OBJ CHANGED
}