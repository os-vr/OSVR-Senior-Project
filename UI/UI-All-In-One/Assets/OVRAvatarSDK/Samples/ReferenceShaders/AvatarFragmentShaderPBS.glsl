#version 330 core

in vec2 vertexUV;
out vec4 fragmentColor;

uniform sampler2D albedo;
uniform sampler2D surface;

void main() {
	vec4 color = texture2D(albedo, vertexUV);
	fragmentColor = color;
}