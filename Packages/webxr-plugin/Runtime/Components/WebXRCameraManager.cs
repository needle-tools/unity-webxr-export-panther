using UnityEngine;

namespace needle.xr.web
{
    public class WebXRCameraManager : SubsystemLifecycleManager<WebXRSubsystem, WebXRSubsystemDescriptor>
    {
        public enum CameraID
        {
            Main,
            LeftVR,
            RightVR,
            LeftAR,
            RightAR
        }

        [SerializeField] private Camera cameraMain, cameraL, cameraR, cameraARL, cameraARR;

        private WebXRState xrState = WebXRState.NORMAL;
        private Rect leftRect, rightRect;

        private int viewsCount = 1;

        private bool switched = false;

        protected override void Awake()
        {
            base.Awake();
            subsystem.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            subsystem.OnXRChange += onXRChange;
            subsystem.OnHeadsetUpdate += onHeadsetUpdate;
        }

        private void OnDestroy()
        {
            subsystem.Stop();
        }

        private void Update()
        {
            subsystem.OnUpdate();
            
            if (switched)
            {
                return;
            }

            switched = true;
            switch (xrState)
            {
                case WebXRState.AR:
                    cameraMain.enabled = false;
                    cameraL.enabled = false;
                    cameraR.enabled = false;
                    cameraARL.enabled = viewsCount > 0;
                    cameraARL.rect = leftRect;
                    cameraARR.enabled = viewsCount > 1;
                    cameraARR.rect = rightRect;
                    break;
                case WebXRState.VR:
                    cameraMain.enabled = false;
                    cameraL.enabled = viewsCount > 0;
                    cameraR.enabled = viewsCount > 1;
                    cameraARL.enabled = false;
                    cameraARR.enabled = false;
                    break;
                case WebXRState.NORMAL:
                    cameraMain.enabled = true;
                    cameraL.enabled = false;
                    cameraR.enabled = false;
                    cameraARL.enabled = false;
                    cameraARR.enabled = false;
                    break;
            }
        }

        private void LateUpdate()
        {
            subsystem.OnLateUpdate();
        }

        public Camera GetCamera(CameraID cameraID)
        {
            switch (cameraID)
            {
                case CameraID.LeftVR:
                    return cameraL;
                case CameraID.RightVR:
                    return cameraR;
                case CameraID.LeftAR:
                    return cameraARL;
                case CameraID.RightAR:
                    return cameraARR;
            }

            return cameraMain;
        }

        private void onXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            xrState = state;
            this.viewsCount = viewsCount;
            this.leftRect = leftRect;
            this.rightRect = rightRect;
            switched = false;
        }

        private void onHeadsetUpdate(
            Matrix4x4 leftProjectionMatrix,
            Matrix4x4 rightProjectionMatrix,
            Matrix4x4 leftViewMatrix,
            Matrix4x4 rightViewMatrix,
            Matrix4x4 sitStandMatrix)
        {
            if (xrState == WebXRState.VR)
            {
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraL.transform, leftViewMatrix * sitStandMatrix.inverse);
                cameraL.projectionMatrix = leftProjectionMatrix;
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraR.transform, rightViewMatrix * sitStandMatrix.inverse);
                cameraR.projectionMatrix = rightProjectionMatrix;
            }
            else if (xrState == WebXRState.AR)
            {
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraARL.transform, leftViewMatrix * sitStandMatrix.inverse);
                cameraARL.projectionMatrix = leftProjectionMatrix;
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraARR.transform, rightViewMatrix * sitStandMatrix.inverse);
                cameraARR.projectionMatrix = rightProjectionMatrix;
            }
        }
    }
}