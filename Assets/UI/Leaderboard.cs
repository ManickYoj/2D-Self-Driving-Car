using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour {
  public Trainer target;
  public Transform rowContainer;

  [SerializeField]
  private GameObject leaderboardRowPrefab = null;
  private LeaderboardRow[] leaderboardRows = null;

  void OnGUI() {
    if (this.target == null) return;
    SyncRows();
    SortRows();
  }

  void SyncRows() {
    if (target.fitnessEvaluators == null) return;
    if (leaderboardRows == null) {
      this.leaderboardRows = new LeaderboardRow[target.fitnessEvaluators.Length];
    } else if (leaderboardRows.Length == target.fitnessEvaluators.Length) {
      return;
    } else if (leaderboardRows.Length > target.fitnessEvaluators.Length) {
      // Destroy extra objects so they don't hang around after resize
      for(int i = target.fitnessEvaluators.Length; i < leaderboardRows.Length; i++) {
        Destroy(leaderboardRows[i].gameObject);
      }
    }

    Array.Resize<LeaderboardRow>(ref this.leaderboardRows, target.fitnessEvaluators.Length);

    // Resync contents of objects
    for (int i = 0; i < leaderboardRows.Length; i++) {
      if (leaderboardRows[i] == null) {
        GameObject obj = (GameObject) Instantiate(leaderboardRowPrefab, rowContainer);
        this.leaderboardRows[i] = obj.GetComponent<LeaderboardRow>();
      }

      leaderboardRows[i].target = target.fitnessEvaluators[i];
    }
  }

  void SortRows() {
    Array.Sort(leaderboardRows, (row1, row2) => row2.SortOrder().CompareTo(row1.SortOrder()));

    for(int i = 0; i < leaderboardRows.Length; i++) {
      leaderboardRows[i].transform.SetSiblingIndex(i);
    }
  }
}
