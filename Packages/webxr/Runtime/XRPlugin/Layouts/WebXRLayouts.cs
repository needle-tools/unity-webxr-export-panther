#if UNITY_INPUT_SYSTEM

using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;


namespace WebXR
{
		[Preserve]
		[InputControlLayout(displayName = "WebXR Controller", commonUsages = new[] {"LeftHand", "RightHand"})]
		public class WebXRControllerLayout : XRController
		{
			[Preserve]
			[InputControl(aliases = new[] {"Primary2DAxis", "Joystick"})]
			public Vector2Control thumbstick { get; private set; }

			[Preserve] [InputControl] public AxisControl trigger { get; private set; }
			// [Preserve] [InputControl] public AxisControl grip { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"A", "X", "Alternate"})]
			public ButtonControl primaryButton { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"B", "Y", "Primary"})]
			public ButtonControl secondaryButton { get; private set; }

			// [Preserve]
			// [InputControl(aliases = new[] {"GripButton"})]
			// public ButtonControl gripPressed { get; private set; }

			// [Preserve] [InputControl] public ButtonControl start { get; private set; }

			[Preserve]
			[InputControl(aliases = new[] {"JoystickOrPadPressed", "thumbstickClick"})]
			public ButtonControl thumbstickClicked { get; private set; }

			// [Preserve]
			// [InputControl(aliases = new[] {"ATouched", "XTouched", "ATouch", "XTouch"})]
			// public ButtonControl primaryTouched { get; private set; }
			//
			// [Preserve]
			// [InputControl(aliases = new[] {"BTouched", "YTouched", "BTouch", "YTouch"})]
			// public ButtonControl secondaryTouched { get; private set; }

			// [Preserve]
			// [InputControl(aliases = new[] {"indexTouch", "indexNearTouched"})]
			// public AxisControl triggerTouched { get; private set; }

			// [Preserve]
			// [InputControl(aliases = new[] {"indexButton", "indexTouched"})]
			// public ButtonControl triggerPressed { get; private set; }
			
			protected override void FinishSetup()
			{
				base.FinishSetup();

				thumbstick = GetChildControl<Vector2Control>("thumbstick");
				trigger = GetChildControl<AxisControl>("trigger");
				// triggerTouched = GetChildControl<AxisControl>("triggerTouched");
				// grip = GetChildControl<AxisControl>("grip");

				primaryButton = GetChildControl<ButtonControl>("primaryButton");
				secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
				// gripPressed = GetChildControl<ButtonControl>("gripPressed");
				// start = GetChildControl<ButtonControl>("start");
				thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
				// primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
				// secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
				// triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			}
		}
}

#endif