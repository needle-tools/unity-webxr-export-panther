﻿using System;
using UnityEngine;
using UnityEngine.XR.Management;

namespace WebXR
{
  [System.Serializable]
  [XRConfigurationData("WebXR", "WebXR.Settings")]
  public class WebXRSettings : ScriptableObject
  {
    public static WebXRSettings GetSettings()
    {
      // When running in the Unity Editor, we have to load user's customization of configuration data directly from
      // EditorBuildSettings. At runtime, we need to grab it from the static instance field instead.
#if UNITY_EDITOR
      UnityEditor.EditorBuildSettings.TryGetConfigObject<WebXRSettings>("WebXR.Settings", out var settings);
#elif UNITY_WEBGL
      var settings = WebXRSettings.Instance;
#endif
      return settings;
    }
    
    public enum ReferenceSpaceTypes
    {
      local = 1,
      local_floor = 2,
      bounded_floor = 4,
      unbounded = 8,
      viewer = 16,
    }

    [Flags]
    public enum ExtraFeatureTypes
    {
      hit_test = 1,
      hand_tracking = 2
    }

    [Header("VR Settings")]
    public ReferenceSpaceTypes VRRequiredReferenceSpace = ReferenceSpaceTypes.local_floor;
    public ExtraFeatureTypes VROptionalFeatures = ExtraFeatureTypes.hand_tracking;

    [Header("AR Settings")]
    public ReferenceSpaceTypes ARRequiredReferenceSpace = ReferenceSpaceTypes.local_floor;
    public ExtraFeatureTypes AROptionalFeatures = (ExtraFeatureTypes)(-1);

    string EnumToString<T>(T value) where T : Enum
    {
      return value.ToString().Replace('_','-');
    }

    string FlagsToString<T>(T value) where T : Enum
    {
      if (value.ToString() == "0")
      {
        return "[]";
      }
      var flags = Enum.GetValues(typeof(T));
      string result = "[";
      foreach (var flag in flags)
      {
        if (value.HasFlag((Enum)flag))
        {
          result += "\"" + flag + "\",";
        }
      }
      result = result.Remove(result.Length - 1).Replace('_','-');
      result += "]";
      return result;
    }

    // TODO: Replace with a better way to send the settings object to native
    [ContextMenu("ToJson")]
    public string ToJson()
    {
      string result = $@"{{
        ""VRRequiredReferenceSpace"": [""{EnumToString(VRRequiredReferenceSpace)}""],
        ""VROptionalFeatures"": {FlagsToString(VROptionalFeatures)},
        ""ARRequiredReferenceSpace"": [""{EnumToString(ARRequiredReferenceSpace)}""],
        ""AROptionalFeatures"": {FlagsToString(AROptionalFeatures)}
}}";
      return result;
    }

#if !UNITY_EDITOR
    private static WebXRSettings instance = null;
    public static WebXRSettings Instance
    {
      get { return instance; }
    }

    void Awake()
    {
      instance = this;
    }
#endif
  }
}
