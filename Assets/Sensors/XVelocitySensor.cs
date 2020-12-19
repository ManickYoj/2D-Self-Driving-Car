using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XVelocitySensor : Sensor {
  Rigidbody2D rbody;

  void Start() {
    rbody = GetComponent<Rigidbody2D>();
  }

  public override float Read(byte _) {
    return transform.InverseTransformDirection(rbody.velocity).x;
  }
}