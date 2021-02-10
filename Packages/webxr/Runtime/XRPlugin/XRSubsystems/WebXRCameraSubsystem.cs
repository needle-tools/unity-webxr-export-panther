using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace WebXR
{
	public class WebXRCameraSubsystem : XRCameraSubsystem
	{
		public const string SubsystemId = "WebXR-CameraSubsystem";

		/// <summary>
		/// Create and register the camera subsystem descriptor to advertise a providing implementation for camera
		/// functionality.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Register()
		{
			Debug.Log("Register " + nameof(WebXRCameraSubsystem));
#if UNITY_WEBGL
			var cameraSubsystemCinfo = new XRCameraSubsystemCinfo
			{
				id = SubsystemId,
#if !UNITY_2019_4
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRCameraSubsystem),
#else
				implementationType = typeof(WebXRCameraSubsystem),
#endif
				supportsAverageBrightness = true,
				supportsAverageColorTemperature = false,
				supportsColorCorrection = true,
				supportsDisplayMatrix = true,
				supportsProjectionMatrix = true,
				supportsTimestamp = true,
				supportsCameraConfigurations = true,
				supportsCameraImage = true,
				supportsAverageIntensityInLumens = false,
				supportsFocusModes = true,
				supportsFaceTrackingAmbientIntensityLightEstimation = true,
				supportsFaceTrackingHDRLightEstimation = false,
				supportsWorldTrackingAmbientIntensityLightEstimation = true,
				supportsWorldTrackingHDRLightEstimation = true,
			};

			if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
			{
				Debug.LogError("Failed registering failed registering " + cameraSubsystemCinfo.id);
			}
#endif
		}

		private class XRProvider : Provider
		{
			/// <summary>
			/// Start the camera functionality.
			/// </summary>
			public override void Start()
			{
			}

			/// <summary>
			/// Stop the camera functionality.
			/// </summary>
			public override void Stop()
			{
			}

			/// <summary>
			/// Destroy any resources required for the camera functionality.
			/// </summary>
			public override void Destroy()
			{
			}

			public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
			{
				return base.TryGetFrame(cameraParams, out cameraFrame);
			}

			public override bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
			{
				return base.TryGetIntrinsics(out cameraIntrinsics);
			}

			public override bool TryAcquireLatestCpuImage(out XRCpuImage.Cinfo cameraImageCinfo)
			{
				return base.TryAcquireLatestCpuImage(out cameraImageCinfo);
			}

			public override Material cameraMaterial { get; }
		}
#if UNITY_2019_4
		protected override Provider CreateProvider()
		{
			return new XRProvider();
		}
#endif
	}
}