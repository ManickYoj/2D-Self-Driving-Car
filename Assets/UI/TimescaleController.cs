using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleController : MonoBehaviour {
  public void SetTimescale(float value) {
    Time.timeScale = value;
  }
}
