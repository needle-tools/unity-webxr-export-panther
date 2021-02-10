using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace WebXR
{
	[Preserve]
	public class WebXRPlaneSubsystem : XRPlaneSubsystem
	{
		public const string SubsystemId = "WebXR-PlaneSubsystem";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RegisterDescriptor()
		{
#if UNITY_WEBGL
			var info = new XRPlaneSubsystemDescriptor.Cinfo
			{
				id = SubsystemId,
				providerType = typeof(XRProvider),
				subsystemTypeOverride = typeof(WebXRPlaneSubsystem),
				supportsHorizontalPlaneDetection = true,
				supportsVerticalPlaneDetection = true,
				supportsArbitraryPlaneDetection = false,
				supportsBoundaryVertices = true
			};

			XRPlaneSubsystemDescriptor.Create(info);
#endif
		}

		private class XRProvider : Provider
		{
			public override void Start()
			{
				
			}

			public override void Stop()
			{
			}

			public override void Destroy()
			{
			}

			public override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
			{
				return new TrackableChanges<BoundedPlane>();
			}

			public override void GetBoundary(TrackableId trackableId, Allocator allocator, ref NativeArray<Vector2> boundary)
			{
			}


			public override PlaneDetectionMode currentPlaneDetectionMode { get; }
			public override PlaneDetectionMode requestedPlaneDetectionMode { get; set; }
		}
	}
}