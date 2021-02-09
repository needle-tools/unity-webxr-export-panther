using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using WebXR;

namespace Demo
{
	public class SimpleCameraController : MonoBehaviour
	{
		public Vector2 RotationFactor = new Vector2(30, 10);
		public float SpeedFactor = 2;
		public Behaviour PoseDriver;
		public Transform Root;

		private Vector2 _delta;

		private void OnValidate()
		{
			if (!Root) Root = GetComponentInParent<XRRig>()?.transform;
		}

		private void Update()
		{
			if (WebXRSubsystem.xrState != WebXRState.NORMAL)
			{
				_delta = Vector2.zero;
				PoseDriver.enabled = true;
				return;
			}

			PoseDriver.enabled = false;
			var mouse = Mouse.current;
			if (mouse != null && (mouse.rightButton.isPressed || mouse.leftButton.isPressed))
			{
				var delta = mouse.delta.ReadValue();
				if (delta.magnitude > 0)
					_delta = Vector2.Lerp(_delta, delta, Time.deltaTime / .5f);
			}

			if (Touchscreen.current != null)
			{
				var delta = Touchscreen.current.primaryTouch?.delta.ReadValue() ?? Vector2.zero;
				if (delta.magnitude > 0)
					_delta = Vector2.Lerp(_delta, delta, Time.deltaTime / .5f);
			}

			Rotate(_delta);
			_delta *= (1 - Time.deltaTime * 9f);

			var kb = Keyboard.current;
			if (kb != null)
			{
				var movement = new Vector3();
				if (kb.wKey.isPressed) movement.z += 1;
				if (kb.sKey.isPressed) movement.z -= 1;
				if (kb.dKey.isPressed) movement.x += 1;
				if (kb.aKey.isPressed) movement.x -= 1;
				if (kb.eKey.isPressed) movement.y += 1;
				if (kb.qKey.isPressed) movement.y -= 1;

				var rot = transform.localRotation.eulerAngles;
				movement = Quaternion.Euler(0, rot.y, 0) * movement.normalized;
				Root.position += movement * (Time.deltaTime * SpeedFactor);
			}
		}

		private void Rotate(Vector2 delta)
		{
			var screenFactor = new Vector2(Screen.width, Screen.height) / Mathf.Max(Screen.dpi, 96);
			delta *= Mathf.Min(screenFactor.x, screenFactor.y);
			delta.x *= 2;
			delta = Vector2.ClampMagnitude(delta, 5);
			delta *= RotationFactor;
			// delta 
			// delta *= 100;
			var rotation = transform.localRotation.eulerAngles;
			rotation += new Vector3(delta.y, -delta.x, 0) * (Time.deltaTime);
			transform.localRotation = Quaternion.Euler(rotation);
		}
	}
}