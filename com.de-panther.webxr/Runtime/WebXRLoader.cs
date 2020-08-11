using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

namespace needle.xr.web
{
    public class WebXRLoader : XRLoaderHelper
    {
        static readonly List<WebXRSubsystemDescriptor> webxrDescriptors = new List<WebXRSubsystemDescriptor>();
        
        
        /// <summary>
        /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public WebXRSubsystem webXRSubsystem => GetLoadedSubsystem<WebXRSubsystem>();
        
        public override bool Initialize()
        {
            Debug.Log("Initialize " + nameof(WebXRLoader));
            CreateSubsystem<WebXRSubsystemDescriptor, WebXRSubsystem>(webxrDescriptors, typeof(WebXRSubsystem).FullName);
            return webXRSubsystem != null;
        }
        

        public override bool Start()
        {
            webXRSubsystem.Start();
            return true;
        }

        public override bool Stop()
        {
            webXRSubsystem.Stop();
            return base.Stop();
        }

        private void OnDestroy()
        {
            webXRSubsystem.Destroy();
        }
    }
}