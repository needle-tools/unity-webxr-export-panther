using System;
using System.Runtime.InteropServices;

namespace WebXR
{
	internal static class Native
	{
	    [DllImport("__Internal")]
	    public static extern void SetWebXRSettings(string settingsJson);
		
		[DllImport("__Internal")]
		public static extern void InitXRSharedArray(float[] array, int length);

		[DllImport("__Internal")]
		public static extern void InitControllersArray(float[] array, int length);

		[DllImport("__Internal")]
		public static extern void InitHandsArray(float[] array, int length);

		[DllImport("__Internal")]
		public static extern void InitViewerHitTestPoseArray(float[] array, int length);

		[DllImport("__Internal")]
		public static extern void ToggleViewerHitTest();

		[DllImport("__Internal")]
		public static extern void ControllerPulse(int controller, float intensity, float duration);

		[DllImport("__Internal")]
		public static extern void ListenWebXRData();

		[DllImport("__Internal")]
		public static extern void set_webxr_events(Action<int, float, float, float, float, float, float, float, float> on_start_ar,
			Action<int, float, float, float, float, float, float, float, float> on_start_vr,
			Action on_end_xr,
			Action<string> on_xr_capabilities,
			Action<string> on_input_profiles);
	}
}