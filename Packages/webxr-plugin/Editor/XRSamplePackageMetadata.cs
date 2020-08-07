using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.XR.Management.Metadata;

namespace needle.xr.web
{
    internal class XRPackage : IXRPackage
    {
        private class WebXRPluginLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        private class WebXRPluginMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; } 
        }

        static readonly IXRPackageMetadata s_Metadata = new WebXRPluginMetadata()
        {
            packageName = "Web XR Plugin",
            packageId = "com.needle.xr.web",
            settingsType = typeof(WebXRSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>() 
            {
                new WebXRPluginLoaderMetadata() 
                {
                    loaderName = "Web XR Plugin",
                    loaderType = typeof(WebXRLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>() 
                    {
                        BuildTargetGroup.WebGL
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