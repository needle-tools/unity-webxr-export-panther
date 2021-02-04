using System.Collections.Generic;
using needle.weaver.webxr;
using UnityEngine;
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

    public override bool Initialize()
    {
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
      WebXRSubsystem.Start();
      // XRDisplaySubsystem.Start();
      XRInputSubsystem.Start();
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