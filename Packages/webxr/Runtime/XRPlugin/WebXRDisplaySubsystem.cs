using needle.weaver.webxr;
using UnityEngine;

namespace WebXR
{
	public class WebXRDisplaySubsystemDescriptor : SubsystemDescriptor<WebXRDisplaySubsystem>
	{
	}

	public class WebXRDisplaySubsystem : Subsystem<WebXRDisplaySubsystemDescriptor>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RegisterDescriptor()
		{
			var res = SubsystemRegistration.CreateDescriptor(new WebXRDisplaySubsystemDescriptor()
			{
				id = typeof(WebXRDisplaySubsystem).FullName,
				subsystemImplementationType = typeof(WebXRDisplaySubsystem)
			});
			#if DEVELOPMENT_BUILD
			if (res) Debug.Log("Registered " + nameof(WebXRDisplaySubsystem));
			else Debug.LogError("Failed registering " + nameof(WebXRDisplaySubsystem));
			#endif
		}

		public override void Start()
		{
			_running = true;
			XRDisplaySubsystem_Patch.Instance.Start();
		}

		public override void Stop()
		{
			_running = false;
			XRDisplaySubsystem_Patch.Instance.Stop();
		}

		protected override void OnDestroy()
		{
			XRDisplaySubsystem_Patch.Instance.Destroy();
		}

		private bool _running;
		public override bool running => _running;

		internal void OnWebXRModeChanged()
		{
			
		}
	}
}