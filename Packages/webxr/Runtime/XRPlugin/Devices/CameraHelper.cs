using UnityEngine;

namespace WebXR
{
	internal static class CameraHelper
	{
		private static Camera _camera;
		private static bool capturedCam;
		private static Color background;
		private static CameraClearFlags clear = (CameraClearFlags) (-1);

		private static Camera Camera
		{
			get
			{
				if (_camera) return _camera;
				_camera = Camera.main;
				if (!_camera) _camera = Object.FindObjectOfType<Camera>();
				if (!_camera || _camera == null) return null;

				return _camera;
			}
		}

		public static void SetCameraClearFlags(bool setToPassThrough)
		{
			if (!Camera) return;

			if (setToPassThrough)
			{
				capturedCam = true;
				clear = Camera.clearFlags;
				background = Camera.backgroundColor;
				Camera.clearFlags = CameraClearFlags.Nothing;
				Camera.backgroundColor = new Color(0, 0, 0, 0);
			}
			else if (capturedCam)
			{
				Camera.clearFlags = clear;
				Camera.backgroundColor = background;
			}
		}

		public static void FixRotationAfterXR()
		{
			if (!Camera) return;
			var t = Camera.transform;
			var rot = t.rotation.eulerAngles;
			rot.z = 0;
			t.rotation = Quaternion.Euler(rot);
		}
	}
}