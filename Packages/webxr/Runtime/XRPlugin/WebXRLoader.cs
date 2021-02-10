using System.Collections.Generic;
using needle.weaver.webxr;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace WebXR
{
	public class WebXRLoader : XRLoaderHelper
	{
		private static readonly List<WebXRSubsystemDescriptor> subsystemDescriptors = new List<WebXRSubsystemDescriptor>();
		private static readonly List<XRDisplaySubsystemDescriptor> displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
		private static readonly List<XRInputSubsystemDescriptor> inputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
		private static readonly List<XRSessionSubsystemDescriptor> sessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
		private static readonly List<XRCameraSubsystemDescriptor> cameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
		private static readonly List<XRPlaneSubsystemDescriptor> planeSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
		private static readonly List<XRRaycastSubsystemDescriptor> raycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
		private static readonly List<XRImageTrackingSubsystemDescriptor> imageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();

		private WebXRSubsystem WebXRSubsystem => GetLoadedSubsystem<WebXRSubsystem>();
		private XRInputSubsystem XRInputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
		private XRDisplaySubsystem XRDisplaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
		private XRCameraSubsystem XRCameraSubsystem => GetLoadedSubsystem<XRCameraSubsystem>();
		private XRSessionSubsystem XRSessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();
		private XRPlaneSubsystem XRPlaneSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();
		private XRRaycastSubsystem XRRaycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();
		private XRImageTrackingSubsystem XRImageTrackingSubsystem => GetLoadedSubsystem<XRImageTrackingSubsystem>();

		internal static XRDisplaySubsystem DisplaySubsystem { get; private set; }
		internal static XRInputSubsystem InputSubsystem { get; private set; }

		public override bool Initialize()
		{
#if UNITY_INPUT_SYSTEM
			InputSystem.RegisterLayout(typeof(XRHMD));
			InputSystem.RegisterLayout(typeof(XRController));
			InputSystem.RegisterLayout(typeof(OpenVROculusTouchController));
			InputSystem.RegisterLayout(typeof(WebXRHMD));
			InputSystem.RegisterLayout(typeof(WebXRHandheldARInputDevice));
			InputSystem.RegisterLayout(typeof(WebXRHeadsetAndARDeviceCombined));
			InputSystem.RegisterLayout(typeof(WebXRControllerLayout));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			InputSystem.onDeviceChange += (arg, evt) =>
			{
				Debug.Log("Device " + arg + " " + evt);
			};
#endif
#endif

			CreateSubsystem<WebXRSubsystemDescriptor, WebXRSubsystem>(subsystemDescriptors, typeof(WebXRSubsystem).FullName);
			CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(displaySubsystemDescriptors, XRDisplaySubsystem_Patch.Id);
			CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(inputSubsystemDescriptors, XRInputSubsystem_Patch.Id);
			CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(sessionSubsystemDescriptors, WebXRSessionSubsystem.SubsystemId);
			CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(cameraSubsystemDescriptors, WebXRCameraSubsystem.SubsystemId);
			CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(planeSubsystemDescriptors, WebXRPlaneSubsystem.SubsystemId);
			CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(raycastSubsystemDescriptors, WebXRRaycastSubsystem.SubsystemId);
			CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(imageTrackingSubsystemDescriptors,
				WebXRImageTrackingSubsystem.SubsystemId);

			return WebXRSubsystem != null;
		}

		public override bool Start()
		{
			var settings = WebXRSettings.GetSettings();
			if (settings != null)
			{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
				Debug.Log($"Got WebXRSettings");
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
				Native.SetWebXRSettings(settings.ToJson());
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
				Debug.Log($"Sent WebXRSettings");
#endif
			}

			DisplaySubsystem = XRDisplaySubsystem;
			InputSubsystem = XRInputSubsystem;

			XRInputSubsystem.Start();
			WebXRSubsystem.Start();
			XRSessionSubsystem.Start();
			XRCameraSubsystem.Start();
			XRPlaneSubsystem.Start();
			XRRaycastSubsystem.Start();
			XRImageTrackingSubsystem.Start();
			return true;
		}

		public override bool Stop()
		{
			WebXRSubsystem.Stop();
			XRDisplaySubsystem.Stop();
			XRInputSubsystem.Stop();
			XRSessionSubsystem.Stop();
			XRCameraSubsystem.Stop();
			XRPlaneSubsystem.Stop();
			XRRaycastSubsystem.Stop();
			XRImageTrackingSubsystem.Stop();
			return base.Stop();
		}

		public override bool Deinitialize()
		{
			WebXRSubsystem.Destroy();
			XRDisplaySubsystem.Destroy();
			XRInputSubsystem.Destroy();
			XRSessionSubsystem.Destroy();
			XRCameraSubsystem.Destroy();
			XRPlaneSubsystem.Destroy();
			XRRaycastSubsystem.Destroy();
			XRImageTrackingSubsystem.Destroy();
			return base.Deinitialize();
		}
	}
}