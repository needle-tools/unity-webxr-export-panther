namespace WebXR
{
  public enum WebXRState { VR, AR, NORMAL }

  public class WebXRManager : SubsystemLifecycleManager<WebXRSubsystem, WebXRSubsystemDescriptor>
  {
      public static WebXRManager Instance { get; private set; }

      public WebXRState XRState => WebXRSubsystem.xrState;
      
      public static event WebXRSubsystem.XRCapabilitiesUpdate OnXRCapabilitiesUpdate
      {
          add => WebXRSubsystem.OnXRCapabilitiesUpdate += value;
          remove => WebXRSubsystem.OnXRCapabilitiesUpdate -= value;
      }

      public static event WebXRSubsystem.XRChange OnXRChange
      {
          add => WebXRSubsystem.OnXRChange += value;
          remove => WebXRSubsystem.OnXRChange -= value;
      }

      public static event WebXRSubsystem.ControllerUpdate OnControllerUpdate
      {
          add => WebXRSubsystem.OnControllerUpdate += value;
          remove => WebXRSubsystem.OnControllerUpdate -= value;
      }

      public static event WebXRSubsystem.HandUpdate OnHandUpdate
      {
          add => WebXRSubsystem.OnHandUpdate += value;
          remove => WebXRSubsystem.OnHandUpdate -= value;
      }

      public static event WebXRSubsystem.HeadsetUpdate OnHeadsetUpdate
      {
          add => WebXRSubsystem.OnHeadsetUpdate += value;
          remove => WebXRSubsystem.OnHeadsetUpdate -= value;
      }

      public static event WebXRSubsystem.HitTestUpdate OnViewerHitTestUpdate
      {
          add => WebXRSubsystem.OnViewerHitTestUpdate += value;
          remove => WebXRSubsystem.OnViewerHitTestUpdate -= value;
      }
      
      public static void HapticPulse(WebXRControllerHand hand, float intensity, float duration)
      {
          WebXRSubsystem.HapticPulse(hand, intensity, duration);
      }

      public void StartViewerHitTest()
      {
          WebXRSubsystem.StartViewerHitTest();
      }

      public void StopViewerHitTest()
      {
          WebXRSubsystem.StopViewerHitTest();
      }
      
#if !UNITY_EDITOR && UNITY_WEBGL
      protected override void Awake()
      {
          base.Awake();
          Instance = this;
          enabled = subsystem != null;
      }
      #else
      protected override bool PlatformSupported => false;
#endif
  }
}
