using System.Collections.Generic;
using needle.weaver.webxr;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace WebXR
{
	public static class DevicesManager
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
#endif

		private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			CreateDevices();
			TrackedDevicesHelper.ResetTrackedPoseDrivers();
		}

		public static void OnStart()
		{
			CreateDevices();
		}

		public static void OnStop()
		{
			foreach (var dev in devices)
				dev.Disconnect();
		}

		private static void CreateDevices()
		{
			foreach (var dev in devices)
				dev.Disconnect();
			devices.Clear();
			InternalCreateInputDevices();
		}


		private static bool capturedCam;
		private static Color background;
		private static CameraClearFlags clear;

		public static void OnXRStateChanged(WebXRState oldState, WebXRState state)
		{
			switch (state)
			{
				case WebXRState.VR:
					foreach (var dev in devices) dev.Disconnect();
					cameraDevice.Connect();
					controllerLeft.Connect();
					controllerRight.Connect();
					SetCameraState(false);
					XRDisplaySubsystem_Patch.AttachDisplayBehaviour<RenderVR>();
					TrackedDevicesHelper.ResetTrackedPoseDrivers();
					break;
				case WebXRState.NORMAL:
					foreach (var dev in devices) dev.Disconnect();
					SetCameraState(false);
					break;
				case WebXRState.AR:
					foreach (var dev in devices) dev.Disconnect();
					cameraDevice.Connect();
					SetCameraState(true);
					TrackedDevicesHelper.ResetTrackedPoseDrivers();
					break;
			}
		}

		private static void SetCameraState(bool passThrough)
		{
			var main = Camera.main;
			if (!main) main = Object.FindObjectOfType<Camera>();
			if (!main || main == null) return;
			if (passThrough)
			{
				capturedCam = true;
				clear = main.clearFlags;
				background = main.backgroundColor;
				main.clearFlags = CameraClearFlags.Nothing;
				main.backgroundColor = new Color(0, 0, 0, 0);
			}
			else if(capturedCam)
			{
				main.clearFlags = clear;
				main.backgroundColor = background;
			}
		}

		public static void OnUpdatedData()
		{
			switch (WebXRSubsystem.xrState)
			{
				case WebXRState.VR:
					cameraDevice?.UpdateDevice();
					controllerLeft?.UpdateDevice();
					controllerRight?.UpdateDevice();
					break;
				case WebXRState.AR:
					cameraDevice.UpdateDevice();
					break;
				case WebXRState.NORMAL:
					break;
			}
		}

		private static MockInputDevice cameraDevice, controllerLeft, controllerRight;
		private static readonly List<MockInputDevice> devices = new List<MockInputDevice>();

#if UNITY_EDITOR
		[MenuItem("Tools/Hint")]
