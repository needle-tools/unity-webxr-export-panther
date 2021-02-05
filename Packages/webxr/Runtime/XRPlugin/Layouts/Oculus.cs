#if UNITY_INPUT_SYSTEM

using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;


namespace WebXR.Layouts
{
	public static class Oculus
	{
		/// <summary>
		/// An Oculus Touch controller.
		/// </summary>
		[Preserve]
		[InputControlLayout(displayName = "Oculus Touch Controller", commonUsages = new[] {"LeftHand", "RightHand"})]
		public class OculusTouchController : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(aliases = new[] {"Primary2DAxis", "Joystick"})]
			public Vector2Control thumbstick { get; private set; }

			[Preserve] [InputControl] public AxisControl trigger { get; private set; }
			[Preserve] [InputControl] public AxisControl grip { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"A", "X", "Alternate"})]
			public ButtonControl primaryButton { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"B", "Y", "Primary"})]
			public ButtonControl secondaryButton { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"GripButton"})]
			public ButtonControl gripPressed { get; private set; }

			[Preserve] [InputControl] public ButtonControl start { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"JoystickOrPadPressed", "thumbstickClick"})]
			public ButtonControl thumbstickClicked { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"ATouched", "XTouched", "ATouch", "XTouch"})]
			public ButtonControl primaryTouched { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"BTouched", "YTouched", "BTouch", "YTouch"})]
			public ButtonControl secondaryTouched { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"indexTouch", "indexNearTouched"})]
			public AxisControl triggerTouched { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"indexButton", "indexTouched"})]
			public ButtonControl triggerPressed { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"JoystickOrPadTouched", "thumbstickTouch"})]
			[InputControl(name = "trackingState", layout = "Integer", aliases = new[] {"controllerTrackingState"})]
			[InputControl(name = "isTracked", layout = "Button", aliases = new[] {"ControllerIsTracked"})]
			[InputControl(name = "devicePosition", layout = "Vector3", aliases = new[] {"controllerPosition"})]
			[InputControl(name = "deviceRotation", layout = "Quaternion", aliases = new[] {"controllerRotation"})]
			public ButtonControl thumbstickTouched { get; private set; }

			[Preserve]
			[InputControl(noisy = true, aliases = new[] {"controllerVelocity"})]
			public Vector3Control deviceVelocity { get; private set; }

			[Preserve]
			[InputControl(noisy = true, aliases = new[] {"controllerAngularVelocity"})]
			public Vector3Control deviceAngularVelocity { get; private set; }

			[Preserve]
			[InputControl(noisy = true, aliases = new[] {"controllerAcceleration"})]
			public Vector3Control deviceAcceleration { get; private set; }

			[Preserve]
			[InputControl(noisy = true, aliases = new[] {"controllerAngularAcceleration"})]
			public Vector3Control deviceAngularAcceleration { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();

				thumbstick = GetChildControl<Vector2Control>("thumbstick");
				trigger = GetChildControl<AxisControl>("trigger");
				triggerTouched = GetChildControl<AxisControl>("triggerTouched");
				grip = GetChildControl<AxisControl>("grip");

				primaryButton = GetChildControl<ButtonControl>("primaryButton");
				secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
				gripPressed = GetChildControl<ButtonControl>("gripPressed");
				start = GetChildControl<ButtonControl>("start");
				thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
				primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
				secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
				thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
				triggerPressed = GetChildControl<ButtonControl>("triggerPressed");

				deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
				deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
				deviceAcceleration = GetChildControl<Vector3Control>("deviceAcceleration");
				deviceAngularAcceleration = GetChildControl<Vector3Control>("deviceAngularAcceleration");
			}
		}
	}
}

#endif