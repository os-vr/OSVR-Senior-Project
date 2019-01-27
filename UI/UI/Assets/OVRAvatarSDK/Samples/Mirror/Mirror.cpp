/************************************************************************************
* Mirror.cpp
*
* Sample app showing basic usage of the avatar SDK
************************************************************************************/

#include <OVR_Avatar.h>

#include <GL/glew.h>

#include <SDL.h>
#include <SDL_opengl.h>

#include <OVR_CAPI.h>
#include <OVR_CAPI_GL.h>
#include <OVR_Platform.h>

#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtx/transform.hpp>
#include <glm/gtx/matrix_decompose.hpp>

#include <malloc.h>
#include <stdlib.h>
#include <stdio.h>
#include <string>
#include <map>
#include <chrono>


/************************************************************************************
* Constants
************************************************************************************/

#define MIRROR_SAMPLE_APP_ID "958062084316416"
#define MIRROR_WINDOW_WIDTH 800
#define MIRROR_WINDOW_HEIGHT 600

// Disable MIRROR_ALLOW_OVR to force 2D rendering
#define MIRROR_ALLOW_OVR true


/************************************************************************************
* Static state
************************************************************************************/

static GLuint _skinnedMeshProgram;
static GLuint _combinedMeshProgram;
static GLuint _skinnedMeshPBSProgram;
static GLuint _debugLineProgram;
static GLuint _debugVertexArray;
static GLuint _debugVertexBuffer;
static ovrAvatar* _avatar;
static bool _combineMeshes = true;
static ovrAvatarAssetID _avatarCombinedMeshAlpha = 0;
static ovrAvatarVector4f _avatarCombinedMeshAlphaOffset;
static size_t _loadingAssets;
static bool _waitingOnCombinedMesh = false;

static float _elapsedSeconds;	
static std::map<ovrAvatarAssetID, void*> _assetMap;


/************************************************************************************
* GL helpers
************************************************************************************/

enum {
	VERTEX,
	FRAGMENT,
	SHADER_COUNT
};

static GLuint _compileProgramFromSource(const char vertexShaderSource[], const char fragmentShaderSource[], size_t errorBufferSize, char* errorBuffer) {
	const GLenum types[SHADER_COUNT] = { GL_VERTEX_SHADER, GL_FRAGMENT_SHADER };
	const char* sources[SHADER_COUNT] = { vertexShaderSource, fragmentShaderSource };
	GLint shaders[SHADER_COUNT] { 0, 0 };
	bool success = true;

	// Compile all of the shader program objects
	for (int i = 0; i < SHADER_COUNT; ++i) {
		shaders[i] = glCreateShader(types[i]);
		glShaderSource(shaders[i], 1, &sources[i], NULL);
		glCompileShader(shaders[i]);

		GLint compileSuccess;
		glGetShaderiv(shaders[i], GL_COMPILE_STATUS, &compileSuccess);
		if (!compileSuccess) {
			glGetShaderInfoLog(shaders[i], (GLsizei)errorBufferSize, NULL, errorBuffer);
			success = false;
			break;
		}
	}

	// Create and link the program
	GLuint program = 0;
	if (success) {
		program = glCreateProgram();
		for (int i = 0; i < SHADER_COUNT; ++i) {
			glAttachShader(program, shaders[i]);
		}
		glLinkProgram(program);

		GLint linkSuccess;
		glGetProgramiv(program, GL_LINK_STATUS, &linkSuccess);
		if (!linkSuccess) {
			glGetProgramInfoLog(program, (GLsizei)errorBufferSize, NULL, errorBuffer);
			glDeleteProgram(program);
			program = 0;
		}
	}
	for (int i = 0; i < SHADER_COUNT; ++i) {
		if (shaders[i]) {
			glDeleteShader(shaders[i]);
		}
	}
	return program;
}

static GLuint _compileProgramFromFiles(const char vertexShaderPath[], const char fragmentShaderPath[], size_t errorBufferSize, char* errorBuffer) {
	const char* fileSources[SHADER_COUNT] = { vertexShaderPath, fragmentShaderPath };
	char* fileBuffers[SHADER_COUNT] = { NULL, NULL };
	bool success = true;

	// Load each of the shader files
	for (int i = 0; i < SHADER_COUNT; ++i) {
		std::string fullPath = SDL_GetBasePath();
		fullPath += fileSources[i];
		FILE* file = fopen(fullPath.c_str(), "rb");
		if (!file) {
			strncpy(errorBuffer, "Failed to open shader files.", errorBufferSize);
			success = false;
			break;
		}
		fseek(file, 0, SEEK_END);
		long offset = ftell(file);
		fseek(file, 0, SEEK_SET);
		fileBuffers[i] = (char*)malloc(offset + 1);
		fread(fileBuffers[i], 1, offset, file);
		fileBuffers[i][offset] = '\0';
	}

	// Compile the program
	GLuint program = 0;
	if (success) {
		program = _compileProgramFromSource(fileBuffers[VERTEX], fileBuffers[FRAGMENT], errorBufferSize, errorBuffer);
	}

	// Clean up the loaded data
	for (int i = 0; i < SHADER_COUNT; ++i) {
		if (fileBuffers[i]) {
			free(fileBuffers[i]);
		}
	}
	return program;
}


/************************************************************************************
* Math helpers and type conversions
************************************************************************************/

static glm::vec3 _glmFromOvrVector(const ovrVector3f& ovrVector)
{
    return glm::vec3(ovrVector.x, ovrVector.y, ovrVector.z);
}

static glm::quat _glmFromOvrQuat(const ovrQuatf& ovrQuat)
{
    return glm::quat(ovrQuat.w, ovrQuat.x, ovrQuat.y, ovrQuat.z);
}

static void _glmFromOvrAvatarTransform(const ovrAvatarTransform& transform, glm::mat4* target) {
	glm::vec3 position(transform.position.x, transform.position.y, transform.position.z);
	glm::quat orientation(transform.orientation.w, transform.orientation.x, transform.orientation.y, transform.orientation.z);
	glm::vec3 scale(transform.scale.x, transform.scale.y, transform.scale.z);
	*target = glm::translate(position) * glm::mat4_cast(orientation) * glm::scale(scale);
}

static void _ovrAvatarTransformFromGlm(const glm::vec3& position, const glm::quat& orientation, const glm::vec3& scale, ovrAvatarTransform* target) {
	target->position.x = position.x;
	target->position.y = position.y;
	target->position.z = position.z;
	target->orientation.x = orientation.x;
	target->orientation.y = orientation.y;
	target->orientation.z = orientation.z;
	target->orientation.w = orientation.w;
	target->scale.x = scale.x;
	target->scale.y = scale.y;
	target->scale.z = scale.z;
}

static void _ovrAvatarTransformFromGlm(const glm::mat4& matrix, ovrAvatarTransform* target) {
	glm::vec3 scale;
	glm::quat orientation;
	glm::vec3 translation;
	glm::vec3 skew;
	glm::vec4 perspective;
	glm::decompose(matrix, scale, orientation, translation, skew, perspective);
	_ovrAvatarTransformFromGlm(translation, orientation, scale, target);
}

