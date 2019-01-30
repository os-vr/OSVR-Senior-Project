cmake_minimum_required(VERSION 3.0)
project(Mirror)

add_definitions(-DGLEW_STATIC)
add_definitions(-D_CRT_SECURE_NO_WARNINGS)
include_directories(../Include)
include_directories(Dependencies/glm)
include_directories(Dependencies/glew/include)
include_directories(Dependencies/OVR/Include)
include_directories(Dependencies/OVRPlatform/Include)
include_directories(Dependencies/SDL2/include)

add_executable(Mirror
	Mirror/Mirror.cpp
	ReferenceShaders/AvatarVertexShader.glsl
	ReferenceShaders/AvatarFragmentShader.glsl
	ReferenceShaders/AvatarFragmentShaderPBS.glsl
	${PROJECT_SOURCE_DIR}/Dependencies/glm/glm/detail/glm.cpp
	${PROJECT_SOURCE_DIR}/Dependencies/OVRPlatform/Windows/OVR_PlatformLoader.cpp)
target_link_libraries(Mirror
	opengl32.lib
	${PROJECT_SOURCE_DIR}/../Windows/libovravatar.lib
	${PROJECT_SOURCE_DIR}/Dependencies/glew/lib/glew32s.lib
	${PROJECT_SOURCE_DIR}/Dependencies/OVR/Lib/LibOVR.lib
	${PROJECT_SOURCE_DIR}/Dependencies/OVRPlatform/Windows/LibOVRPlatform64_1.lib
	${PROJECT_SOURCE_DIR}/Dependencies/SDL2/lib/x64/SDL2.lib)

	add_custom_command(TARGET Mirror POST_BUILD
		COMMAND ${CMAKE_COMMAND} -E copy_if_different
		${PROJECT_SOURCE_DIR}/ReferenceShaders/AvatarVertexShader.glsl
		$<TARGET_FILE_DIR:Mirror>
		COMMAND ${CMAKE_COMMAND} -E copy_if_different
		${PROJECT_SOURCE_DIR}/ReferenceShaders/AvatarFragmentShader.glsl
		$<TARGET_FILE_DIR:Mirror>
		COMMAND ${CMAKE_COMMAND} -E copy_if_different
		${PROJECT_SOURCE_DIR}/ReferenceShaders/AvatarFragmentShaderPBS.glsl
		$<TARGET_FILE_DIR:Mirror>
		COMMAND ${CMAKE_COMMAND} -E copy_if_different
		${PROJECT_SOURCE_DIR}/Dependencies/SDL2/lib/x64/SDL2.dll
		$<TARGET_FILE_DIR:Mirror>)
