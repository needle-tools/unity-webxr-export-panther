using System.Linq;
using UnityEngine;

namespace needle.xr.web
{
    public class WebXRSubsystemDescriptor : SubsystemDescriptor<WebXRCameraSubsystem>
    {
            
    }
    
    public class WebXRCameraSubsystem : Subsystem<WebXRSubsystemDescriptor>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterDescriptor()
        {
            var res = SubsystemRegistration.CreateDescriptor(new WebXRSubsystemDescriptor()
            {
                id = typeof(WebXRCameraSubsystem).FullName,
                subsystemImplementationType = typeof(WebXRCameraSubsystem)
            });
            if (res)
                Debug.Log("Registered " + nameof(WebXRSubsystemDescriptor));
            else Debug.Log("Failed registering " + nameof(WebXRSubsystemDescriptor));
        }

        public override void Start()
        {
            Debug.Log("Hello " + nameof(WebXRCameraSubsystem));
            _running = true;
        }

        public override void Stop()
        {
            Debug.Log("Goodbye " + nameof(WebXRCameraSubsystem));
            _running = false;
        }

        protected override void OnDestroy()
        {
            Debug.Log("The End of " + nameof(WebXRCameraSubsystem));
            _running = false;
        }

        private bool _running;
        public override bool running => _running;
    }
}