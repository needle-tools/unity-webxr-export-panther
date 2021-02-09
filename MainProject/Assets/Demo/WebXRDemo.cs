using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebXR;

public class WebXRDemo : MonoBehaviour
{
	public Camera Camera;
	public Transform Scene;
	public float ARScale = .1f;
	public List<GameObject> HideInAR;
	private Vector3 normalScale;

	public GameObject Canvas;
	public Button ScaleToggle;

	private bool isMiniature = true;

	private void Awake()
	{
		normalScale = Scene.transform.localScale;
		if (Canvas) Canvas.SetActive(false);
	}

	[ContextMenu(nameof(OnToggleScale))]
	private void OnToggleScale()
	{
#if !UNITY_EDITOR
		if (WebXRSubsystem.xrState != WebXRState.AR) return;
#endif
		if (Scene.transform.localScale.x < normalScale.x)
			Scene.transform.localScale = normalScale;
		else
			Scene.transform.localScale = normalScale * ARScale;
	}

	private void OnEnable()
	{
		WebXRSubsystem.OnXRChange += OnChanged;
		if (ScaleToggle)
			ScaleToggle.onClick.AddListener(this.OnToggleScale);
	}

	private void OnDisable()
	{
		WebXRSubsystem.OnXRChange -= OnChanged;
		if (ScaleToggle)
			ScaleToggle.onClick.RemoveListener(this.OnToggleScale);
	}

	private void OnChanged(WebXRState state, int viewscount, Rect leftrect, Rect rightrect)
	{
		switch (state)
		{
			case WebXRState.NORMAL:
			case WebXRState.VR:
				if (Canvas)
					Canvas.SetActive(false);
				Scene.transform.localScale = normalScale;
				foreach (var obj in HideInAR) obj.SetActive(true);
				break;
			case WebXRState.AR:
				if (Canvas)
					Canvas.SetActive(true);
				SetCameraParentToFloor();
				Scene.transform.localScale = normalScale * ARScale;
				Debug.Log("Scaled scene " + Scene.transform.localScale);
				foreach (var obj in HideInAR) obj.SetActive(false);
				break;
		}
	}

	private void SetCameraParentToFloor()
	{
		if (Camera && Camera != null && Camera.transform != null && Camera.transform.parent)
		{
			var t = Camera.transform.parent;
			if (transform != null)
			{
				var pos = t.position;
				pos.y = 0;
				t.position = Vector3.zero;
			}
		}
	}
}