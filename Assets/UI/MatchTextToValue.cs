using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchTextToValue : MonoBehaviour {
  Text textBox = null;

  public void Start() {
    this.textBox = GetComponent<Text>();
  }

  public void SetText(float value) {
    textBox.text = value.ToString();
  }
}
