#version 330

struct PointLight
{
	vec3 position;
	float constant;
	float linear;
	float quadratic;
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

};

in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;


#define NUMBEROFLIGHTS 1
uniform PointLight pointLights[NUMBEROFLIGHTS];
uniform vec3 viewPos;
uniform sampler2D s_texture;

out vec4 Color;
 
vec3 calcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
	vec3 normal = normalize(v_Normal);
	vec3 viewDir = normalize(viewPos - v_FragPos);

	vec3 resultingFrag = vec3(0,0,0);
	for (int i = 0; i < NUMBEROFLIGHTS; i++){
		resultingFrag += calcPointLight(pointLights[i], normal, v_FragPos, viewDir);
	}
	
	Color = vec4(resultingFrag,1.0);
}

vec3 calcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 lightDir = normalize(light.position - fragPos);
	float diff = max(dot(normal,lightDir),0.0);
	vec3 reflectDir = reflect(-lightDir,normal);
	float spec = max(dot(viewDir,reflectDir),0.0); //Material stuff goes here if you add materials in future
	float dist = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * (dist*dist));

	vec3 ambient = light.ambient * vec3(texture(s_texture,v_TexCoord));
	vec3 diffuse = light.diffuse * diff * vec3(texture(s_texture, v_TexCoord));
	vec3 specular = light.specular * spec * vec3(texture(s_texture, v_TexCoord));

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	return (ambient + diffuse + specular);
}