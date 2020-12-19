using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {
    public Collider2D[] checkpoints;

    public float Distance(
      Vector3 startPos,
      Vector3 curPos,
      Collider2D firstCheckpoint,
      Collider2D lastCheckpoint,
      int lapNum
    ) {
      if (firstCheckpoint == null) {
        return Vector2.Distance(
          (Vector2) startPos,
          (Vector2) curPos
        );
      } else {
        return Vector2.Distance(
          (Vector2) startPos,
          (Vector2) firstCheckpoint.transform.position
        ) + Vector2.Distance(
          (Vector2) lastCheckpoint.transform.position,
          (Vector2) curPos
        ) + DistanceBetween(
          firstCheckpoint,
          lastCheckpoint
        ) + LapDistance() * lapNum;
      }
    }

    // Distance between any two checkpoints short of a full lap
    public float DistanceBetween(
      Collider2D firstCheckpoint,
      Collider2D lastCheckpoint
    ) {
      if (firstCheckpoint == lastCheckpoint) return 0f;

      int firstIndex = Array.IndexOf(checkpoints, firstCheckpoint);
      int lastIndex = Array.IndexOf(checkpoints, lastCheckpoint);
      float distance = 0f;

      if (firstIndex < lastIndex) {
        for(int i = firstIndex; i < lastIndex; i++) {
          distance += Vector2.Distance(
            (Vector2) checkpoints[i].transform.position,
            (Vector2) checkpoints[i+1].transform.position
          );
        }
      } else { // firstIndex > lastIndex
        distance += DistanceBetween(
          firstCheckpoint,
          checkpoints[checkpoints.Length - 1]
        );

        distance += Vector2.Distance(
          (Vector2) checkpoints[checkpoints.Length - 1].transform.position,
          (Vector2) checkpoints[0].transform.position
        );

        distance += DistanceBetween(
          checkpoints[0],
          lastCheckpoint
        );
      }

      return distance;
    }

    public Collider2D NextCheckpoint(Collider2D lastCheckpoint) {
      int lastIndex = Array.IndexOf(this.checkpoints, lastCheckpoint);
      int nextIndex = (lastIndex + 1) % this.checkpoints.Length;
      return this.checkpoints[nextIndex];
    }

    public float LapDistance() {
      Collider2D firstCheckpoint = this.checkpoints[0];
      Collider2D lastCheckpoint = this.checkpoints[checkpoints.Length - 1];

      return DistanceBetween(firstCheckpoint, lastCheckpoint) +
      DistanceBetween(lastCheckpoint, firstCheckpoint);
    }

    public bool IsForward(Collider2D first, Collider2D second) {
      int firstIndex = Array.IndexOf(checkpoints, first);
      int secondIndex = Array.IndexOf(checkpoints, second);

      if (firstIndex == checkpoints.Length - 1 && secondIndex == 0) {
        return true;
      } else {
        return secondIndex == firstIndex || secondIndex == firstIndex + 1;
      }
    }
}
