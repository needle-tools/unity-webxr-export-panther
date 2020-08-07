using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

namespace needle.xr.web
{
    public class WebXRLoader : XRLoaderHelper
    {
        static readonly List<WebXRSubsystemDescriptor> sampleSubsystemDescriptors = new List<WebXRSubsystemDescriptor>();
        
        
        /// <summary>
        /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public WebXRCameraSubsystem webXRCameraSubsystem => GetLoadedSubsystem<WebXRCameraSubsystem>();
        
        public override bool Initialize()
        {
            Debug.Log("Initialize " + nameof(WebXRLoader));
            CreateSubsystem<WebXRSubsystemDescriptor, WebXRCameraSubsystem>(sampleSubsystemDescriptors, typeof(WebXRCameraSubsystem).FullName);
            return webXRCameraSubsystem != null;
        }
        

        public override bool Start()
        {
            webXRCameraSubsystem.Start();
            return true;
        }

        public override bool Stop()
        {
            webXRCameraSubsystem.Stop();
            return base.Stop();
        }

        private void OnDestroy()
        {
            webXRCameraSubsystem.Destroy();
        }
    }
}