static void _ovrAvatarHandInputStateFromOvr(const ovrAvatarTransform& transform, const ovrInputState& inputState, ovrHandType hand, ovrAvatarHandInputState* state)
{
	state->transform = transform;
	state->buttonMask = 0;
	state->touchMask = 0;
	state->joystickX = inputState.Thumbstick[hand].x;
	state->joystickY = inputState.Thumbstick[hand].y;
	state->indexTrigger = inputState.IndexTrigger[hand];
	state->handTrigger = inputState.HandTrigger[hand];
	state->isActive = false;
	if (hand == ovrHand_Left)
	{
		if (inputState.Buttons & ovrButton_X) state->buttonMask |= ovrAvatarButton_One;
		if (inputState.Buttons & ovrButton_Y) state->buttonMask |= ovrAvatarButton_Two;
		if (inputState.Buttons & ovrButton_Enter) state->buttonMask |= ovrAvatarButton_Three;
		if (inputState.Buttons & ovrButton_LThumb) state->buttonMask |= ovrAvatarButton_Joystick;
		if (inputState.Touches & ovrTouch_X) state->touchMask |= ovrAvatarTouch_One;
		if (inputState.Touches & ovrTouch_Y) state->touchMask |= ovrAvatarTouch_Two;
		if (inputState.Touches & ovrTouch_LThumb) state->touchMask |= ovrAvatarTouch_Joystick;
		if (inputState.Touches & ovrTouch_LThumbRest) state->touchMask |= ovrAvatarTouch_ThumbRest;
		if (inputState.Touches & ovrTouch_LIndexTrigger) state->touchMask |= ovrAvatarTouch_Index;
		if (inputState.Touches & ovrTouch_LIndexPointing) state->touchMask |= ovrAvatarTouch_Pointing;
		if (inputState.Touches & ovrTouch_LThumbUp) state->touchMask |= ovrAvatarTouch_ThumbUp;
		state->isActive = (inputState.ControllerType & ovrControllerType_LTouch) != 0;
	}
	else if (hand == ovrHand_Right)
	{
		if (inputState.Buttons & ovrButton_A) state->buttonMask |= ovrAvatarButton_One;
		if (inputState.Buttons & ovrButton_B) state->buttonMask |= ovrAvatarButton_Two;
		if (inputState.Buttons & ovrButton_Home) state->buttonMask |= ovrAvatarButton_Three;
		if (inputState.Buttons & ovrButton_RThumb) state->buttonMask |= ovrAvatarButton_Joystick;
		if (inputState.Touches & ovrTouch_A) state->touchMask |= ovrAvatarTouch_One;
		if (inputState.Touches & ovrTouch_B) state->touchMask |= ovrAvatarTouch_Two;
		if (inputState.Touches & ovrTouch_RThumb) state->touchMask |= ovrAvatarTouch_Joystick;
		if (inputState.Touches & ovrTouch_RThumbRest) state->touchMask |= ovrAvatarTouch_ThumbRest;
		if (inputState.Touches & ovrTouch_RIndexTrigger) state->touchMask |= ovrAvatarTouch_Index;
		if (inputState.Touches & ovrTouch_RIndexPointing) state->touchMask |= ovrAvatarTouch_Pointing;
		if (inputState.Touches & ovrTouch_RThumbUp) state->touchMask |= ovrAvatarTouch_ThumbUp;
		state->isActive = (inputState.ControllerType & ovrControllerType_RTouch) != 0;
	}
}

static void _computeWorldPose(const ovrAvatarSkinnedMeshPose& localPose, glm::mat4* worldPose)
{
	for (uint32_t i = 0; i < localPose.jointCount; ++i)
	{
		glm::mat4 local;
		_glmFromOvrAvatarTransform(localPose.jointTransform[i], &local);

		int parentIndex = localPose.jointParents[i];
		if (parentIndex < 0)
		{
			worldPose[i] = local;
		}
		else
		{
			worldPose[i] = worldPose[parentIndex] * local;
		}
	}
}

static glm::mat4 _computeReflectionMatrix(const glm::vec4& plane)
{
	return glm::mat4(
		1.0f - 2.0f * plane.x * plane.x,
		-2.0f * plane.x * plane.y,
		-2.0f * plane.x * plane.z,
		-2.0f * plane.w * plane.x,

		-2.0f * plane.y * plane.x,
		1.0f - 2.0f * plane.y * plane.y,
		-2.0f * plane.y * plane.z,
		-2.0f * plane.w * plane.y,

		-2.0f * plane.z * plane.x,
		-2.0f * plane.z * plane.y,
		1.0f - 2.0f * plane.z * plane.z,
		-2.0f * plane.w * plane.z,

		0.0f,
		0.0f,
		0.0f,
		1.0f
	);
}


/************************************************************************************
* Wrappers for GL representations of avatar assets
************************************************************************************/

struct MeshData {
	GLuint vertexArray;
	GLuint vertexBuffer;
	GLuint elementBuffer;
	GLuint elementCount;
	glm::mat4 bindPose[OVR_AVATAR_MAXIMUM_JOINT_COUNT];
	glm::mat4 inverseBindPose[OVR_AVATAR_MAXIMUM_JOINT_COUNT];
};

struct TextureData {
	GLuint textureID;
};

static MeshData* _loadCombinedMesh(const ovrAvatarMeshAssetDataV2* data)
{
	_waitingOnCombinedMesh = false;

	MeshData* mesh = new MeshData();

	// Create the vertex array and buffer
	glGenVertexArrays(1, &mesh->vertexArray);
	glGenBuffers(1, &mesh->vertexBuffer);
	glGenBuffers(1, &mesh->elementBuffer);

	// Bind the vertex buffer and assign the vertex data	
	glBindVertexArray(mesh->vertexArray);
	glBindBuffer(GL_ARRAY_BUFFER, mesh->vertexBuffer);
	glBufferData(GL_ARRAY_BUFFER, data->vertexCount * sizeof(ovrAvatarMeshVertexV2), data->vertexBuffer, GL_STATIC_DRAW);

	// Bind the index buffer and assign the index data
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh->elementBuffer);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, data->indexCount * sizeof(GLushort), data->indexBuffer, GL_STATIC_DRAW);
	mesh->elementCount = data->indexCount;

	// Fill in the array attributes
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->x);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->nx);
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->tx);
	glEnableVertexAttribArray(2);
	glVertexAttribPointer(3, 2, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->u);
	glEnableVertexAttribArray(3);
	glVertexAttribPointer(4, 4, GL_BYTE, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->blendIndices);
	glEnableVertexAttribArray(4);
	glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->blendWeights);
	glEnableVertexAttribArray(5);
	glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertexV2), &((ovrAvatarMeshVertexV2*)0)->r);
	glEnableVertexAttribArray(6);

	// Clean up
	glBindVertexArray(0);

	// Translate the bind pose
	_computeWorldPose(data->skinnedBindPose, mesh->bindPose);
	for (uint32_t i = 0; i < data->skinnedBindPose.jointCount; ++i)
	{
		mesh->inverseBindPose[i] = glm::inverse(mesh->bindPose[i]);
	}
	return mesh;
}

static MeshData* _loadMesh(const ovrAvatarMeshAssetData* data)
{
	MeshData* mesh = new MeshData();

	// Create the vertex array and buffer
	glGenVertexArrays(1, &mesh->vertexArray);
	glGenBuffers(1, &mesh->vertexBuffer);
	glGenBuffers(1, &mesh->elementBuffer);

	// Bind the vertex buffer and assign the vertex data	
	glBindVertexArray(mesh->vertexArray);
	glBindBuffer(GL_ARRAY_BUFFER, mesh->vertexBuffer);
	glBufferData(GL_ARRAY_BUFFER, data->vertexCount * sizeof(ovrAvatarMeshVertex), data->vertexBuffer, GL_STATIC_DRAW);

	// Bind the index buffer and assign the index data
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh->elementBuffer);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, data->indexCount * sizeof(GLushort), data->indexBuffer, GL_STATIC_DRAW);
	mesh->elementCount = data->indexCount;

	// Fill in the array attributes
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->x);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->nx);
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->tx);
	glEnableVertexAttribArray(2);
	glVertexAttribPointer(3, 2, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->u);
	glEnableVertexAttribArray(3);
	glVertexAttribPointer(4, 4, GL_BYTE, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->blendIndices);
	glEnableVertexAttribArray(4);
	glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, sizeof(ovrAvatarMeshVertex), &((ovrAvatarMeshVertex*)0)->blendWeights);
	glEnableVertexAttribArray(5);

	// Clean up
	glBindVertexArray(0);

	// Translate the bind pose
	_computeWorldPose(data->skinnedBindPose, mesh->bindPose);
	for (uint32_t i = 0; i < data->skinnedBindPose.jointCount; ++i)
	{
		mesh->inverseBindPose[i] = glm::inverse(mesh->bindPose[i]);
	}
	return mesh;
}

