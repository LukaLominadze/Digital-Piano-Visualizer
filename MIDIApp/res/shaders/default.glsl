//type vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 texCoord;
layout (location = 3) in float texSlot;

out vec4 v_VertexColor;
out vec2 v_TexCoord;
out float v_TexSlot;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(position, 1.0) * model * view * projection;
	v_VertexColor = color;
	v_TexCoord = texCoord;
	v_TexSlot = texSlot;
}

//type fragment
#version 330 core

in vec4 v_VertexColor;
in vec2 v_TexCoord;
in float v_TexSlot;
out vec4 fragColor;

uniform sampler2D u_Textures[16];

void main()
{
	fragColor = texture(u_Textures[int(v_TexSlot)], v_TexCoord) * v_VertexColor;
}