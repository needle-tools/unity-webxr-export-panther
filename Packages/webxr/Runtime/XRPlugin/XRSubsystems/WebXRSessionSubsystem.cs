using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace WebXR
{
	public class WebXRSessionSubsystem : XRSessionSubsystem
	{
		public const string SubsystemId = "WebXR-SessionSubsystem";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Register()
		{
#if UNITY_WEBGL
			XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
			{
				id = SubsystemId,
#if !UNITY_2019_4
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRSessionSubsystem),
#endif
				supportsInstall = true,
				supportsMatchFrameRate = true,
			});
#endif
		}

		private class XRProvider : Provider
		{
			// TODO: implement with backend :)

			public override Feature requestedFeatures { get; }
			public override Feature requestedTrackingMode { get; set; }
			public override TrackingState trackingState { get; }
			public override NotTrackingReason notTrackingReason { get; }
			public override bool matchFrameRateEnabled { get; }
			public override bool matchFrameRateRequested { get; set; }
			public override int frameRate { get; }
			public override Feature currentTrackingMode { get; }
			public override IntPtr nativePtr { get; }
		}
#if UNITY_2019_4
		protected override Provider CreateProvider()
		{
			return new XRProvider();
		}
#endif
	}
}