static TextureData* _loadTexture(const ovrAvatarTextureAssetData* data)
{
	// Create a texture
	TextureData* texture = new TextureData();
	glGenTextures(1, &texture->textureID);
	glBindTexture(GL_TEXTURE_2D, texture->textureID);

	// Load the image data
	switch (data->format)
	{
		// Handle uncompressed image data
		case ovrAvatarTextureFormat_RGB24:
			for (uint32_t level = 0, offset = 0, width = data->sizeX, height = data->sizeY; level < data->mipCount; ++level)
			{
				glTexImage2D(GL_TEXTURE_2D, level, GL_RGB, width, height, 0, GL_BGR, GL_UNSIGNED_BYTE, data->textureData + offset);
				offset += width * height * 3;
				width /= 2;
				height /= 2;
			}
			break;

		// Handle compressed image data
		case ovrAvatarTextureFormat_DXT1:
		case ovrAvatarTextureFormat_DXT5:
		{
			GLenum glFormat;
			int blockSize;
			if (data->format == ovrAvatarTextureFormat_DXT1)
			{
				blockSize = 8;
				glFormat = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT;
			}
			else
			{
				blockSize = 16;
				glFormat = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT;
			}

			for (uint32_t level = 0, offset = 0, width = data->sizeX, height = data->sizeY; level < data->mipCount; ++level)
			{
				GLsizei levelSize = (width < 4 || height < 4) ? blockSize : blockSize * (width / 4) * (height / 4);
				glCompressedTexImage2D(GL_TEXTURE_2D, level, glFormat, width, height, 0, levelSize, data->textureData + offset);
				offset += levelSize;
				width /= 2;
				height /= 2;
			}
			break;
		}

		// Handle ASTC data
		case ovrAvatarTextureFormat_ASTC_RGB_6x6_MIPMAPS:
		{
            const unsigned char * level = (const unsigned char*)data->textureData;

            unsigned int w = data->sizeX;
            unsigned int h = data->sizeY;
            for ( unsigned int i = 0; i < data->mipCount; i++ )
            {
                int32_t blocksWide = ( w + 5 ) / 6;
                int32_t blocksHigh = ( h + 5 ) / 6;
                int32_t mipSize = 16 * blocksWide * blocksHigh;

                glCompressedTexImage2D( GL_TEXTURE_2D, i, GL_COMPRESSED_RGBA_ASTC_6x6_KHR, w, h, 0, mipSize, level );

                level += mipSize;

                w >>= 1;
                h >>= 1;
                if ( w < 1 ) { w = 1; }
                if ( h < 1 ) { h = 1; }
            }
			break;
		}

	}
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	return texture;
}


/************************************************************************************
* Rendering functions
************************************************************************************/

static void _setTextureSampler(GLuint program, int textureUnit, const char uniformName[], ovrAvatarAssetID assetID)
{
	GLuint textureID = 0;
	if (assetID)
	{
		void* data = _assetMap[assetID];
		TextureData* textureData = (TextureData*)data;
		textureID = textureData->textureID;
	}
	glActiveTexture(GL_TEXTURE0 + textureUnit);
	glBindTexture(GL_TEXTURE_2D, textureID);
	glUniform1i(glGetUniformLocation(program, uniformName), textureUnit);
}

static void _setTextureSamplers(GLuint program, const char uniformName[], size_t count, const int textureUnits[], const ovrAvatarAssetID assetIDs[])
{
	for (int i = 0; i < count; ++i)
	{
		ovrAvatarAssetID assetID = assetIDs[i];

		GLuint textureID = 0;
		if (assetID)
		{
			void* data = _assetMap[assetID];
			if (data)
			{
				TextureData* textureData = (TextureData*)data;
				textureID = textureData->textureID;
			}
		}
		glActiveTexture(GL_TEXTURE0 + textureUnits[i]);
		glBindTexture(GL_TEXTURE_2D, textureID);
	}
	GLint uniformLocation = glGetUniformLocation(program, uniformName);
	glUniform1iv(uniformLocation, (GLsizei)count, textureUnits);
}

static void _setMeshState(
	GLuint program,
	const ovrAvatarTransform& localTransform,
	const MeshData* data,
	const ovrAvatarSkinnedMeshPose& skinnedPose,
	const glm::mat4& world,
	const glm::mat4& view,
	const glm::mat4 proj,
	const glm::vec3& viewPos
) {
	// Compute the final world and viewProjection matrices for this part
	glm::mat4 local;
	_glmFromOvrAvatarTransform(localTransform, &local);
	glm::mat4 worldMat = world * local;
	glm::mat4 viewProjMat = proj * view;

	// Compute the skinned pose
	glm::mat4* skinnedPoses = (glm::mat4*)alloca(sizeof(glm::mat4) * skinnedPose.jointCount);
	_computeWorldPose(skinnedPose, skinnedPoses);
	for (uint32_t i = 0; i < skinnedPose.jointCount; ++i)
	{
		skinnedPoses[i] = skinnedPoses[i] * data->inverseBindPose[i];
	}

	// Pass the world view position to the shader for view-dependent rendering
	glUniform3fv(glGetUniformLocation(program, "viewPos"), 1, glm::value_ptr(viewPos));

	// Assign the vertex uniforms
	glUniformMatrix4fv(glGetUniformLocation(program, "world"), 1, 0, glm::value_ptr(worldMat));
	glUniformMatrix4fv(glGetUniformLocation(program, "viewProj"), 1, 0, glm::value_ptr(viewProjMat));
	glUniformMatrix4fv(glGetUniformLocation(program, "meshPose"), (GLsizei)skinnedPose.jointCount, 0, glm::value_ptr(*skinnedPoses));
}

