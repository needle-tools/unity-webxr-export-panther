using System;
using System.Threading.Tasks;
using AOT;
using needle.weaver.webxr;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.XR;
using Utils;

namespace WebXR
{
  public class WebXRSubsystemDescriptor : SubsystemDescriptor<WebXRSubsystem>
  {
  }

  public class WebXRSubsystem : Subsystem<WebXRSubsystemDescriptor>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterDescriptor()
    {
      var res = SubsystemRegistration.CreateDescriptor(new WebXRSubsystemDescriptor()
      {
        id = typeof(WebXRSubsystem).FullName,
        subsystemImplementationType = typeof(WebXRSubsystem)
      });
      #if DEVELOPMENT_BUILD
      if (res) Debug.Log("Registered " + nameof(WebXRSubsystemDescriptor));
      else Debug.LogError("Failed registering " + nameof(WebXRSubsystemDescriptor));
      #endif
    }


    public override void Start()
    {
      if (running) return;
      Debug.Log("Start " + nameof(WebXRSubsystem));
      _running = true;
      Instance = this;
      
      Native.set_webxr_events(OnStartAR, OnStartVR, OnEndXR, OnXRCapabilities, OnInputProfiles);
      Native.InitControllersArray(controllersArray, controllersArray.Length);
      Native.InitHandsArray(handsArray, handsArray.Length);
      Native.InitViewerHitTestPoseArray(viewerHitTestPoseArray, viewerHitTestPoseArray.Length);
      Native.InitXRSharedArray(sharedArray, sharedArray.Length);
      Native.ListenWebXRData();
      
      CreateInputDevices();
      headset.Connect();
      controllerLeft.Connect();
      controllerRight.Connect();
      
      PlayerLoopHelper.AddUpdateCallback(this.GetType(), this.OnUpdate, PlayerLoopHelper.Stages.EarlyUpdate);
    }

    public override void Stop()
    {
      if (!_running) return;
      Debug.Log("Stop " + nameof(WebXRSubsystem));
      _running = false;
      Instance = null;
      headset.Disconnect();
      controllerLeft.Disconnect();
      controllerRight.Disconnect();
      PlayerLoopHelper.RemoveUpdateDelegate(this.GetType(), this.OnUpdate);
    }

    protected override void OnDestroy()
    {
      if (!running) return;
      Debug.Log("Destroy " + nameof(WebXRSubsystem));
      _running = false;
      Instance = null;
    }

    private PlayerLoopSystem update;
    private MockInputDevice headset, controllerLeft, controllerRight;

    private void CreateInputDevices()
    {
      if (headset == null)
      {
        headset = MockDeviceBuilder.CreateHeadset(
          () => true,
          () => centerPosition,
          () => centerRotation,
          null,
          () => leftPosition,
          () => leftRotation,
          () => rightPosition,
          () => rightRotation
        );
      }
      if(controllerRight == null) 
        controllerRight = CreateController(XRNode.RightHand, controller1, InputDeviceCharacteristics.Right);
      if (controllerLeft == null)
        controllerLeft = CreateController(XRNode.LeftHand, controller2, InputDeviceCharacteristics.Left);

    }
    
    private static MockInputDevice CreateController(XRNode node, WebXRControllerData controller, InputDeviceCharacteristics side)
    {
      var device = new MockInputDevice("<XRController>", node)
      {
        SerialNumber = "1.0.0",
        Manufacturer = "Needle",
        DeviceCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | side
      };
      device.AddFeature(CommonUsages.isTracked, () => controller?.enabled ?? false);
      device.AddFeature(CommonUsages.trackingState, () => InputTrackingState.Position | InputTrackingState.Rotation);
      device.AddFeature(CommonUsages.devicePosition, () => controller?.position ?? Vector3.zero);
      device.AddFeature(CommonUsages.deviceRotation, () => controller?.rotation * Quaternion.Euler(90,0,0) ?? Quaternion.identity); 
      device.AddFeature(CommonUsages.trigger, () => controller?.trigger ?? 0);
      device.AddFeature(CommonUsages.grip, () => controller?.squeeze ?? 0);
      device.AddFeature(CommonUsages.secondary2DAxis, () => controller != null ? new Vector2(controller.touchpadX, controller.touchpadY) : Vector2.zero);
      device.AddFeature(CommonUsages.primary2DAxis, () => controller != null ? new Vector2(controller.thumbstickX, controller.thumbstickY) : Vector2.zero);
      device.AddFeature(CommonUsages.primary2DAxisClick, () => controller?.thumbstick > .5f);
      device.AddFeature(CommonUsages.primaryButton, () => controller?.buttonA > .5f);
      device.AddFeature(CommonUsages.secondaryButton, () => controller?.buttonB > .5f);
      return device;
    }

