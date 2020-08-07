using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.XR.Management.Metadata;

namespace needle.xr.sample
{
    internal class XRPackage : IXRPackage
    {
        private class XRPluginSampleLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        private class XRPluginSampleMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; } 
        }

        static readonly IXRPackageMetadata s_Metadata = new XRPluginSampleMetadata()
        {
            packageName = "XR Plugin Sample",
            packageId = "com.needle.xr.sample",
            settingsType = typeof(XRSampleSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>() 
            {
                new XRPluginSampleLoaderMetadata() 
                {
                    loaderName = "XR Plugin Sample",
                    loaderType = typeof(XRSampleLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>() 
                    {
                        BuildTargetGroup.Standalone
                    }
                },
            }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            return true;
        }
    }
}