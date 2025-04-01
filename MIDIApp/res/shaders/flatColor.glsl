//type vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;

out vec4 vertexColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(position, 1.0) * model * view * projection;
	vertexColor = color;
}

//type fragment
#version 330 core

in vec4 vertexColor;
out vec4 fragColor;

void main()
{
	fragColor = vertexColor;
}