    private void UpdateControllersOnEnd()
    {
      if (OnHandUpdate != null)
      {
        if (GetHandFromHandsArray(0, ref leftHand))
        {
          OnHandUpdate?.Invoke(leftHand);
        }

        if (GetHandFromHandsArray(1, ref rightHand))
        {
          OnHandUpdate?.Invoke(rightHand);
        }
      }

      if (OnControllerUpdate != null)
      {
        if (GetGamepadFromControllersArray(0, ref controller1))
        {
          OnControllerUpdate?.Invoke(controller1);
        }

        if (GetGamepadFromControllersArray(1, ref controller2))
        {
          OnControllerUpdate?.Invoke(controller2);
        }
      }
    }

    private void OnUpdate()
    {
      if (switchToEnd)
      {
        switchToEnd = false;
        UpdateControllersOnEnd();
      }
      if (this.xrState == WebXRState.NORMAL) return;
      
      UpdateXRCameras();
      var hasHandsData = false;
      if (this.xrState != WebXRState.NORMAL)
      {
        if (GetHandFromHandsArray(0, ref leftHand)) OnHandUpdate?.Invoke(leftHand);
        if (GetHandFromHandsArray(1, ref rightHand)) OnHandUpdate?.Invoke(rightHand);
        hasHandsData = leftHand.enabled || rightHand.enabled;
      }

      if (!hasHandsData && this.xrState != WebXRState.NORMAL)
      {
        if (GetGamepadFromControllersArray(0, ref controller1)) OnControllerUpdate?.Invoke(controller1);
        if (GetGamepadFromControllersArray(1, ref controller2)) OnControllerUpdate?.Invoke(controller2);
      }

      if (OnViewerHitTestUpdate != null && this.xrState == WebXRState.AR)
      {
        if (GetHitTestPoseFromViewerHitTestPoseArray(ref viewerHitTestPose)) OnViewerHitTestUpdate?.Invoke(viewerHitTestPose);
      }
    }

    private void UpdateXRCameras()
    {
      if (this.xrState != WebXRState.NORMAL)
      {
        GetMatrixFromSharedArray(0, ref leftProjectionMatrix);
        GetMatrixFromSharedArray(16, ref rightProjectionMatrix);
        GetQuaternionFromSharedArray(32, ref leftRotation);
        GetQuaternionFromSharedArray(36, ref rightRotation);
        GetVector3FromSharedArray(40, ref leftPosition);
        GetVector3FromSharedArray(43, ref rightPosition);

        centerPosition = Vector3.Lerp(leftPosition, rightPosition, .5f);
        centerRotation = Quaternion.Lerp(leftRotation, rightRotation, .5f);

        XRDisplaySubsystem_Patch.Instance.ProjectionLeft = leftProjectionMatrix;
        XRDisplaySubsystem_Patch.Instance.ProjectionRight = rightProjectionMatrix;
        
        OnHeadsetUpdate?.Invoke(
            leftProjectionMatrix,
            rightProjectionMatrix,
            leftRotation,
            rightRotation,
            leftPosition,
            rightPosition);
      }
    }

    private bool _running;
    public override bool running => _running;

    private static WebXRSubsystem Instance;

    internal WebXRState xrState = WebXRState.NORMAL;

    public delegate void XRCapabilitiesUpdate(WebXRDisplayCapabilities capabilities);
    public static event XRCapabilitiesUpdate OnXRCapabilitiesUpdate;

