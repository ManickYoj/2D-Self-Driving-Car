using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomController : MonoBehaviour {
  private Camera cam;

  [SerializeField]
  private float zoomSpeed = 300f;
  [SerializeField]
  float smoothTime = 0.1f;
  [SerializeField]
  float minCameraSize= 1f;
  [SerializeField]
  float maxCameraSize= 10f;

  float targetCameraSize;
  float currentZoomSpeed = 0.0f;

  void Awake() {
    this.cam = GetComponent<Camera>();
    this.targetCameraSize = cam.orthographicSize;
  }

  void Update() {
    cam.orthographicSize = Mathf.SmoothDamp(
      cam.orthographicSize,
      targetCameraSize,
      ref currentZoomSpeed,
      smoothTime
    );

    float zoomInput = Input.GetAxis("Mouse ScrollWheel");

    if (zoomInput != 0) {
      targetCameraSize += zoomSpeed * -zoomInput * Time.deltaTime;
      targetCameraSize = Mathf.Clamp(targetCameraSize, minCameraSize, maxCameraSize);
    }
  }
}