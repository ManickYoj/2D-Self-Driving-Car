using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarController : MonoBehaviour {
  [SerializeField]
  Actuator yAxisActuator = null;

  [SerializeField]
  Actuator xAxisActuator = null;

  [SerializeField]
  Actuator turnActuator = null;

  void Update() {
    yAxisActuator.Set(Input.GetAxis("Vertical"));
    xAxisActuator.Set(Input.GetAxis("Horizontal"));
    turnActuator.Set(Input.GetAxis("Turn"));
  }
}
