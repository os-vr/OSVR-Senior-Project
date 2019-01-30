#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec4 tangent;
layout (location = 3) in vec2 texCoord;
layout (location = 4) in vec4 poseIndices;
layout (location = 5) in vec4 poseWeights;
layout (location = 6) in vec4 color;
out vec3 vertexWorldPos;
out vec3 vertexViewDir;
out vec3 vertexObjPos;
out vec3 vertexNormal;
out vec3 vertexTangent;
out vec3 vertexBitangent;
out vec2 vertexUV;
out vec4 vertexColor;
uniform vec3 viewPos;
uniform mat4 world;
uniform mat4 viewProj;
uniform mat4 meshPose[64];
void main() {
    vec4 vertexPose;
    vertexPose = meshPose[int(poseIndices[0])] * vec4(position, 1.0) * poseWeights[0];
    vertexPose += meshPose[int(poseIndices[1])] * vec4(position, 1.0) * poseWeights[1];
    vertexPose += meshPose[int(poseIndices[2])] * vec4(position, 1.0) * poseWeights[2];
    vertexPose += meshPose[int(poseIndices[3])] * vec4(position, 1.0) * poseWeights[3];
    
    vec4 normalPose;
    normalPose = meshPose[int(poseIndices[0])] * vec4(normal, 0.0) * poseWeights[0];
    normalPose += meshPose[int(poseIndices[1])] * vec4(normal, 0.0) * poseWeights[1];
    normalPose += meshPose[int(poseIndices[2])] * vec4(normal, 0.0) * poseWeights[2];
    normalPose += meshPose[int(poseIndices[3])] * vec4(normal, 0.0) * poseWeights[3];
    normalPose = normalize(normalPose);

	vec4 tangentPose;
    tangentPose = meshPose[int(poseIndices[0])] * vec4(tangent.xyz, 0.0) * poseWeights[0];
    tangentPose += meshPose[int(poseIndices[1])] * vec4(tangent.xyz, 0.0) * poseWeights[1];
    tangentPose += meshPose[int(poseIndices[2])] * vec4(tangent.xyz, 0.0) * poseWeights[2];
    tangentPose += meshPose[int(poseIndices[3])] * vec4(tangent.xyz, 0.0) * poseWeights[3];
	tangentPose = normalize(tangentPose);

	vertexWorldPos = vec3(world * vertexPose);
	gl_Position = viewProj * vec4(vertexWorldPos, 1.0);
	vertexViewDir = normalize(viewPos - vertexWorldPos.xyz);
    vertexObjPos = position.xyz;
	vertexNormal = (world * normalPose).xyz;
	vertexTangent = (world * tangentPose).xyz;
	vertexBitangent = normalize(cross(vertexNormal, vertexTangent) * tangent.w);
	vertexUV = texCoord;
	vertexColor = color.rgba;
}