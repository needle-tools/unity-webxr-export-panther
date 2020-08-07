using UnityEngine;

namespace needle.xr.sample
{
    public class SampleSubsystem : SubsystemLifecycleManager<XRSampleSubsystem, XRSampleSubsystemDescriptor>
    {
        protected override void Awake()
        {
            base.Awake();
            Debug.Log(nameof(Awake) + ": " + subsystem.running);
        }

        private float lastToggleTime;
        
        private void Update()
        {
            if (Time.time - lastToggleTime < 2) return;
            
            ToggleActive();
            Debug.Log(nameof(Update) + ": " + subsystem.running);
        }

        [ContextMenu(nameof(ToggleActive))]
        public void ToggleActive()
        {
            Debug.Log(nameof(ToggleActive));
            lastToggleTime = Time.time;
            
            if(subsystem.running) subsystem.Stop();
            else subsystem.Start();
        }
    }
}