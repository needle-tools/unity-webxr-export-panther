using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

namespace needle.xr.sample
{
    public class XRSampleLoader : XRLoaderHelper
    {
        static readonly List<XRSampleSubsystemDescriptor> sampleSubsystemDescriptors = new List<XRSampleSubsystemDescriptor>();
        
        
        /// <summary>
        /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRSampleSubsystem XRSampleSubsystem => GetLoadedSubsystem<XRSampleSubsystem>();
        
        public override bool Initialize()
        {
            Debug.Log("Initialize " + nameof(XRSampleLoader));
            CreateSubsystem<XRSampleSubsystemDescriptor, XRSampleSubsystem>(sampleSubsystemDescriptors, typeof(XRSampleSubsystem).FullName);
            return XRSampleSubsystem != null;
        }
        

        public override bool Start()
        {
            XRSampleSubsystem.Start();
            return true;
        }

        public override bool Stop()
        {
            XRSampleSubsystem.Stop();
            return base.Stop();
        }

        private void OnDestroy()
        {
            XRSampleSubsystem.Destroy();
        }
    }
}