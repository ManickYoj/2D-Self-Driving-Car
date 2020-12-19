using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensor : Sensor  {
  public float length = 1f;

  byte cacheKey;
  float cachedValue = 1f;

  public override float Read(byte cacheKey) {
    if (cacheKey == this.cacheKey) return this.cachedValue;
    this.cacheKey = cacheKey;

    // Ignore the car and ignore raycast layers (2, 8)
    int ignoreRaycastMask = 1 << 2;
    int carMask = 1 << 8;
    int layerMask = ignoreRaycastMask | carMask;
    layerMask = ~layerMask; // Negate

    RaycastHit2D hit = Physics2D.Raycast(
      transform.position,
      transform.up,
      this.length,
      layerMask
    );

    if (hit.collider == null) {
      this.cachedValue = 1f;
    } else {
      // Normalize hit distance between 0 and 1
      this.cachedValue = hit.distance / length;
    }

    return this.cachedValue;
  }

  void OnDrawGizmos() {
    Vector3 endpoint = transform.position + transform.up * this.length * this.cachedValue;

    if (cachedValue == 1f) {
      Gizmos.color = Color.green;
      Gizmos.DrawLine(transform.position, endpoint);
    } else {
      Gizmos.color = Color.Lerp(Color.red, Color.green, this.cachedValue/this.length);;
      Gizmos.DrawLine(transform.position, endpoint);
    }
  }
}
