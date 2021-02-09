using System;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

public class WebXRDemo : MonoBehaviour
{
    public Transform Scene;
    public float ARScale = .1f;
    public List<GameObject> HideInAR;
    private Vector3 normalScale;

    private void Awake()
    {
        normalScale = Scene.transform.localScale;
    }

    private void OnEnable()
    {
        WebXRSubsystem.OnXRChange += OnChanged;
    }
    
    private void OnDisable()
    {
        WebXRSubsystem.OnXRChange -= OnChanged;
    }

    private void OnChanged(WebXRState state, int viewscount, Rect leftrect, Rect rightrect)
    {
        switch (state)
        {
            case WebXRState.NORMAL:
            case WebXRState.VR:
                Scene.transform.localScale = normalScale;
                foreach (var obj in HideInAR) obj.SetActive(true);
                break;
            case WebXRState.AR:
                Scene.transform.localScale = normalScale * ARScale;
                Debug.Log("Scaled scene " + Scene.transform.localScale);
                foreach (var obj in HideInAR) obj.SetActive(false);
                break;
        }
    }
}
