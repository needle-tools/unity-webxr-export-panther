using System.Linq;
using UnityEngine;

namespace needle.xr.sample
{
    public class XRSampleSubsystemDescriptor : SubsystemDescriptor<XRSampleSubsystem>
    {
            
    }
    
    public class XRSampleSubsystem : Subsystem<XRSampleSubsystemDescriptor>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterDescriptor()
        {
            var res = SubsystemRegistration.CreateDescriptor(new XRSampleSubsystemDescriptor()
            {
                id = typeof(XRSampleSubsystem).FullName,
                subsystemImplementationType = typeof(XRSampleSubsystem)
            });
            if (res)
                Debug.Log("Registered " + nameof(XRSampleSubsystemDescriptor));
            else Debug.Log("Failed registering " + nameof(XRSampleSubsystemDescriptor));
        }

        public override void Start()
        {
            Debug.Log("Hello " + nameof(XRSampleSubsystem));
            _running = true;
        }

        public override void Stop()
        {
            Debug.Log("Goodbye " + nameof(XRSampleSubsystem));
            _running = false;
        }

        protected override void OnDestroy()
        {
            Debug.Log("The End of " + nameof(XRSampleSubsystem));
            _running = false;
        }

        private bool _running;
        public override bool running => _running;
    }
}