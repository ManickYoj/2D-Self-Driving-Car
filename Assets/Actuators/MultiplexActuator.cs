using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplexActuator : Actuator {
  [SerializeField]
  private Actuator[] positiveOutputs;

  [SerializeField]
  private Actuator[] negativeOutputs;

  public override void Set(float input) {
    // Map -1 to 0 inputs as 0. Apply the 0 to 1 result to
    // the outputs that should fire on a positive input
    foreach(Actuator output in this.positiveOutputs) {
      float outputVal = Mathf.Max(0, input);
      output.Set(outputVal);
    }

    // Map 0 to 1 inputs to 0; Invert 0 to -1 inputs so that they map
    // to 0 to 1 to outputs that should fire on a negative input
    foreach(Actuator output in this.negativeOutputs) {
      float outputVal = -Mathf.Min(0, input);
      output.Set(outputVal);
    }
  }
}
