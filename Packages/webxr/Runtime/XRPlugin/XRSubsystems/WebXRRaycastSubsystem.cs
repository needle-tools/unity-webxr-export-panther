using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace WebXR
{
	public class WebXRRaycastSubsystem : XRRaycastSubsystem
	{
		public const string SubsystemId = "WebXR-RaycastSubsystem";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RegisterDescriptor()
		{
#if UNITY_WEBGL
			XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
			{
				id = SubsystemId,
#if !UNITY_2019_4
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRRaycastSubsystem),
#endif
				supportsViewportBasedRaycast = true,
				supportsWorldBasedRaycast = true,
				supportedTrackableTypes = (TrackableType.Planes & ~TrackableType.PlaneWithinInfinity) | TrackableType.FeaturePoint
			});
#endif
		}


		private class XRProvider : Provider
		{
			public override NativeArray<XRRaycastHit> Raycast(XRRaycastHit defaultRaycastHit, Ray ray, TrackableType trackableTypeMask, Allocator allocator)
			{
				return base.Raycast(defaultRaycastHit, ray, trackableTypeMask, allocator);
			}

			public override NativeArray<XRRaycastHit> Raycast(XRRaycastHit defaultRaycastHit, Vector2 screenPoint, TrackableType trackableTypeMask,
				Allocator allocator)
			{
				return base.Raycast(defaultRaycastHit, screenPoint, trackableTypeMask, allocator);
			}
		}

		#if UNITY_2019_4
		protected override Provider CreateProvider()
		{
			return new XRProvider();
		}
		#endif
	}
}