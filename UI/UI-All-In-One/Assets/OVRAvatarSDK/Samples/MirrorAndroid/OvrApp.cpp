/************************************************************************************

Filename    :   OvrApp.cpp
Content     :   Trivial use of the application framework.
Created     :
Authors     :

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

*************************************************************************************/

#define APP_ID "1221388694647274"

#include "OvrApp.h"
#include "GuiSys.h"
#include "VrApi_Types.h"
#include "OVR_Locale.h"

#include "OVR_Platform.h"

#if defined( OVR_OS_WIN32 )
#include "../res_pc/resource.h"
#endif

using namespace OVR;

#if defined( OVR_OS_ANDROID )
extern "C" {

jlong Java_com_oculus_mirror_MainActivity_nativeSetAppInterface( JNIEnv * jni, jclass clazz, jobject activity,
		jstring fromPackageName, jstring commandString, jstring uriString )
{
	LOG( "nativeSetAppInterface" );
	return (new OvrTemplateApp::OvrApp())->SetActivity( jni, clazz, activity, fromPackageName, commandString, uriString );
}

} // extern "C"

#endif

namespace OvrTemplateApp
{

OvrApp::OvrApp()
	: SoundEffectContext( NULL )
	, SoundEffectPlayer( NULL )
	, GuiSys( OvrGuiSys::Create() )
	, Locale( NULL )
	, SceneModel( NULL )
	, TheAvatarManager()
{
}

OvrApp::~OvrApp()
{
	delete SoundEffectPlayer;
	SoundEffectPlayer = NULL;

	delete SoundEffectContext;
	SoundEffectContext = NULL;

	TheAvatarManager.Destroy();

	OvrGuiSys::Destroy( GuiSys );
	if ( SceneModel != NULL )
	{
		delete SceneModel;
	}
}

void OvrApp::Configure( ovrSettings & settings )
{
	settings.PerformanceParms.CpuLevel = 1;
	settings.PerformanceParms.GpuLevel = 3;
#if defined( OVR_OS_WIN32 )
	settings.WindowParms.IconResourceId = IDI_ICON1;
	settings.WindowParms.Title = "VrTemplate";		// TODO: Use VersionInfo.ProductName
#endif

	settings.RenderMode = RENDERMODE_MULTIVIEW;
}

void OvrApp::EnteredVrMode( const ovrIntentType intentType, const char * intentFromPackage, const char * intentJSON, const char * intentURI )
{
	OVR_UNUSED( intentFromPackage );
	OVR_UNUSED( intentJSON );
	OVR_UNUSED( intentURI );

	if ( intentType == INTENT_LAUNCH )
	{
		const ovrJava * java = app->GetJava();
		SoundEffectContext = new ovrSoundEffectContext( *java->Env, java->ActivityObject );
		SoundEffectContext->Initialize( &app->GetFileSys() );
		SoundEffectPlayer = new OvrGuiSys::ovrDummySoundEffectPlayer();

		Locale = ovrLocale::Create( *java->Env, java->ActivityObject, "default" );

		String fontName;
		GetLocale().GetString( "@string/font_name", "efigs.fnt", fontName );
		GuiSys->Init( this->app, *SoundEffectPlayer, fontName.ToCStr(), &app->GetDebugLines() );

		ovr_PlatformInitializeAndroid( APP_ID, java->ActivityObject, java->Env );
		TheAvatarManager.Initialize( APP_ID, java->ActivityObject, java->Env );
	}
	else if ( intentType == INTENT_NEW )
	{
	}
}

void OvrApp::LeavingVrMode()
{
}

bool OvrApp::OnKeyEvent( const int keyCode, const int repeatCount, const KeyEventType eventType )
{
	if ( GuiSys->OnKeyEvent( keyCode, repeatCount, eventType ) )
	{
		return true;
	}
	return false;
}

ovrFrameResult OvrApp::Frame( const ovrFrameInput & vrFrame )
{
	// process input events first because this mirrors the behavior when OnKeyEvent was
	// a virtual function on VrAppInterface and was called by VrAppFramework.
	for ( int i = 0; i < vrFrame.Input.NumKeyEvents; i++ )
	{
		const int keyCode = vrFrame.Input.KeyEvents[i].KeyCode;
		const int repeatCount = vrFrame.Input.KeyEvents[i].RepeatCount;
		const KeyEventType eventType = vrFrame.Input.KeyEvents[i].EventType;

		if ( OnKeyEvent( keyCode, repeatCount, eventType ) )
		{
			continue;   // consumed the event
		}
		// If nothing consumed the key and it's a short-press of the back key, then exit the application to OculusHome.
		if ( keyCode == OVR_KEY_BACK && eventType == KEY_EVENT_SHORT_PRESS )
		{
			app->ShowSystemUI( VRAPI_SYS_UI_CONFIRM_QUIT_MENU );
			continue;
		}
	}

	EnumerateInputDevices();
	UpdateMalibuController( vrFrame );

	// Player movement.
	Scene.Frame( vrFrame, app->GetHeadModelParms() );

	ovrFrameResult res;
	Scene.GetFrameMatrices( vrFrame.FovX, vrFrame.FovY, res.FrameMatrices );
	Scene.GenerateFrameSurfaceList( res.FrameMatrices, res.Surfaces );

	// Clear color
	res.ClearColorBuffer = true;
	res.ClearColor = Vector4f( 0.5f, 0.5f, 0.5f, 0.0f );;

	// Update and render avatars
	TheAvatarManager.Frame( vrFrame, res.FrameMatrices.CenterView );
	TheAvatarManager.AppendSurfaceList( &res.Surfaces );

	// Update GUI systems after the app frame, but before rendering anything.
	GuiSys->Frame( vrFrame, res.FrameMatrices.CenterView );
	// Append GuiSys surfaces.
	GuiSys->AppendSurfaceList( res.FrameMatrices.CenterView, &res.Surfaces );

	FrameParms = vrapi_DefaultFrameParms( app->GetJava(), VRAPI_FRAME_INIT_DEFAULT, vrapi_GetTimeInSeconds(), NULL );

	FrameParms.FrameIndex = vrFrame.FrameNumber;
	FrameParms.MinimumVsyncs = app->GetMinimumVsyncs();
	FrameParms.PerformanceParms = app->GetPerformanceParms();

	ovrFrameLayer & worldLayer = FrameParms.Layers[0];
	for ( int eye = 0; eye < VRAPI_FRAME_LAYER_EYE_MAX; eye++ )
	{
		worldLayer.Textures[eye].ColorTextureSwapChain = vrFrame.ColorTextureSwapChain[eye];
		worldLayer.Textures[eye].DepthTextureSwapChain = vrFrame.DepthTextureSwapChain[eye];
		worldLayer.Textures[eye].TextureSwapChainIndex = vrFrame.TextureSwapChainIndex;

		worldLayer.Textures[eye].TexCoordsFromTanAngles = vrFrame.TexCoordsFromTanAngles;
		worldLayer.Textures[eye].HeadPose = vrFrame.Tracking.HeadPose;
	}

	FrameParms.ExternalVelocity = Scene.GetExternalVelocity();
	worldLayer.Flags = VRAPI_FRAME_LAYER_FLAG_CHROMATIC_ABERRATION_CORRECTION;

	res.FrameParms = (ovrFrameParmsExtBase *) & FrameParms;
	return res;
}

void OvrApp::UpdateMalibuController( const ovrFrameInput & vrFrame )
{
	ovrTracking remoteTracking;
	ovrInputStateTrackedRemote remoteInputState;
	remoteInputState.Header.ControllerType = ovrControllerType_TrackedRemote;

	if( MalibuDeviceID != ovrDeviceIdType_Invalid )
	{
		vrapi_GetInputTrackingState( app->GetOvrMobile(), MalibuDeviceID, vrFrame.PredictedDisplayTimeInSeconds, &remoteTracking );
		vrapi_GetCurrentInputState( app->GetOvrMobile(), MalibuDeviceID, &remoteInputState.Header );
	}

	const bool isActive = MalibuDeviceID != ovrDeviceIdType_Invalid;
	TheAvatarManager.UpdateMalibuInputState( vrFrame.Tracking.HeadPose.Pose, remoteInputState, remoteTracking, RemoteCapabilities, isActive );
}

void OvrApp::OnMalibuConnected()
{
	RemoteCapabilities.Header.Type = ovrControllerType_TrackedRemote;
	RemoteCapabilities.Header.DeviceID = MalibuDeviceID;

	vrapi_GetInputDeviceCapabilities( app->GetOvrMobile(), &RemoteCapabilities.Header );
	vrapi_RecenterInputPose( app->GetOvrMobile(), MalibuDeviceID );
}

void OvrApp::EnumerateInputDevices()
{
	bool foundRemote = false;

	for (uint32_t deviceIndex = 0; ; deviceIndex++)
	{
		ovrInputCapabilityHeader curCaps;

		if( vrapi_EnumerateInputDevices( app->GetOvrMobile(), deviceIndex, &curCaps ) < 0 )
		{
			break;
		}

		switch( curCaps.Type )
		{
		case ovrControllerType_TrackedRemote:
			if( !foundRemote )
			{
				foundRemote = true;
				if( MalibuDeviceID != curCaps.DeviceID )
				{
					MalibuDeviceID = curCaps.DeviceID;
					OnMalibuConnected();
				}
			}
			break;
		case ovrControllerType_Headset:
		default:
			break;
		}
	}

	if( MalibuDeviceID != ovrDeviceIdType_Invalid && !foundRemote )
	{
		MalibuDeviceID = ovrDeviceIdType_Invalid;
	}
}
}

