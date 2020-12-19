using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
  public Transform target;

  // Update is called once per frame
  void Update() {
    if (target != null) {
      Vector3 newPos = transform.position;

      newPos.x = target.position.x;
      newPos.y = target.position.y;

      this.transform.position = newPos;
    }
  }
}
