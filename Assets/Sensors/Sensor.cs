using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Readable {
  float Read(byte cacheKey);
}

public abstract class Sensor : MonoBehaviour, Readable {
  public abstract float Read(byte cacheKey);
}