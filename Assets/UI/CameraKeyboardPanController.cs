using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Camera))]
public class CameraKeyboardPanController : MonoBehaviour {
  private Camera cam;
  private Rigidbody2D rbody;
  private CameraController camController;

  [SerializeField]
  private float moveSpeed = 5.0f;

  void Awake() {
    this.cam = GetComponent<Camera>();
    this.rbody = GetComponent<Rigidbody2D>();
    this.camController = GetComponent<CameraController>();
  }

  void Update() {
    Vector2 moveVector = new Vector2(
      Input.GetAxisRaw("Horizontal"),
      Input.GetAxisRaw("Vertical")
    ).normalized;

    rbody.velocity = (
      moveVector *
      moveSpeed *
      cam.orthographicSize
    );

    if (moveVector != Vector2.zero) {
      camController.target = null;
    }
  }
}
