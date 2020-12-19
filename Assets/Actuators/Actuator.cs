using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Settable {
  void Set(float val);
}

public abstract class Actuator : MonoBehaviour, Settable {
  public abstract void Set(float val);
}