static void _setMaterialState(GLuint program, const ovrAvatarMaterialState* state, glm::mat4* projectorInv)
{
	// Assign the fragment uniforms
	glUniform1i(glGetUniformLocation(program, "useAlpha"), state->alphaMaskTextureID != 0);
	glUniform1i(glGetUniformLocation(program, "useNormalMap"), state->normalMapTextureID != 0);
	glUniform1i(glGetUniformLocation(program, "useRoughnessMap"), state->roughnessMapTextureID != 0);

	glUniform1f(glGetUniformLocation(program, "elapsedSeconds"), _elapsedSeconds);

	if (projectorInv)
	{
		glUniform1i(glGetUniformLocation(program, "useProjector"), 1);
		glUniformMatrix4fv(glGetUniformLocation(program, "projectorInv"), 1, 0, glm::value_ptr(*projectorInv));
	}
	else
	{
		glUniform1i(glGetUniformLocation(program, "useProjector"), 0);
	}

	int textureSlot = 1;
	glUniform4fv(glGetUniformLocation(program, "baseColor"), 1, &state->baseColor.x);
	glUniform1i(glGetUniformLocation(program, "baseMaskType"), state->baseMaskType);
	glUniform4fv(glGetUniformLocation(program, "baseMaskParameters"), 1, &state->baseMaskParameters.x);
	glUniform4fv(glGetUniformLocation(program, "baseMaskAxis"), 1, &state->baseMaskAxis.x);
	_setTextureSampler(program, textureSlot++, "alphaMask", state->alphaMaskTextureID);
	glUniform4fv(glGetUniformLocation(program, "alphaMaskScaleOffset"), 1, &state->alphaMaskScaleOffset.x);
	_setTextureSampler(program, textureSlot++, "clothingAlpha", _avatarCombinedMeshAlpha);
	glUniform4fv(glGetUniformLocation(program, "clothingAlphaScaleOffset"), 1, &_avatarCombinedMeshAlphaOffset.x);
	_setTextureSampler(program, textureSlot++, "normalMap", state->normalMapTextureID);
	glUniform4fv(glGetUniformLocation(program, "normalMapScaleOffset"), 1, &state->normalMapScaleOffset.x);
	_setTextureSampler(program, textureSlot++, "parallaxMap", state->parallaxMapTextureID);
	glUniform4fv(glGetUniformLocation(program, "parallaxMapScaleOffset"), 1, &state->parallaxMapScaleOffset.x);
	_setTextureSampler(program, textureSlot++, "roughnessMap", state->roughnessMapTextureID);
	glUniform4fv(glGetUniformLocation(program, "roughnessMapScaleOffset"), 1, &state->roughnessMapScaleOffset.x);

	struct LayerUniforms {
		int layerSamplerModes[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		int layerBlendModes[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		int layerMaskTypes[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarVector4f layerColors[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		int layerSurfaces[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarAssetID layerSurfaceIDs[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarVector4f layerSurfaceScaleOffsets[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarVector4f layerSampleParameters[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarVector4f layerMaskParameters[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
		ovrAvatarVector4f layerMaskAxes[OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT];
	} layerUniforms;
	memset(&layerUniforms, 0, sizeof(layerUniforms));
	for (uint32_t i = 0; i < state->layerCount; ++i)
	{
		const ovrAvatarMaterialLayerState& layerState = state->layers[i];
		layerUniforms.layerSamplerModes[i] = layerState.sampleMode;
		layerUniforms.layerBlendModes[i] = layerState.blendMode;
		layerUniforms.layerMaskTypes[i] = layerState.maskType;
		layerUniforms.layerColors[i] = layerState.layerColor;
		layerUniforms.layerSurfaces[i] = textureSlot++;
		layerUniforms.layerSurfaceIDs[i] = layerState.sampleTexture;
		layerUniforms.layerSurfaceScaleOffsets[i] = layerState.sampleScaleOffset;
		layerUniforms.layerSampleParameters[i] = layerState.sampleParameters;
		layerUniforms.layerMaskParameters[i] = layerState.maskParameters;
		layerUniforms.layerMaskAxes[i] = layerState.maskAxis;
	}

	glUniform1i(glGetUniformLocation(program, "layerCount"), state->layerCount);
	glUniform1iv(glGetUniformLocation(program, "layerSamplerModes"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, layerUniforms.layerSamplerModes);
	glUniform1iv(glGetUniformLocation(program, "layerBlendModes"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, layerUniforms.layerBlendModes);
	glUniform1iv(glGetUniformLocation(program, "layerMaskTypes"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, layerUniforms.layerMaskTypes);
	glUniform4fv(glGetUniformLocation(program, "layerColors"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, (float*)layerUniforms.layerColors);
	_setTextureSamplers(program, "layerSurfaces", OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, layerUniforms.layerSurfaces, layerUniforms.layerSurfaceIDs);
	glUniform4fv(glGetUniformLocation(program, "layerSurfaceScaleOffsets"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, (float*)layerUniforms.layerSurfaceScaleOffsets);
	glUniform4fv(glGetUniformLocation(program, "layerSampleParameters"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, (float*)layerUniforms.layerSampleParameters);
	glUniform4fv(glGetUniformLocation(program, "layerMaskParameters"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, (float*)layerUniforms.layerMaskParameters);
	glUniform4fv(glGetUniformLocation(program, "layerMaskAxes"), OVR_AVATAR_MAX_MATERIAL_LAYER_COUNT, (float*)layerUniforms.layerMaskAxes);

}

static void _setPBSState(GLuint program, const ovrAvatarAssetID albedoTextureID, const ovrAvatarAssetID surfaceTextureID)
{
	int textureSlot = 0;
	_setTextureSampler(program, textureSlot++, "albedo", albedoTextureID);
	_setTextureSampler(program, textureSlot++, "surface", surfaceTextureID);
}

static void _renderDebugLine(const glm::mat4& worldViewProj, const glm::vec3& a, const glm::vec3& b, const glm::vec4& aColor, const glm::vec4& bColor)
{
	glUseProgram(_debugLineProgram);
	glUniformMatrix4fv(glGetUniformLocation(_debugLineProgram, "worldViewProj"), 1, 0, glm::value_ptr(worldViewProj));

	struct {
		glm::vec3 p;
		glm::vec4 c;
	} vertices[2] = {
		{ a, aColor },
		{ b, bColor },
	};

	glBindVertexArray(_debugVertexArray);
	glBindBuffer(GL_ARRAY_BUFFER, _debugVertexArray);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_DYNAMIC_DRAW);

	// Fill in the array attributes
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(vertices[0]), 0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 4, GL_FLOAT, GL_FALSE, sizeof(vertices[0]), (void*)sizeof(glm::vec3));
	glEnableVertexAttribArray(1);

	glDrawArrays(GL_LINE_STRIP, 0, 2);
}

static void _renderPose(const glm::mat4& worldViewProj, const ovrAvatarSkinnedMeshPose& pose)
{
	glm::mat4* skinnedPoses = (glm::mat4*)alloca(sizeof(glm::mat4) * pose.jointCount);
	_computeWorldPose(pose, skinnedPoses);
	for (uint32_t i = 1; i < pose.jointCount; ++i)
	{
		int parent = pose.jointParents[i];
		_renderDebugLine(worldViewProj, glm::vec3(skinnedPoses[parent][3]), glm::vec3(skinnedPoses[i][3]), glm::vec4(1, 1, 1, 1), glm::vec4(1, 0, 0, 1));
	}
}

static void _renderSkinnedMeshPart(GLuint shader, const ovrAvatarRenderPart_SkinnedMeshRender* mesh, uint32_t visibilityMask, const glm::mat4& world, const glm::mat4& view, const glm::mat4 proj, const glm::vec3& viewPos, bool renderJoints)
{
	// If this part isn't visible from the viewpoint we're rendering from, do nothing
	if ((mesh->visibilityMask & visibilityMask) == 0)
	{
		return;
	}

	// Get the GL mesh data for this mesh's asset
	MeshData* data = (MeshData*)_assetMap[mesh->meshAssetID];

	glUseProgram(shader);

	// Apply the vertex state
	_setMeshState(shader, mesh->localTransform, data, mesh->skinnedPose, world, view, proj, viewPos);

	// Apply the material state
	_setMaterialState(shader, &mesh->materialState, nullptr);

	// Draw the mesh
	glBindVertexArray(data->vertexArray);
	glDepthFunc(GL_LESS);

	// Write to depth first for self-occlusion
	if (mesh->visibilityMask & ovrAvatarVisibilityFlag_SelfOccluding)
	{
		glDepthMask(GL_TRUE);
		glColorMaski(0, GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);
		glDrawElements(GL_TRIANGLES, (GLsizei)data->elementCount, GL_UNSIGNED_SHORT, 0);
		glDepthFunc(GL_EQUAL);
	}

	// Render to color buffer
	glDepthMask(GL_FALSE);
	glColorMaski(0, GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
	glDrawElements(GL_TRIANGLES, (GLsizei)data->elementCount, GL_UNSIGNED_SHORT, 0);
	glBindVertexArray(0);

	if (renderJoints)
	{
		glm::mat4 local;
		_glmFromOvrAvatarTransform(mesh->localTransform, &local);
		glDepthFunc(GL_ALWAYS);
		_renderPose(proj * view * world * local, mesh->skinnedPose);
	}
}

static void _renderSkinnedMeshPartPBS(const ovrAvatarRenderPart_SkinnedMeshRenderPBS* mesh, uint32_t visibilityMask, const glm::mat4& world, const glm::mat4& view, const glm::mat4 proj, const glm::vec3& viewPos, bool renderJoints)
{
	// If this part isn't visible from the viewpoint we're rendering from, do nothing
	if ((mesh->visibilityMask & visibilityMask) == 0)
	{
		return;
	}

	// Get the GL mesh data for this mesh's asset
	MeshData* data = (MeshData*)_assetMap[mesh->meshAssetID];

	glUseProgram(_skinnedMeshPBSProgram);

	// Apply the vertex state
	_setMeshState(_skinnedMeshPBSProgram, mesh->localTransform, data, mesh->skinnedPose, world, view, proj, viewPos);

	// Apply the material state
	_setPBSState(_skinnedMeshPBSProgram, mesh->albedoTextureAssetID, mesh->surfaceTextureAssetID);

	// Draw the mesh
	glBindVertexArray(data->vertexArray);
	glDepthFunc(GL_LESS);

	// Write to depth first for self-occlusion
	if (mesh->visibilityMask & ovrAvatarVisibilityFlag_SelfOccluding)
	{
		glDepthMask(GL_TRUE);
		glColorMaski(0, GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);
		glDrawElements(GL_TRIANGLES, (GLsizei)data->elementCount, GL_UNSIGNED_SHORT, 0);
		glDepthFunc(GL_EQUAL);
	}
	glDepthMask(GL_FALSE);

	// Draw the mesh
	glColorMaski(0, GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
	glDrawElements(GL_TRIANGLES, (GLsizei)data->elementCount, GL_UNSIGNED_SHORT, 0);
	glBindVertexArray(0);

	if (renderJoints)
	{
		glm::mat4 local;
		_glmFromOvrAvatarTransform(mesh->localTransform, &local);
		glDepthFunc(GL_ALWAYS);
		_renderPose(proj * view * world * local, mesh->skinnedPose);
	}
}

static void _renderProjector(const ovrAvatarRenderPart_ProjectorRender* projector, ovrAvatar* avatar, uint32_t visibilityMask, const glm::mat4& world, const glm::mat4& view, const glm::mat4 proj, const glm::vec3& viewPos)
{

	// Compute the mesh transform
	const ovrAvatarComponent* component = ovrAvatarComponent_Get(avatar, projector->componentIndex);
	const ovrAvatarRenderPart* renderPart = component->renderParts[projector->renderPartIndex];
	const ovrAvatarRenderPart_SkinnedMeshRender* mesh = ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart);

	// If this part isn't visible from the viewpoint we're rendering from, do nothing
	if ((mesh->visibilityMask & visibilityMask) == 0)
	{
		return;
	}

	// Compute the projection matrix
	glm::mat4 projection;
	_glmFromOvrAvatarTransform(projector->localTransform, &projection);
	glm::mat4 worldProjection = world * projection;
	glm::mat4 projectionInv = glm::inverse(worldProjection);

	// Compute the mesh transform
	glm::mat4 meshWorld;
	_glmFromOvrAvatarTransform(component->transform, &meshWorld);

	// Get the GL mesh data for this mesh's asset
	MeshData* data = (MeshData*)_assetMap[mesh->meshAssetID];

	glUseProgram(_skinnedMeshProgram);

	// Apply the vertex state
	_setMeshState(_skinnedMeshProgram, mesh->localTransform, data, mesh->skinnedPose, meshWorld, view, proj, viewPos);

	// Apply the material state
	_setMaterialState(_skinnedMeshProgram, &projector->materialState, &projectionInv);

	// Draw the mesh
	glBindVertexArray(data->vertexArray);
	glDepthMask(GL_FALSE);
	glDepthFunc(GL_EQUAL);
	glDrawElements(GL_TRIANGLES, (GLsizei)data->elementCount, GL_UNSIGNED_SHORT, 0);
	glBindVertexArray(0);
}

static void _renderAvatar(ovrAvatar* avatar, uint32_t visibilityMask, const glm::mat4& view, const glm::mat4& proj, const glm::vec3& viewPos, bool renderJoints)
{
	// Traverse over all components on the avatar
	uint32_t componentCount = ovrAvatarComponent_Count(avatar);

	const ovrAvatarComponent* bodyComponent = nullptr;
	if (const ovrAvatarBodyComponent* body = ovrAvatarPose_GetBodyComponent(avatar))
	{
		bodyComponent = body->renderComponent;
	}

	for (uint32_t i = 0; i < componentCount; ++i)
	{
		const ovrAvatarComponent* component = ovrAvatarComponent_Get(avatar, i);

		const bool useCombinedMeshProgram = _combineMeshes && bodyComponent == component;

		// Compute the transform for this component
		glm::mat4 world;
		_glmFromOvrAvatarTransform(component->transform, &world);

		// Render each render part attached to the component
		for (uint32_t j = 0; j < component->renderPartCount; ++j)
		{
			const ovrAvatarRenderPart* renderPart = component->renderParts[j];
			ovrAvatarRenderPartType type = ovrAvatarRenderPart_GetType(renderPart);
			switch (type)
			{
			case ovrAvatarRenderPartType_SkinnedMeshRender:
				_renderSkinnedMeshPart(useCombinedMeshProgram ? _combinedMeshProgram : _skinnedMeshProgram, ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart), visibilityMask, world, view, proj, viewPos, renderJoints);
				break;
			case ovrAvatarRenderPartType_SkinnedMeshRenderPBS:
				_renderSkinnedMeshPartPBS(ovrAvatarRenderPart_GetSkinnedMeshRenderPBS(renderPart), visibilityMask, world, view, proj, viewPos, renderJoints);
				break;
			case ovrAvatarRenderPartType_ProjectorRender:
				_renderProjector(ovrAvatarRenderPart_GetProjectorRender(renderPart), avatar, visibilityMask, world, view, proj, viewPos);
				break;
			}
		}
	}
}

static void _updateAvatar(
	ovrAvatar* avatar,
	float deltaSeconds,
	const ovrAvatarTransform& hmd,
	const ovrAvatarHandInputState& left,
	const ovrAvatarHandInputState& right,
	ovrMicrophone* mic,
	ovrAvatarPacket* packet,
	float* packetPlaybackTime
) {
	if (packet)
	{
		float packetDuration = ovrAvatarPacket_GetDurationSeconds(packet);
		*packetPlaybackTime += deltaSeconds;
		if (*packetPlaybackTime > packetDuration)
		{
			ovrAvatarPose_Finalize(avatar, 0.0f);
			*packetPlaybackTime = 0;
		}
		ovrAvatar_UpdatePoseFromPacket(avatar, packet, *packetPlaybackTime);
	}
	else
	{
		// If we have a mic update the voice visualization
		if (mic)
		{
			float micSamples[48000];
			size_t sampleCount = ovr_Microphone_ReadData(mic, micSamples, sizeof(micSamples) / sizeof(micSamples[0]));
			if (sampleCount > 0)
			{
				ovrAvatarPose_UpdateVoiceVisualization(_avatar, (uint32_t)sampleCount, micSamples);
			}
		}

		// Update the avatar pose from the inputs
		ovrAvatarPose_UpdateBody(avatar, hmd);
		ovrAvatarPose_UpdateHands(avatar, left, right);
	}
	ovrAvatarPose_Finalize(avatar, deltaSeconds);
}


/************************************************************************************
* OVR helpers
************************************************************************************/

static ovrSession _initOVR()
{
	ovrSession ovr;
	if (OVR_SUCCESS(ovr_Initialize(NULL)))
	{
		ovrGraphicsLuid luid;
		if (OVR_SUCCESS(ovr_Create(&ovr, &luid)))
		{
			return ovr;
		}
		ovr_Shutdown();
	}
	return NULL;
}

static void _destroyOVR(ovrSession session)
{
	if (session)
	{
		ovr_Destroy(session);
		ovr_Shutdown();
	}
}

/************************************************************************************
* Avatar message handlers
************************************************************************************/

static void _handleAvatarSpecification(const ovrAvatarMessage_AvatarSpecification* message)
{
	// Create the avatar instance
	_avatar = ovrAvatar_Create(message->avatarSpec, ovrAvatarCapability_All);

	// Trigger load operations for all of the assets referenced by the avatar
	uint32_t refCount = ovrAvatar_GetReferencedAssetCount(_avatar);
	for (uint32_t i = 0; i < refCount; ++i)
	{
		ovrAvatarAssetID id = ovrAvatar_GetReferencedAsset(_avatar, i);
		ovrAvatarAsset_BeginLoading(id);
		++_loadingAssets;
	}
	printf("Loading %d assets...\r\n", _loadingAssets);
}

static void _handleAssetLoaded(const ovrAvatarMessage_AssetLoaded* message)
{
	// Determine the type of the asset that got loaded
	ovrAvatarAssetType assetType = ovrAvatarAsset_GetType(message->asset);
	void* data = nullptr;

	// Call the appropriate loader function
	switch (assetType)
	{
	case ovrAvatarAssetType_Mesh:
		data = _loadMesh(ovrAvatarAsset_GetMeshData(message->asset));
		break;
	case ovrAvatarAssetType_Texture:
		data = _loadTexture(ovrAvatarAsset_GetTextureData(message->asset));
		break;
	case ovrAvatarAssetType_CombinedMesh:
		data = _loadCombinedMesh(ovrAvatarAsset_GetCombinedMeshData(message->asset));
		break;
	default:
		break;
	}

	// Store the data that we loaded for the asset in the asset map
	_assetMap[message->assetID] = data;

	if (assetType == ovrAvatarAssetType_CombinedMesh)
	{
		uint32_t idCount = 0;
		ovrAvatarAsset_GetCombinedMeshIDs(message->asset, &idCount);
		_loadingAssets -= idCount;
		ovrAvatar_GetCombinedMeshAlphaData(_avatar, &_avatarCombinedMeshAlpha, &_avatarCombinedMeshAlphaOffset);
	}
	else
	{
		--_loadingAssets;
	}

	printf("Loading %d assets...\r\n", _loadingAssets);
}


/************************************************************************************
* Main entrypoint
************************************************************************************/

#undef main
int main(int argc, char** argv)
{
	// Initialize the platform module
	if (ovr_PlatformInitializeWindows(MIRROR_SAMPLE_APP_ID) != ovrPlatformInitialize_Success)
	{
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", "Failed to initialize the Oculus platform", NULL);
		return 1;
	}

	// Attempt to initialize the Oculus SDK
	ovrSession ovr = MIRROR_ALLOW_OVR ? _initOVR() : 0;
	if (!ovr)
	{
		printf("OVR not initialized - rendering to 2D viewport...\r\n");
	}

	// Initialize SDL
	if (SDL_Init(SDL_INIT_VIDEO) != 0)
	{
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", "Couldn't start SDL.", NULL);
		_destroyOVR(ovr);
		return 1;
	}

	// Create the application window
	SDL_Window* window = SDL_CreateWindow("Mirror", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, MIRROR_WINDOW_WIDTH, MIRROR_WINDOW_HEIGHT, SDL_WINDOW_OPENGL);
	if (!window)
	{
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", "Couldn't create an SDL window.", NULL);
		SDL_Quit();
		_destroyOVR(ovr);
		return 1;
	}

	// Initialize GL
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
	SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);

	SDL_GLContext glContext = SDL_GL_CreateContext(window);
	glEnable(GL_CULL_FACE);
	glEnable(GL_DEPTH_TEST);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glClearColor(0.5f, 0.5f, 0.5f, 0.0f);
	glClearDepth(1.0f);

	glewExperimental = 1;
	glewInit();

	// Compile the reference shaders
	char errorBuffer[512];
	_skinnedMeshProgram = _compileProgramFromFiles("AvatarVertexShader.glsl", "AvatarFragmentShader.glsl", sizeof(errorBuffer), errorBuffer);
	if (!_skinnedMeshProgram) {
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", errorBuffer, NULL);
		SDL_Quit();
		_destroyOVR(ovr);
		return 1;
	}
	_skinnedMeshPBSProgram = _compileProgramFromFiles("AvatarVertexShader.glsl", "AvatarFragmentShaderPBS.glsl", sizeof(errorBuffer), errorBuffer);
	if (!_skinnedMeshPBSProgram) {
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", errorBuffer, NULL);
		SDL_Quit();
		_destroyOVR(ovr);
		return 1;
	}

	_combinedMeshProgram = _compileProgramFromFiles("AvatarVertexShader.glsl", "AvatarFragmentShader_CombinedMesh.glsl", sizeof(errorBuffer), errorBuffer);
	if (!_combinedMeshProgram) {
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", errorBuffer, NULL);
		SDL_Quit();
		_destroyOVR(ovr);
		return 1;
	}

	const char debugLineVertexShader[] =
		"#version 330 core\n"
		"layout (location = 0) in vec3 position;\n"
		"layout (location = 1) in vec4 color;\n"
		"out vec4 vertexColor;\n"
		"uniform mat4 worldViewProj;\n"
		"void main() {\n"
		"    gl_Position = worldViewProj * vec4(position, 1.0);\n"
		"    vertexColor = color;\n"
		"}";

	const char debugLineFragmentShader[] =
		"#version 330 core\n"
		"in vec4 vertexColor;\n"
		"out vec4 fragmentColor;\n"
		"void main() {\n"
		"    fragmentColor = vertexColor;"
		"}";

	_debugLineProgram = _compileProgramFromSource(debugLineVertexShader, debugLineFragmentShader, sizeof(errorBuffer), errorBuffer);
	if (!_debugLineProgram) {
		SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "Mirror startup error", errorBuffer, NULL);
		SDL_Quit();
		_destroyOVR(ovr);
		return 1;
	}

	glGenVertexArrays(1, &_debugVertexArray);
	glGenBuffers(1, &_debugVertexBuffer);

	// Create the microphone for voice effects
	ovrMicrophoneHandle mic = ovr_Microphone_Create();
	if (mic)
	{
		ovr_Microphone_Start(mic);
	}

    // If we're in VR mode, initialize the swap chain
    ovrHmdDesc hmdDesc;
    GLuint mirrorFBO = 0;
    ovrTextureSwapChain eyeSwapChains[2];
    GLuint eyeFrameBuffers[2];
    GLuint eyeDepthBuffers[2];
    ovrSizei eyeSizes[2];
    if (ovr)
    {
        // Get the buffer size we need for rendering
        hmdDesc = ovr_GetHmdDesc(ovr);        
        for (int eye = 0; eye < 2; ++eye)
        {
            eyeSizes[eye] = ovr_GetFovTextureSize(ovr, (ovrEyeType)eye, hmdDesc.DefaultEyeFov[eye], 1.0f);

            // Create the swap chain
            ovrTextureSwapChainDesc desc;
            memset(&desc, 0, sizeof(desc));
            desc.Type = ovrTexture_2D;
            desc.ArraySize = 1;
            desc.Format = OVR_FORMAT_R8G8B8A8_UNORM_SRGB;
            desc.Width = eyeSizes[eye].w;
            desc.Height = eyeSizes[eye].h;
            desc.MipLevels = 1;
            desc.SampleCount = 1;
            desc.StaticImage = ovrFalse;
            ovr_CreateTextureSwapChainGL(ovr, &desc, &eyeSwapChains[eye]);

            int length = 0;
            ovr_GetTextureSwapChainLength(ovr, eyeSwapChains[eye], &length);
            for (int i = 0; i < length; ++i)
            {
                GLuint chainTexId;
                ovr_GetTextureSwapChainBufferGL(ovr, eyeSwapChains[eye], i, &chainTexId);
                glBindTexture(GL_TEXTURE_2D, chainTexId);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            }
            glGenFramebuffers(1, &eyeFrameBuffers[eye]);

            glGenTextures(1, &eyeDepthBuffers[eye]);
            glBindTexture(GL_TEXTURE_2D, eyeDepthBuffers[eye]);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT24, eyeSizes[eye].w, eyeSizes[eye].h, 0, GL_DEPTH_COMPONENT, GL_UNSIGNED_INT, NULL);
        }

        // Create mirror buffer
        ovrMirrorTextureDesc mirrorDesc;
        memset(&mirrorDesc, 0, sizeof(mirrorDesc));
        mirrorDesc.Width = MIRROR_WINDOW_WIDTH;
        mirrorDesc.Height = MIRROR_WINDOW_HEIGHT;
        mirrorDesc.Format = OVR_FORMAT_R8G8B8A8_UNORM_SRGB;

        ovrMirrorTexture mirrorTexture;
        ovr_CreateMirrorTextureGL(ovr, &mirrorDesc, &mirrorTexture);

        GLuint mirrorTextureID;
        ovr_GetMirrorTextureBufferGL(ovr, mirrorTexture, &mirrorTextureID);

        glGenFramebuffers(1, &mirrorFBO);
        glBindFramebuffer(GL_READ_FRAMEBUFFER, mirrorFBO);
        glFramebufferTexture2D(GL_READ_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, mirrorTextureID, 0);
        glFramebufferRenderbuffer(GL_READ_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, 0);
        glBindFramebuffer(GL_READ_FRAMEBUFFER, 0);
    }

	// Initialize the avatar module
	ovrAvatar_Initialize(MIRROR_SAMPLE_APP_ID);

	// Start retrieving the avatar specification
	printf("Requesting avatar specification...\r\n");
	ovrID userID = ovr_GetLoggedInUserID();

	_waitingOnCombinedMesh = _combineMeshes;
	auto requestSpec = ovrAvatarSpecificationRequest_Create(userID);
	ovrAvatarSpecificationRequest_SetCombineMeshes(requestSpec, _combineMeshes);
	ovrAvatar_RequestAvatarSpecificationFromSpecRequest(requestSpec);
	ovrAvatarSpecificationRequest_Destroy(requestSpec);

	SDL_GL_SetSwapInterval(0);

	// Recenter the tracking origin at startup so that the reflection avatar appears directly in front of the user
	if (ovr)
	{
		ovr_SetTrackingOriginType(ovr, ovrTrackingOrigin_FloorLevel);
		ovr_RecenterTrackingOrigin(ovr);
	}

	// Run the main loop
	bool recording = false;
	bool controllersVisible = false;
	bool customBasePosition = false;
	bool renderJoints = false;
	bool freezePose = false;
	int capabilities = ovrAvatarCapability_All;
	bool running = true;
	long long frameIndex = 0;
	ovrAvatarPacket* playbackPacket = nullptr;
	float playbackTime = 0;
	std::chrono::steady_clock::time_point lastTime = std::chrono::steady_clock::now();
	uint64_t testUserID = 0;
	while (running) {

		// Pump avatar messages
		while (ovrAvatarMessage* message = ovrAvatarMessage_Pop())
		{
			switch (ovrAvatarMessage_GetType(message))
			{
				case ovrAvatarMessageType_AvatarSpecification:
					_handleAvatarSpecification(ovrAvatarMessage_GetAvatarSpecification(message));
					break;
				case ovrAvatarMessageType_AssetLoaded:
					_handleAssetLoaded(ovrAvatarMessage_GetAssetLoaded(message));
					break;
			}
			ovrAvatarMessage_Free(message);
		}

		// Pump SDL messages
		SDL_Event sdlEvent;
		while (SDL_PollEvent(&sdlEvent)) {
			switch (sdlEvent.type) {
			case SDL_KEYDOWN:
				switch (sdlEvent.key.keysym.sym) {
					case SDLK_RIGHT:
					{
						testUserID++;
						ovrAvatar_Destroy(_avatar);
						_avatar = nullptr;
						_waitingOnCombinedMesh = _combineMeshes;
						auto requestSpec = ovrAvatarSpecificationRequest_Create(testUserID);
						ovrAvatarSpecificationRequest_SetCombineMeshes(requestSpec, _combineMeshes);
						ovrAvatar_RequestAvatarSpecificationFromSpecRequest(requestSpec);
						ovrAvatarSpecificationRequest_Destroy(requestSpec);
						printf("Requesting avatar specification %d...\r\n", testUserID);
					}
						break;
					case SDLK_LEFT:
						if (testUserID > 0)
						{
							ovrAvatar_Destroy(_avatar);
							_avatar = nullptr;
							testUserID--;
							_waitingOnCombinedMesh = _combineMeshes;
							auto requestSpec = ovrAvatarSpecificationRequest_Create(testUserID);
							ovrAvatarSpecificationRequest_SetCombineMeshes(requestSpec, _combineMeshes);
							ovrAvatar_RequestAvatarSpecificationFromSpecRequest(requestSpec);
							ovrAvatarSpecificationRequest_Destroy(requestSpec);
							printf("Requesting avatar specification %d...\r\n", testUserID);
						}
						break;
					case 'b':
					{
						customBasePosition = !customBasePosition;
						if (customBasePosition)
						{							
							ovrAvatar_SetCustomBasePosition(_avatar, ovrAvatarPose_GetBaseComponent(_avatar)->basePosition);
						}
						else
						{
							ovrAvatar_ClearCustomBasePosition(_avatar);
						}
						break;
					}
					case 'c':
					{
						controllersVisible = !controllersVisible;
						ovrAvatar_SetLeftControllerVisibility(_avatar, controllersVisible);
						ovrAvatar_SetRightControllerVisibility(_avatar, controllersVisible);
						break;
					}
					case 'f':
					{
						freezePose = !freezePose;
						if (freezePose) {
							const ovrAvatarHandComponent* handComp =
								ovrAvatarPose_GetLeftHandComponent(_avatar);
							const ovrAvatarComponent* comp = handComp->renderComponent;
							const ovrAvatarRenderPart* renderPart = comp->renderParts[0];
							const ovrAvatarRenderPart_SkinnedMeshRender* meshRender =
								ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart);
							ovrAvatar_SetLeftHandCustomGesture(_avatar,
								meshRender->skinnedPose.jointCount,
								meshRender->skinnedPose.jointTransform);
							handComp =
								ovrAvatarPose_GetRightHandComponent(_avatar);
							comp = handComp->renderComponent;
							renderPart = comp->renderParts[0];
							meshRender = ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart);
							ovrAvatar_SetRightHandCustomGesture(_avatar,
								meshRender->skinnedPose.jointCount,
								meshRender->skinnedPose.jointTransform);
						}
						else {
							ovrAvatar_SetLeftHandGesture(_avatar, ovrAvatarHandGesture_Default);
							ovrAvatar_SetRightHandGesture(_avatar, ovrAvatarHandGesture_Default);
						}
						break;
					}
					case 'j':
						renderJoints = !renderJoints;
						break;
					case 'm':
						_combineMeshes = !_combineMeshes;
						break;
					case 'u':
						freezePose = true;
						ovrAvatar_SetLeftHandGesture(_avatar, ovrAvatarHandGesture_GripCube);
						ovrAvatar_SetRightHandGesture(_avatar, ovrAvatarHandGesture_GripCube);
						break;
					case 's':
						freezePose = true;
						ovrAvatar_SetLeftHandGesture(_avatar, ovrAvatarHandGesture_GripSphere);
						ovrAvatar_SetRightHandGesture(_avatar, ovrAvatarHandGesture_GripSphere);
						break;
					case '1':
						capabilities ^= ovrAvatarCapability_Body;
						ovrAvatar_SetActiveCapabilities(_avatar, static_cast<ovrAvatarCapabilities>(capabilities));
						break;
					case '2':
						capabilities ^= ovrAvatarCapability_Hands;
						ovrAvatar_SetActiveCapabilities(_avatar, static_cast<ovrAvatarCapabilities>(capabilities));
						break;
					case '3':
						capabilities ^= ovrAvatarCapability_Base;
						ovrAvatar_SetActiveCapabilities(_avatar, static_cast<ovrAvatarCapabilities>(capabilities));
						break;
					case '4':
						capabilities ^= ovrAvatarCapability_Voice;
						ovrAvatar_SetActiveCapabilities(_avatar, static_cast<ovrAvatarCapabilities>(capabilities));
						break;
					case 'r':
						if (!recording)
						{
							printf("Recording avatar packet...\r\n");
							if (playbackPacket)
							{
								ovrAvatarPacket_Free(playbackPacket);
								playbackPacket = nullptr;
								playbackTime = 0;
							}
							ovrAvatarPacket_BeginRecording(_avatar);
							recording = true;
						}
						else
						{
							// Finish the recording
							ovrAvatarPacket* recordedPacket = ovrAvatarPacket_EndRecording(_avatar);

							// Write the packet to a byte buffer to exercise the packet writing code
							uint32_t packetSize = ovrAvatarPacket_GetSize(recordedPacket);
							uint8_t* packetBuffer = (uint8_t*)malloc(packetSize);
							ovrAvatarPacket_Write(recordedPacket, packetSize, packetBuffer);
							ovrAvatarPacket_Free(recordedPacket);

							// Read the buffer back into a packet to exericse the packet reading code
							playbackPacket = ovrAvatarPacket_Read(packetSize, packetBuffer);
							free(packetBuffer);

							float duration = ovrAvatarPacket_GetDurationSeconds(playbackPacket);
							printf("Playing back recorded packet (%.3f KB/s) ...\r\n", packetSize / (1024 * duration));
							recording = false;
						}
				}
				break;
			case SDL_QUIT:
				running = false;
				break;
			}
		}
		if (!running)
			break;

		// Compute how much time has elapsed since the last frame
		std::chrono::steady_clock::time_point currentTime = std::chrono::steady_clock::now();
		std::chrono::duration<float> deltaTime = currentTime - lastTime;
		float deltaSeconds = deltaTime.count();
		lastTime = currentTime;
		_elapsedSeconds += deltaSeconds;

		// Do VR rendering
        if (ovr)
        {

            // Call ovr_GetRenderDesc each frame to get the ovrEyeRenderDesc, as the returned values (e.g. HmdToEyeOffset) may change at runtime.
            ovrEyeRenderDesc eyeRenderDesc[2];
            eyeRenderDesc[0] = ovr_GetRenderDesc(ovr, ovrEye_Left, hmdDesc.DefaultEyeFov[0]);
            eyeRenderDesc[1] = ovr_GetRenderDesc(ovr, ovrEye_Right, hmdDesc.DefaultEyeFov[1]);

            // Get eye poses, feeding in correct IPD offset
            ovrPosef                  eyeRenderPose[2];
            ovrVector3f               hmdToEyeOffset[2] = { eyeRenderDesc[0].HmdToEyeOffset, eyeRenderDesc[1].HmdToEyeOffset };
            double sensorSampleTime;
            ovr_GetEyePoses(ovr, frameIndex, ovrTrue, hmdToEyeOffset, eyeRenderPose, &sensorSampleTime);

			// If the avatar is initialized, update it
			if (_avatar)
			{
				// Convert the OVR inputs into Avatar SDK inputs
				ovrInputState touchState;
				ovr_GetInputState(ovr, ovrControllerType_Active, &touchState);
				ovrTrackingState trackingState = ovr_GetTrackingState(ovr, 0.0, false);
				
				glm::vec3 hmdP = _glmFromOvrVector(trackingState.HeadPose.ThePose.Position);
				glm::quat hmdQ = _glmFromOvrQuat(trackingState.HeadPose.ThePose.Orientation);
				glm::vec3 leftP = _glmFromOvrVector(trackingState.HandPoses[ovrHand_Left].ThePose.Position);
				glm::quat leftQ = _glmFromOvrQuat(trackingState.HandPoses[ovrHand_Left].ThePose.Orientation);
				glm::vec3 rightP = _glmFromOvrVector(trackingState.HandPoses[ovrHand_Right].ThePose.Position);
				glm::quat rightQ = _glmFromOvrQuat(trackingState.HandPoses[ovrHand_Right].ThePose.Orientation);

				ovrAvatarTransform hmd;
				_ovrAvatarTransformFromGlm(hmdP, hmdQ, glm::vec3(1.0f), &hmd);

				ovrAvatarTransform left;
				_ovrAvatarTransformFromGlm(leftP, leftQ, glm::vec3(1.0f), &left);

				ovrAvatarTransform right;
				_ovrAvatarTransformFromGlm(rightP, rightQ, glm::vec3(1.0f), &right);

				ovrAvatarHandInputState inputStateLeft;
				_ovrAvatarHandInputStateFromOvr(left, touchState, ovrHand_Left, &inputStateLeft);

				ovrAvatarHandInputState inputStateRight;
				_ovrAvatarHandInputStateFromOvr(right, touchState, ovrHand_Right, &inputStateRight);

				_updateAvatar(_avatar, deltaSeconds, hmd, inputStateLeft, inputStateRight, mic, playbackPacket, &playbackTime);
			}

            // Render each eye
            for (int eye = 0; eye < 2; ++eye)
            {
                // Switch to eye render target
                int curIndex;
                GLuint curTexId;
                ovr_GetTextureSwapChainCurrentIndex(ovr, eyeSwapChains[eye], &curIndex);
                ovr_GetTextureSwapChainBufferGL(ovr, eyeSwapChains[eye], curIndex, &curTexId);

                glBindFramebuffer(GL_FRAMEBUFFER, eyeFrameBuffers[eye]);
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, curTexId, 0);
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, eyeDepthBuffers[eye], 0);

                glViewport(0, 0, eyeSizes[eye].w, eyeSizes[eye].h);
				glDepthMask(GL_TRUE);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
                glEnable(GL_FRAMEBUFFER_SRGB);

                ovrVector3f eyePosition = eyeRenderPose[eye].Position;
                ovrQuatf eyeOrientation = eyeRenderPose[eye].Orientation;
                glm::quat glmOrientation = _glmFromOvrQuat(eyeOrientation);
                glm::vec3 eyeWorld = _glmFromOvrVector(eyePosition);
                glm::vec3 eyeForward =  glmOrientation * glm::vec3(0, 0, -1);
                glm::vec3 eyeUp = glmOrientation * glm::vec3(0, 1, 0);
                glm::mat4 view = glm::lookAt(eyeWorld, eyeWorld + eyeForward, eyeUp);

                ovrMatrix4f ovrProjection = ovrMatrix4f_Projection(hmdDesc.DefaultEyeFov[eye], 0.01f, 1000.0f, ovrProjection_None);
                glm::mat4 proj(
                    ovrProjection.M[0][0], ovrProjection.M[1][0], ovrProjection.M[2][0], ovrProjection.M[3][0],
                    ovrProjection.M[0][1], ovrProjection.M[1][1], ovrProjection.M[2][1], ovrProjection.M[3][1],
                    ovrProjection.M[0][2], ovrProjection.M[1][2], ovrProjection.M[2][2], ovrProjection.M[3][2],
                    ovrProjection.M[0][3], ovrProjection.M[1][3], ovrProjection.M[2][3], ovrProjection.M[3][3]
                );

				// If we have the avatar and have finished loading assets, render it
				if (_avatar && !_loadingAssets && !_waitingOnCombinedMesh)
				{
					_renderAvatar(_avatar, ovrAvatarVisibilityFlag_FirstPerson, view, proj, eyeWorld, renderJoints);
					
					glm::vec4 reflectionPlane = glm::vec4(0.0, 0.0, -1.0, 0.0);
					glm::mat4 reflection = _computeReflectionMatrix(reflectionPlane);

					glFrontFace(GL_CW);
					_renderAvatar(_avatar, ovrAvatarVisibilityFlag_ThirdPerson, view * reflection, proj, glm::vec3(reflection * glm::vec4(eyeWorld, 1.0f)), renderJoints);
					glFrontFace(GL_CCW);
				}

                // Unbind the eye buffer
                glBindFramebuffer(GL_FRAMEBUFFER, eyeFrameBuffers[eye]);
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, 0, 0);
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, 0, 0);

                // Commit changes to the textures so they get picked up frame
                ovr_CommitTextureSwapChain(ovr, eyeSwapChains[eye]);
            } 

            // Prepare the layers
            ovrLayerEyeFov layerDesc;
            memset(&layerDesc, 0, sizeof(layerDesc));
            layerDesc.Header.Type = ovrLayerType_EyeFov;
            layerDesc.Header.Flags = ovrLayerFlag_TextureOriginAtBottomLeft;   // Because OpenGL.
            for (int eye = 0; eye < 2; ++eye)
            {
                layerDesc.ColorTexture[eye] = eyeSwapChains[eye];
                layerDesc.Viewport[eye].Size = eyeSizes[eye];
                layerDesc.Fov[eye] = hmdDesc.DefaultEyeFov[eye];
                layerDesc.RenderPose[eye] = eyeRenderPose[eye];
                layerDesc.SensorSampleTime = sensorSampleTime;
            }

            ovrLayerHeader* layers = &layerDesc.Header;
            ovr_SubmitFrame(ovr, frameIndex, NULL, &layers, 1);

            ovrSessionStatus sessionStatus;
            ovr_GetSessionStatus(ovr, &sessionStatus);
            if (sessionStatus.ShouldQuit)
                running = false;
            if (sessionStatus.ShouldRecenter)
                ovr_RecenterTrackingOrigin(ovr);

            // Blit mirror texture to back buffer
            glBindFramebuffer(GL_READ_FRAMEBUFFER, mirrorFBO);
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
            glBlitFramebuffer(0, MIRROR_WINDOW_HEIGHT, MIRROR_WINDOW_WIDTH, 0, 0, 0, MIRROR_WINDOW_WIDTH, MIRROR_WINDOW_HEIGHT, GL_COLOR_BUFFER_BIT, GL_NEAREST);
            glBindFramebuffer(GL_READ_FRAMEBUFFER, 0);
        }

		// Render to 2D viewport
		else
		{
			glDepthMask(GL_TRUE);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			glm::vec3 eyePos = glm::vec3(0, 1.0f, -2.5f);
			glm::vec3 eyeTarget = glm::vec3(0, 1.0f, 0.0f);
			glm::mat4 view = glm::lookAt(eyePos, eyeTarget, glm::vec3(0, 1, 0));
			glm::mat4 proj = glm::perspectiveFov(glm::radians(45.0f), (float)MIRROR_WINDOW_WIDTH, (float)MIRROR_WINDOW_HEIGHT, 0.01f, 100000.0f);
			if (_avatar && !_loadingAssets)
			{
				// Compute the total elapsed time so that we can animate a rotation of the avatar
				static bool rotate = true;
				static float rotateTheta;
				if (rotate)
				{
					rotateTheta += deltaSeconds;
					while (rotateTheta > glm::radians(360.0f))
					{
						rotateTheta -= glm::radians(360.0f);
					}
				}

				// Compute poses for each of the components
				glm::quat orientation = glm::quat(glm::vec3(0, rotateTheta, 0));
				glm::vec3 bodyPosition = orientation * glm::vec3(0, 1.75f, 0.25f);
				glm::vec3 handLeftPosition = orientation * glm::vec3(-0.25, 1.5f, -0.25);
				glm::vec3 handRightPosition = orientation * glm::vec3(0.25, 1.5f, -0.25);

				ovrAvatarTransform bodyPose, handLeftPose, handRightPose;
				_ovrAvatarTransformFromGlm(bodyPosition, orientation, glm::vec3(1, 1, 1), &bodyPose);
				_ovrAvatarTransformFromGlm(handLeftPosition, orientation, glm::vec3(1, 1, 1), &handLeftPose);
				_ovrAvatarTransformFromGlm(handRightPosition, orientation, glm::vec3(1, 1, 1), &handRightPose);

				// Synthesize some input
				ovrInputState inputState;
				memset(&inputState, 0, sizeof(inputState));
				inputState.ControllerType = ovrControllerType_Touch;
				inputState.Touches |= ovrTouch_LIndexPointing;
				inputState.Touches |= ovrTouch_RThumbUp;
				inputState.HandTrigger[ovrHand_Left] = 0.5;
				inputState.HandTrigger[ovrHand_Right] = 1.0;

				ovrAvatarHandInputState leftInputState;
				_ovrAvatarHandInputStateFromOvr(handLeftPose, inputState, ovrHand_Left, &leftInputState);

				ovrAvatarHandInputState rightInputState;
				_ovrAvatarHandInputStateFromOvr(handRightPose, inputState, ovrHand_Right, &rightInputState);

				_updateAvatar(_avatar, deltaSeconds, bodyPose, leftInputState, rightInputState, mic, playbackPacket, &playbackTime);

				// Render the avatar
				_renderAvatar(_avatar, ovrAvatarVisibilityFlag_ThirdPerson, view, proj, eyePos, renderJoints);
			}
		}

    	SDL_GL_SwapWindow(window);
        ++frameIndex;
	}

	printf("Shutting down...\r\n");
	if (_avatar)
	{
		ovrAvatar_Destroy(_avatar);
	}
    ovrAvatar_Shutdown();

	if (mic)
	{
		ovr_Microphone_Destroy(mic);
	}

	_destroyOVR(ovr);
	SDL_GL_DeleteContext(glContext);
	SDL_DestroyWindow(window);
	SDL_Quit();
	return 0;
}
