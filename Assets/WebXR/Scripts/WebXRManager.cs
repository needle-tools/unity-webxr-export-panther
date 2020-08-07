#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System;
using System.Runtime.InteropServices;
#endif
using needle.xr.web;
using UnityEngine;

namespace WebXR
{

  public class WebXRManager : MonoBehaviour
  {
    [Tooltip("Preserve the manager across scenes changes.")]
    public bool dontDestroyOnLoad = true;
    [Header("Tracking")]
    [Tooltip("Default height of camera if no room-scale transform is present.")]
    public float DefaultHeight = 1.2f;

    private static WebXRManager instance;
    [HideInInspector]
    public WebXRState xrState = WebXRState.NORMAL;


    public delegate void XRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect);
    public event XRChange OnXRChange;

    public delegate void HeadsetUpdate(
        Matrix4x4 leftProjectionMatrix,
        Matrix4x4 leftViewMatrix,
        Matrix4x4 rightProjectionMatrix,
        Matrix4x4 rightViewMatrix,
        Matrix4x4 sitStandMatrix);
    public event HeadsetUpdate OnHeadsetUpdate;

    public delegate void ControllerUpdate(WebXRControllerData controllerData);
    public event ControllerUpdate OnControllerUpdate;

    public delegate void HandUpdate(WebXRHandData handData);
    public event HandUpdate OnHandUpdate;

    public delegate void HitTestUpdate(WebXRHitPoseData hitPoseData);
    public event HitTestUpdate OnViewerHitTestUpdate;

    bool viewerHitTestOn = false;

    private WebXRHandData leftHand = new WebXRHandData();
    private WebXRHandData rightHand = new WebXRHandData();

    private WebXRControllerData controller1 = new WebXRControllerData();
    private WebXRControllerData controller2 = new WebXRControllerData();

    private WebXRHitPoseData viewerHitTestPose = new WebXRHitPoseData();


    public static WebXRManager Instance
    {
      get
      {
        if (instance == null)
        {
          var managerInScene = FindObjectOfType<WebXRManager>();

          if (managerInScene != null)
          {
            instance = managerInScene;
          }
          else
          {
            GameObject go = new GameObject("WebXRCameraSet");
            go.AddComponent<WebXRManager>();
          }
        }
        return instance;
      }
    }

    private void Awake()
    {
      Debug.Log("Active Graphics Tier: " + Graphics.activeTier);
      if (null == instance) {
        instance = this;
      } else if (instance != this) {
        Destroy(gameObject);
      }

      if (instance.dontDestroyOnLoad)
      {
        DontDestroyOnLoad(instance);
      }
      xrState = WebXRState.NORMAL;
    }

   

    public void StartViewerHitTest()
    {
      if (xrState == WebXRState.AR && !viewerHitTestOn)
      {
        viewerHitTestOn = true;
// #if UNITY_WEBGL && !UNITY_EDITOR
//         ToggleViewerHitTest();
// #endif
      }
    }

    public void StopViewerHitTest()
    {
      if (xrState == WebXRState.AR && viewerHitTestOn)
      {
        viewerHitTestOn = false;
// #if UNITY_WEBGL && !UNITY_EDITOR
//         ToggleViewerHitTest();
// #endif
      }
    }

    public void HapticPulse(WebXRControllerHand hand, float intensity, float duration)
    {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         ControllerPulse((int)hand, intensity, duration);
// #endif
    }

    float[] GetMatrixFromSharedArray(int index)
    {
      float[] newArray = new float[16];
      // for (int i = 0; i < newArray.Length; i++)
      // {
      //   newArray[i] = sharedArray[index * 16 + i];
      // }
      return newArray;
    }


    bool GetHitTestPoseFromViewerHitTestPoseArray(ref WebXRHitPoseData hitPoseData)
    {
      // int arrayPosition = 0;
      // int frameNumber = (int)viewerHitTestPoseArray[arrayPosition++];
      // if (hitPoseData.frame == frameNumber)
      // {
      //   return false;
      // }
      // hitPoseData.frame = frameNumber;
      // hitPoseData.available = viewerHitTestPoseArray[arrayPosition++] != 0;
      // if (!hitPoseData.available)
      // {
      //   return true;
      // }
      // hitPoseData.position = new Vector3(viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++]);
      // hitPoseData.rotation = new Quaternion(viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++], viewerHitTestPoseArray[arrayPosition++]);
      return true;
    }

    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
#endif
    }

    void Update()
    {
      // bool hasHandsData = false;
      // if (OnHandUpdate != null && this.xrState != WebXRState.NORMAL)
      // {
      //   if (GetHandFromHandsArray(0, ref leftHand))
      //   {
      //     OnHandUpdate(leftHand);
      //   }
      //   if (GetHandFromHandsArray(1, ref rightHand))
      //   {
      //     OnHandUpdate(rightHand);
      //   }
      //   hasHandsData = leftHand.enabled || rightHand.enabled;
      // }
      //
      // if (!hasHandsData && OnControllerUpdate != null && this.xrState != WebXRState.NORMAL)
      // {
      //   if (GetGamepadFromControllersArray(0, ref controller1))
      //   {
      //     OnControllerUpdate(controller1);
      //   }
      //   if (GetGamepadFromControllersArray(1, ref controller2))
      //   {
      //     OnControllerUpdate(controller2);
      //   }
      // }
      //
      // if (OnViewerHitTestUpdate != null && this.xrState == WebXRState.AR)
      // {
      //   if (GetHitTestPoseFromViewerHitTestPoseArray(ref viewerHitTestPose))
      //   {
      //     OnViewerHitTestUpdate(viewerHitTestPose);
      //   }
      // }
    }

    void LateUpdate()
    {
      // if (OnHeadsetUpdate != null && this.xrState != WebXRState.NORMAL)
      // {
      //   Matrix4x4 leftProjectionMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(0));
      //   Matrix4x4 rightProjectionMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(1));
      //   Matrix4x4 leftViewMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(2));
      //   Matrix4x4 rightViewMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(3));
      //   Matrix4x4 sitStandMatrix = WebXRMatrixUtil.NumbersToMatrix(GetMatrixFromSharedArray(4));
      //   // if (!this.capabilities.hasPosition)
      //   // {
      //   //   sitStandMatrix = Matrix4x4.Translate(new Vector3(0, this.DefaultHeight, 0));
      //   // }
      //
      //   OnHeadsetUpdate(
      //       leftProjectionMatrix,
      //       rightProjectionMatrix,
      //       leftViewMatrix,
      //       rightViewMatrix,
      //       sitStandMatrix);
      // }
    }
  }
}
