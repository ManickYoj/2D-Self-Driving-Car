using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraTarget : MonoBehaviour {
  public void SetTarget() {
    // One of the janker things I've coded but I might revise it if I keep iterating
    LeaderboardRow lr = GetComponent<LeaderboardRow>();
    CameraController cc = Camera.main.GetComponent<CameraController>();
    cc.target = lr.target.transform;
  }
}