    public delegate void XRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect);
    public static event XRChange OnXRChange;

    public delegate void HeadsetUpdate(
        Matrix4x4 leftProjectionMatrix,
        Matrix4x4 rightProjectionMatrix,
        Quaternion leftRotation,
        Quaternion rightRotation,
        Vector3 leftPosition,
        Vector3 rightPosition);
    public static event HeadsetUpdate OnHeadsetUpdate;

    public delegate void ControllerUpdate(WebXRControllerData controllerData);
    public static event ControllerUpdate OnControllerUpdate;

    public delegate void HandUpdate(WebXRHandData handData);
    public static event HandUpdate OnHandUpdate;

    public delegate void HitTestUpdate(WebXRHitPoseData hitPoseData);
    public static event HitTestUpdate OnViewerHitTestUpdate;

    // Cameras calculations helpers
    private Matrix4x4 leftProjectionMatrix, rightProjectionMatrix;
    private Vector3 centerPosition, leftPosition, rightPosition;
    private Quaternion centerRotation, leftRotation, rightRotation;

    // Shared array which we will load headset data in from webxr.jslib
    // Array stores 2 matrices, each 16 values, 2 Quaternions and 2 Vector3, stored linearly.
    private readonly float[] sharedArray = new float[(2 * 16) + (2 * 7)];

    // Shared array for controllers data
    private readonly float[] controllersArray = new float[2 * 20];

    // Shared array for hands data
    private readonly float[] handsArray = new float[2 * (25 * 9 + 5)];

    // Shared array for hit-test pose data
    private readonly float[] viewerHitTestPoseArray = new float[9];

    private bool viewerHitTestOn = false;

    private bool switchToEnd = false;

    private WebXRHandData leftHand = new WebXRHandData();
    private WebXRHandData rightHand = new WebXRHandData();
    private WebXRControllerData controller1 = new WebXRControllerData();
    private WebXRControllerData controller2 = new WebXRControllerData();
    private WebXRHitPoseData viewerHitTestPose = new WebXRHitPoseData();
    private WebXRDisplayCapabilities capabilities = new WebXRDisplayCapabilities();

    // Handles WebXR capabilities from browser
    [MonoPInvokeCallback(typeof(Action<string>))]
    public static void OnXRCapabilities(string json)
    {
      WebXRDisplayCapabilities capabilities = JsonUtility.FromJson<WebXRDisplayCapabilities>(json);
      Instance.OnXRCapabilities(capabilities);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    public static void OnInputProfiles(string json)
    {
      WebXRControllersProfiles controllersProfiles = JsonUtility.FromJson<WebXRControllersProfiles>(json);
      Instance.OnInputProfiles(controllersProfiles);
    }

    private void OnXRCapabilities(WebXRDisplayCapabilities cap)
    {
      this.capabilities = cap;
      OnXRCapabilitiesUpdate?.Invoke(cap);
    }

    private void OnInputProfiles(WebXRControllersProfiles controllersProfiles)
    {
      controller1.profiles = controllersProfiles.conrtoller1;
      controller2.profiles = controllersProfiles.conrtoller2;
    }

    private void setXrState(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
    {
      this.xrState = state;
      viewerHitTestOn = false;
      OnXRChange?.Invoke(state, viewsCount, leftRect, rightRect);

      switch (this.xrState)
      {
        case WebXRState.VR:
          XRDisplaySubsystem_Patch.AttachDisplayBehaviour<RenderVR>();
          WebXRLoader.DisplaySubsystem.Start();
          WebXRLoader.InputSubsystem.Start();
          headset.Connect();
          break;
        case WebXRState.NORMAL:
          headset.Disconnect();
          WebXRLoader.InputSubsystem.Stop();
          WebXRLoader.DisplaySubsystem.Stop();
          break;
      }
    }

    // received start AR from WebXR browser
    [MonoPInvokeCallback(typeof(Action<int, float, float, float, float, float, float, float, float>))]
    public static void OnStartAR(int viewsCount,
        float left_x, float left_y, float left_w, float left_h,
        float right_x, float right_y, float right_w, float right_h)
    {
      Instance.setXrState(WebXRState.AR, viewsCount,
          new Rect(left_x, left_y, left_w, left_h),
          new Rect(right_x, right_y, right_w, right_h));
    }

    // received start VR from WebXR browser
    [MonoPInvokeCallback(typeof(Action<int, float, float, float, float, float, float, float, float>))]
    public static void OnStartVR(int viewsCount,
        float left_x, float left_y, float left_w, float left_h,
        float right_x, float right_y, float right_w, float right_h)
    {
      Instance.setXrState(WebXRState.VR, viewsCount,
          new Rect(left_x, left_y, left_w, left_h),
          new Rect(right_x, right_y, right_w, right_h));
    }

    // receive end VR from WebVR browser
    [MonoPInvokeCallback(typeof(Action))]
    public static void OnEndXR()
    {
      Instance.switchToEnd = true;
      Instance.setXrState(WebXRState.NORMAL, 1, new Rect(), new Rect());
    }

    public void StartViewerHitTest()
    {
      if (xrState == WebXRState.AR && !viewerHitTestOn)
      {
        viewerHitTestOn = true;
        Native.ToggleViewerHitTest();
      }
    }

    public void StopViewerHitTest()
    {
      if (xrState == WebXRState.AR && viewerHitTestOn)
      {
        viewerHitTestOn = false;
        Native.ToggleViewerHitTest();
      }
    }

    public void HapticPulse(WebXRControllerHand hand, float intensity, float duration)
    {
      Native.ControllerPulse((int)hand, intensity, duration);
    }

    private void GetMatrixFromSharedArray(int index, ref Matrix4x4 matrix)
    {
      for (int i = 0; i < 16; i++)
      {
        matrix[i] = sharedArray[index + i];
      }
    }

    private void GetQuaternionFromSharedArray(int index, ref Quaternion quaternion)
    {
      quaternion.x = sharedArray[index];
      quaternion.y = sharedArray[index + 1];
      quaternion.z = sharedArray[index + 2];
      quaternion.w = sharedArray[index + 3];
    }

    private void GetVector3FromSharedArray(int index, ref Vector3 vec3)
    {
      vec3.x = sharedArray[index];
      vec3.y = sharedArray[index + 1];
      vec3.z = sharedArray[index + 2];
    }

    private bool GetGamepadFromControllersArray(int controllerIndex, ref WebXRControllerData newControllerData)
    {
      int arrayPosition = controllerIndex * 20;
      int frameNumber = (int)controllersArray[arrayPosition++];
      if (newControllerData.frame == frameNumber)
      {
        return false;
      }

      newControllerData.frame = frameNumber;
      newControllerData.enabled = controllersArray[arrayPosition++] != 0;
      newControllerData.hand = (int)controllersArray[arrayPosition++];
      if (!newControllerData.enabled)
      {
        return true;
      }

      newControllerData.position = new Vector3(controllersArray[arrayPosition++], controllersArray[arrayPosition++], controllersArray[arrayPosition++]);
      newControllerData.rotation = new Quaternion(controllersArray[arrayPosition++], controllersArray[arrayPosition++], controllersArray[arrayPosition++],
          controllersArray[arrayPosition++]);
      newControllerData.trigger = controllersArray[arrayPosition++];
      newControllerData.squeeze = controllersArray[arrayPosition++];
      newControllerData.thumbstick = controllersArray[arrayPosition++];
      newControllerData.thumbstickX = controllersArray[arrayPosition++];
      newControllerData.thumbstickY = controllersArray[arrayPosition++];
      newControllerData.touchpad = controllersArray[arrayPosition++];
      newControllerData.touchpadX = controllersArray[arrayPosition++];
      newControllerData.touchpadY = controllersArray[arrayPosition++];
      newControllerData.buttonA = controllersArray[arrayPosition++];
      newControllerData.buttonB = controllersArray[arrayPosition];
      return true;
    }

    private bool GetHandFromHandsArray(int handIndex, ref WebXRHandData handObject)
    {
      int arrayPosition = handIndex * 230;
      int frameNumber = (int)handsArray[arrayPosition++];
      if (handObject.frame == frameNumber)
      {
        return false;
      }

      handObject.frame = frameNumber;
      handObject.enabled = handsArray[arrayPosition++] != 0;
      handObject.hand = (int)handsArray[arrayPosition++];
      handObject.trigger = handsArray[arrayPosition++];
      handObject.squeeze = handsArray[arrayPosition++];
      if (!handObject.enabled)
      {
        return true;
      }

      for (int i = 0; i <= WebXRHandData.LITTLE_PHALANX_TIP; i++)
      {
        handObject.joints[i].enabled = handsArray[arrayPosition++] != 0;
        handObject.joints[i].position = new Vector3(handsArray[arrayPosition++], handsArray[arrayPosition++], handsArray[arrayPosition++]);
        handObject.joints[i].rotation = new Quaternion(handsArray[arrayPosition++], handsArray[arrayPosition++], handsArray[arrayPosition++],
            handsArray[arrayPosition++]);
        handObject.joints[i].radius = handsArray[arrayPosition++];
      }

      return true;
    }

    private bool GetHitTestPoseFromViewerHitTestPoseArray(ref WebXRHitPoseData hitPoseData)
    {
      int arrayPosition = 0;
      int frameNumber = (int)viewerHitTestPoseArray[arrayPosition++];
      if (hitPoseData.frame == frameNumber)
      {
        return false;
      }

      hitPoseData.frame = frameNumber;
      hitPoseData.available = viewerHitTestPoseArray[arrayPosition++] != 0;
      if (!hitPoseData.available)
      {
        return true;
      }

      hitPoseData.position = new Vector3(viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++],
          viewerHitTestPoseArray[arrayPosition++]);
      hitPoseData.rotation = new Quaternion(viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++],
          viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition]);
      return true;
    }
  }
}