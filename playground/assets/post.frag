#version 330 core

in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform sampler2D depthTexture;
uniform vec2 screenSize;

out vec4 FragColor;

bool equals(float x, float y)
{
	return abs(x-y) < 0.01;
}

void main()
{
	vec2 pos = TexCoords * screenSize;

	int scale = 2;
	pos = scale * floor(pos / scale);

	float depth = pow(texture(depthTexture, pos / screenSize).r, 100);
	vec3 color = texture(screenTexture, pos / screenSize).rgb;

	color = mix(color, vec3(0.5, 0.8, 1), depth);

	FragColor = vec4(color, 1);
}