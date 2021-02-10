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
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRSessionSubsystem),
				supportsInstall = true,
				supportsMatchFrameRate = true,
			});
#endif
		}

		private class XRProvider : Provider
		{
		}
	}
}