using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThurstActuator : Actuator {
  public float maxThrust = 10f;
  float throttle = 0f;

  private Rigidbody2D rbody;

  void Start() {
    this.rbody = GetComponentInParent<Rigidbody2D>();
  }

  void FixedUpdate() {
    this.rbody.AddForceAtPosition(
      -transform.up * throttle * maxThrust,
      this.transform.position
    );
  }

  public override void Set(float val) {
    this.throttle = val;
  }

  void OnDrawGizmos() {
    Vector3 thrustDirEndpoint = transform.position + transform.up * 0.05f * this.maxThrust;

    Gizmos.color = new Color(1, 0.7f, 0); // Orange
    Gizmos.DrawLine(transform.position, thrustDirEndpoint);
    Gizmos.DrawWireSphere(transform.position, 0.01f * this.maxThrust);
  }
}
