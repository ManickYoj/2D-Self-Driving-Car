using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour {

  public FitnessEvaluator target;

  [SerializeField]
  View lifespanView;
  [SerializeField]
  View distanceView;
  [SerializeField]
  View speedView;
  [SerializeField]
  View fitnessView;
  [SerializeField]
  GameObject crashPanel;

  // Update is called once per frame
  void OnGUI() {
    if (target != null) {
      try {
        speedView.Display(target.RollingAvgSpeed());
      } catch (NotEnoughDataException e) {
        speedView.Clear();
      }

      lifespanView.Display(target.Lifespan());
      distanceView.Display(target.ForwardDistance());
      fitnessView.Display(target.Evaluate());
      crashPanel.SetActive(target.crashed);
    }
  }

  public float SortOrder() {
    return target.Evaluate();
  }
}