#endif
		private static string GetHint()
		{
			string hint = null;
#if UNITY_INPUT_SYSTEM
			if (string.IsNullOrEmpty(hint))
			{
				var driver = Object.FindObjectOfType<TrackedPoseDriver>();
				if (driver != null)
				{
					if (driver.positionAction != null)
					{
						foreach (var binding in driver.positionAction.bindings)
						{
							if (binding.path.EndsWith("centerEyePosition"))
								hint = "centerEyePosition";
							else if (binding.path.EndsWith("devicePosition"))
								hint = "devicePosition";
						}
					}

					if (string.IsNullOrEmpty(hint))
					{
						if (driver.rotationAction != null)
						{
							foreach (var binding in driver.rotationAction.bindings)
							{
								if (binding.path.EndsWith("centerEyeRotation"))
									hint = "centerEyeRotation";
								else if (binding.path.EndsWith("deviceRotation"))
									hint = "deviceRotation";
							}
						}
					}
				}
			}
#endif

#if AR_FOUNDATION_INSTALLED
			if (string.IsNullOrEmpty(hint))
			{
				var driver = Object.FindObjectOfType<ARPoseDriver>();
				if (driver != null)
				{
					hint = "devicePosition";
				}
			}
#endif
			return hint;
		}


		private static void InternalCreateInputDevices()
		{
			Debug.Log("Create InputDevices");

			cameraDevice?.Disconnect();
			cameraDevice = null;

			// here we figure out what devices we need to create for the scene
			// e.g. a scene setup with ar pose driver, old input system we 

			bool hasXRRig;
			string cameraDeviceLayoutHint = GetHint();


			Debug.Log(cameraDeviceLayoutHint);

			switch (cameraDeviceLayoutHint)
			{
				case "centerEyePosition":
				case "centerEyeRotation":
					cameraDevice = new MockInputDevice(
						"<XRHMD>",
						InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice,
						XRNode.Head, nameof(XRHMD));
					cameraDevice.AddFeature(CommonUsages.isTracked, () => true);
					cameraDevice.AddFeature(CommonUsages.trackingState, () => InputTrackingState.Position | InputTrackingState.Rotation);
					cameraDevice.AddFeature(CommonUsages.devicePosition, () => WebXRSubsystem.Instance.centerPosition);
					cameraDevice.AddFeature(CommonUsages.deviceRotation, () => WebXRSubsystem.Instance.centerRotation);
					cameraDevice.AddFeature(CommonUsages.centerEyePosition, () => WebXRSubsystem.Instance.centerPosition);
					cameraDevice.AddFeature(CommonUsages.centerEyeRotation, () => WebXRSubsystem.Instance.centerRotation);
					cameraDevice.AddFeature(CommonUsages.leftEyePosition, () => WebXRSubsystem.Instance.leftPosition);
					cameraDevice.AddFeature(CommonUsages.leftEyeRotation, () => WebXRSubsystem.Instance.leftRotation);
					cameraDevice.AddFeature(CommonUsages.rightEyePosition, () => WebXRSubsystem.Instance.rightPosition);
					cameraDevice.AddFeature(CommonUsages.rightEyeRotation, () => WebXRSubsystem.Instance.rightRotation);
					break;

				case "devicePosition":
				case "deviceRotation":
					cameraDevice = new MockInputDevice(
						"WebXR-Device",
						InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice |
						InputDeviceCharacteristics.Camera | InputDeviceCharacteristics.HeadMounted,
						XRNode.Head, nameof(WebXRHandheldARInputDevice));
					cameraDevice.AddFeature(CommonUsages.devicePosition, () => WebXRSubsystem.Instance.centerPosition);
					cameraDevice.AddFeature(CommonUsages.deviceRotation, () => WebXRSubsystem.Instance.centerRotation);
					cameraDevice.AddFeature(CommonUsages.centerEyePosition, () => WebXRSubsystem.Instance.centerPosition);
					cameraDevice.AddFeature(CommonUsages.centerEyeRotation, () => WebXRSubsystem.Instance.centerRotation);
					break;
			}

			Debug.Log("Created camera device " + cameraDevice.Name + " - " + cameraDevice.Layout);

			// if (cameraDevice == null)
			// {
			// 	cameraDevice = MockDeviceBuilder.CreateHeadset(
			// 		() => true,
			// 		() => WebXRSubsystem.Instance.centerPosition,
			// 		() => WebXRSubsystem.Instance.centerRotation,
			// 		null,
			// 		() => WebXRSubsystem.Instance.leftPosition,
			// 		() => WebXRSubsystem.Instance.leftRotation,
			// 		() => WebXRSubsystem.Instance.rightPosition,
			// 		() => WebXRSubsystem.Instance.rightRotation
			// 	);
			// 	devices.Add(cameraDevice);
			// }

			if (controllerRight == null)
			{
				controllerRight = CreateController(XRNode.RightHand, WebXRSubsystem.Instance.controller1, InputDeviceCharacteristics.Right);
			}

			if (controllerLeft == null)
			{
				controllerLeft = CreateController(XRNode.LeftHand, WebXRSubsystem.Instance.controller2, InputDeviceCharacteristics.Left);
			}
		}

		private static MockInputDevice CreateController(XRNode node, WebXRControllerData controller, InputDeviceCharacteristics side)
		{
			var device = new MockInputDevice("<XRController>", InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | side, node,
				nameof(WebXRControllerLayout))
			{
			};
			device.AddFeature(CommonUsages.isTracked, () => true);
			device.AddFeature(CommonUsages.trackingState, () => InputTrackingState.Position | InputTrackingState.Rotation);
			device.AddFeature(CommonUsages.devicePosition, () => controller?.position ?? Vector3.zero);
			device.AddFeature(CommonUsages.deviceRotation, () => controller?.rotation ?? Quaternion.identity);
			device.AddFeature(CommonUsages.trigger, () => controller?.trigger ?? 0);
			device.AddFeature(CommonUsages.triggerButton, () => controller?.trigger > .5f);
			device.AddFeature(CommonUsages.grip, () => controller?.squeeze ?? 0);
			device.AddFeature(CommonUsages.gripButton, () => controller?.squeeze > .5f);
			// TODO: not sure about the touchpads / primary or secondary axis inputs
			device.AddFeature(CommonUsages.primary2DAxisTouch, () => controller.touchpad > .5f);
			device.AddFeature(CommonUsages.primary2DAxis,
				() => controller != null ? new Vector2(controller.thumbstickX, controller.thumbstickY) : Vector2.zero);
			device.AddFeature(CommonUsages.primary2DAxisClick, () => controller?.touchpad > .5f);
			device.AddFeature(CommonUsages.secondary2DAxisTouch, () => controller.touchpad > .5f);
			device.AddFeature(CommonUsages.secondary2DAxis, () => controller != null ? new Vector2(controller.touchpadX, controller.touchpadY) : Vector2.zero);
			device.AddFeature(CommonUsages.secondary2DAxisClick, () => controller.touchpad > .5f);
			device.AddFeature(CommonUsages.primaryButton, () => controller?.buttonA > .5f);
			device.AddFeature(CommonUsages.secondaryButton, () => controller?.buttonB > .5f);
			// openvr 
			device.AddFeature(new InputFeatureUsage<float>("thumbstickClicked"), () => controller?.thumbstick ?? 0);
			devices.Add(device);
			return device;
		}
	}
}