using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WebXR
{
	public static class TrackedDevicesHelper
	{
		/// <summary>
		/// e.g. ARPoseDriver does only search for tracked device in on enable.
		/// When switching xr state we also might use another mock device for tracking and thus need
		/// to ensure tracked pose drivers are also being reset.
		/// We want them to search for the device again
		/// </summary>
		public static void ResetTrackedPoseDrivers()
		{
			// TODO: optimize to not search objects every time we switch context

// #if UNITY_INPUT_SYSTEM
// 			Object.FindObjectsOfType<UnityEngine.InputSystem.XR.TrackedPoseDriver>().ResetComponents();
// #endif

// #if LEGACY_INPUT_HELPERS_INSTALLED
			// reset in old TrackedPoseDriver is not necessary because it just loops XRNodes and doesnt cache devices
// 			Object.FindObjectsOfType<UnityEngine.SpatialTracking.TrackedPoseDriver>().ResetComponents();
// #endif

#if AR_FOUNDATION_INSTALLED
			Object.FindObjectsOfType<UnityEngine.XR.ARFoundation.ARPoseDriver>().ResetComponents(ResetARPoseDriverAssignedDeviceInstance);
#endif
		}

		private static void ResetComponents(this IEnumerable<Behaviour> beh, Action<Behaviour> beforeEnable = null)
		{
			if (beh == null) return;
			foreach (var b in beh)
			{
				ResetComponent(b, beforeEnable);
			}
		}

		private static void ResetComponent(Behaviour beh, Action<Behaviour> beforeEnable = null)
		{
			if (!beh) return;
			if (!beh.enabled) return;
			beh.enabled = false;
			beforeEnable?.Invoke(beh);
			// ReSharper disable once Unity.InefficientPropertyAccess
			beh.enabled = true;
		}

		private static FieldInfo arPoseDriverDeviceField;

		private static void ResetARPoseDriverAssignedDeviceInstance(object obj)
		{
			if (arPoseDriverDeviceField == null)
			{
				arPoseDriverDeviceField = obj?.GetType().GetField("s_InputTrackingDevice", (BindingFlags) ~0);
				if (arPoseDriverDeviceField == null)
				{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
					Debug.LogWarning("Could not find input device field in ar pose driver: " + obj, obj as Object);
#endif
					return;
				}
			}
			// its a static field so obj can be null
			arPoseDriverDeviceField.SetValue(null, null);
		}
	}
}