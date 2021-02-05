using System.Collections.Generic;
using needle.weaver.webxr;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace WebXR
{
  public class WebXRLoader : XRLoaderHelper
  {
    private static readonly List<WebXRSubsystemDescriptor> subsystemDescriptors = new List<WebXRSubsystemDescriptor>();
    private static readonly List<XRDisplaySubsystemDescriptor> displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
    private static readonly List<XRInputSubsystemDescriptor> inputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

    private WebXRSubsystem WebXRSubsystem => GetLoadedSubsystem<WebXRSubsystem>();
    private XRInputSubsystem XRInputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
    private XRDisplaySubsystem XRDisplaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();

    internal static XRDisplaySubsystem DisplaySubsystem { get; private set; }
    internal static XRInputSubsystem InputSubsystem { get; private set; }

    public override bool Initialize()
    {
      #if UNITY_INPUT_SYSTEM
      InputSystem.RegisterLayout(typeof(XRHMD));
      InputSystem.onDeviceChange += (arg, evt) => Debug.Log("Device " + arg + " " + evt);
      #endif
      
      CreateSubsystem<WebXRSubsystemDescriptor, WebXRSubsystem>(subsystemDescriptors, typeof(WebXRSubsystem).FullName);
      CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(displaySubsystemDescriptors, XRDisplaySubsystem_Patch.Id);
      CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(inputSubsystemDescriptors, XRInputSubsystem_Patch.Id);
      return WebXRSubsystem != null;
    }
    
    public override bool Start()
    {
      var settings = WebXRSettings.GetSettings();
      if (settings != null)
      {
        Debug.Log($"Got WebXRSettings");
#if UNITY_WEBGL && !UNITY_EDITOR
        Native.SetWebXRSettings(settings.ToJson());
#endif
        Debug.Log($"Sent WebXRSettings");
      }

      DisplaySubsystem = XRDisplaySubsystem;
      InputSubsystem = XRInputSubsystem;
      
      // if we start it here we get a black screen.
      // Not sure why it works when we stop it after VR mode
      // XRDisplaySubsystem.Start();
      // XRDisplaySubsystem_Patch.AttachDisplayBehaviour<RenderVR>();
      XRInputSubsystem.Start();
      WebXRSubsystem.Start();
      return true;
    }

    public override bool Stop()
    {
      WebXRSubsystem.Stop();
      XRDisplaySubsystem.Stop();
      XRInputSubsystem.Stop();
      return base.Stop();
    }

    public override bool Deinitialize()
    {
      WebXRSubsystem.Destroy();
      XRDisplaySubsystem.Destroy();
      XRInputSubsystem.Destroy();
      return base.Deinitialize();
    }
  }
}