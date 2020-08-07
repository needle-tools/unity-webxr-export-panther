using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace needle.xr.web
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
            if (res)
                Debug.Log("Registered " + nameof(WebXRSubsystemDescriptor));
            else Debug.Log("Failed registering " + nameof(WebXRSubsystemDescriptor));
        }

        public override void Start()
        {
            if (_running)
            {
                Debug.LogWarning( nameof(WebXRSubsystem) + " is already running");
                return;
            }
            
            if (Instance != null && Instance != this) throw new Exception("Instance is already set by another subsystem");
            Instance = this;
            
            _running = true;
            set_webxr_events(OnStartAR, OnStartVR, OnEndXR, OnXRCapabilities);
            InitControllersArray(controllersArray, controllersArray.Length);
            InitHandsArray(handsArray, handsArray.Length);
            InitViewerHitTestPoseArray(viewerHitTestPoseArray, viewerHitTestPoseArray.Length);
            InitXRSharedArray(sharedArray, sharedArray.Length);
            ListenWebXRData();
        }

        public override void Stop()
        {
            _running = false;
        }
        
        protected override void OnDestroy()
        {
            _running = false;
            if(Instance == this)
                Instance = null;
        }

        private bool _running;
        public override bool running => _running;

        internal void OnUpdate()
        {
            // bool hasHandsData = false;
            // if (OnHandUpdate != null && this.xrState != WebXRState.NORMAL)
            // {
            //     if (GetHandFromHandsArray(0, ref leftHand))
            //     {
            //         OnHandUpdate(leftHand);
            //     }
            //     if (GetHandFromHandsArray(1, ref rightHand))
            //     {
            //         OnHandUpdate(rightHand);
            //     }
            //     hasHandsData = leftHand.enabled || rightHand.enabled;
            // }
            //
            // if (!hasHandsData && OnControllerUpdate != null && this.xrState != WebXRState.NORMAL)
            // {
            //     if (GetGamepadFromControllersArray(0, ref controller1))
            //     {
            //         OnControllerUpdate(controller1);
            //     }
            //     if (GetGamepadFromControllersArray(1, ref controller2))
            //     {
            //         OnControllerUpdate(controller2);
            //     }
            // }
            //
            // if (OnViewerHitTestUpdate != null && this.xrState == WebXRState.AR)
            // {
            //     if (GetHitTestPoseFromViewerHitTestPoseArray(ref viewerHitTestPose))
            //     {
            //         OnViewerHitTestUpdate(viewerHitTestPose);
            //     }
            // }
        }

        internal void OnLateUpdate()
        {
            if (OnHeadsetUpdate == null || this.xrState == WebXRState.NORMAL) return;
            var leftProjectionMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(0));
            var rightProjectionMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(1));
            var leftViewMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(2));
            var rightViewMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(3));
            var sitStandMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(4));
            if (!this.capabilities.hasPosition)
            {
                sitStandMatrix = Matrix4x4.Translate(new Vector3(0, this.DefaultHeight, 0));
            }

            OnHeadsetUpdate?.Invoke(
                leftProjectionMatrix,
                rightProjectionMatrix,
                leftViewMatrix,
                rightViewMatrix,
                sitStandMatrix);
        }
        

        float[] GetMatrixFromSharedArray(int index)
        {
            float[] newArray = new float[16];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = sharedArray[index * 16 + i];
            }
            return newArray;
        }
        
        [Header("Tracking")]
        [Tooltip("Default height of camera if no room-scale transform is present.")]
        public float DefaultHeight = 1.2f;
        
        bool viewerHitTestOn = false;
        
        public delegate void XRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect);
        public event XRChange OnXRChange;
        
        public delegate void HeadsetUpdate(
            Matrix4x4 leftProjectionMatrix,
            Matrix4x4 leftViewMatrix,
            Matrix4x4 rightProjectionMatrix,
            Matrix4x4 rightViewMatrix,
            Matrix4x4 sitStandMatrix);
        public event HeadsetUpdate OnHeadsetUpdate;
        
        private WebXRState xrState = WebXRState.NORMAL;
        public WebXRState XRState => xrState;
        
        private static WebXRSubsystem Instance { get; set; }

        // Shared array which we will load headset data in from webxr.jslib
        // Array stores  5 matrices, each 16 values, stored linearly.
        float[] sharedArray = new float[5 * 16];

        // Shared array for controllers data
        float[] controllersArray = new float[2 * 20];

        // Shared array for hands data
        float[] handsArray = new float[2 * (25 * 9 + 5)];

        // Shared array for hit-test pose data
        float[] viewerHitTestPoseArray = new float[9];
        
        private WebXRDisplayCapabilities capabilities = new WebXRDisplayCapabilities();
        
        public delegate void XRCapabilitiesUpdate(WebXRDisplayCapabilities capabilities);
        public event XRCapabilitiesUpdate OnXRCapabilitiesUpdate;
        
        
        public void HapticPulse(WebXRControllerHand hand, float intensity, float duration)
        { 
            ControllerPulse((int)hand, intensity, duration);
        }

        // received start VR from WebVR browser
        [MonoPInvokeCallback(typeof(Action<int, float, float, float, float, float, float, float, float>))]
        public static void OnStartAR(int viewsCount,
            float left_x, float left_y, float left_w, float left_h,
            float right_x, float right_y, float right_w, float right_h)
        {
            Instance.setXrState(WebXRState.AR, viewsCount,
                new Rect(left_x, left_y, left_w, left_h),
                new Rect(right_x, right_y, right_w, right_h));
        }

        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void OnStartVR(int viewsCount)
        {
            Instance.setXrState(WebXRState.VR, viewsCount, new Rect(), new Rect());
        }

        // receive end VR from WebVR browser
        [MonoPInvokeCallback(typeof(Action))]
        public static void OnEndXR()
        {
            Instance.setXrState(WebXRState.NORMAL, 1, new Rect(), new Rect());
        }
        
        
        // Handles WebXR capabilities from browser
        [MonoPInvokeCallback(typeof(Action<string>))]
        public static void OnXRCapabilities(string json)
        {
            WebXRDisplayCapabilities capabilities = JsonUtility.FromJson<WebXRDisplayCapabilities>(json);
            Instance.OnXRCapabilities(capabilities);
        }

        public void OnXRCapabilities(WebXRDisplayCapabilities capabilities)
        {
        this.capabilities = capabilities;

            if (OnXRCapabilitiesUpdate != null)
                OnXRCapabilitiesUpdate(capabilities);
        }

        public void setXrState(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            this.xrState = state;
            viewerHitTestOn = false;
            if (OnXRChange != null)
                OnXRChange(state, viewsCount, leftRect, rightRect);
        }



        [DllImport("__Internal")]
        private static extern void InitXRSharedArray(float[] array, int length);

        [DllImport("__Internal")]
        private static extern void InitControllersArray(float[] array, int length);

        [DllImport("__Internal")]
        private static extern void InitHandsArray(float[] array, int length);

        [DllImport("__Internal")]
        private static extern void InitViewerHitTestPoseArray(float[] array, int length);

        [DllImport("__Internal")]
        private static extern void ToggleViewerHitTest();

        [DllImport("__Internal")]
        private static extern void ControllerPulse(int controller, float intensity, float duration);

        [DllImport("__Internal")]
        private static extern void ListenWebXRData();

        [DllImport("__Internal")]
        private static extern void set_webxr_events(Action<int, float, float, float, float, float, float, float, float> on_start_ar,
            Action<int> on_start_vr,
            Action on_end_xr,
            Action<string> on_xr_capabilities);
    }
}