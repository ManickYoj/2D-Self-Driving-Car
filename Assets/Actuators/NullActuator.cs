using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullActuator : Actuator {
  public override void Set(float val) { return; }
}
