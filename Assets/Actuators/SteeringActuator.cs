using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringActuator : Actuator {
  public float maxSteer = 0.5f;
  public float curSteer = 0f;

  private Rigidbody2D rbody;

  void Start() {
    this.rbody = GetComponentInParent<Rigidbody2D>();
  }

  void FixedUpdate() {
    // float forwardVel = transform.InverseTransformDirection(rbody.velocity).y;
    rbody.rotation += curSteer;
  }

  public override void Set(float val) {
    this.curSteer = val * this.maxSteer;
  }
}
