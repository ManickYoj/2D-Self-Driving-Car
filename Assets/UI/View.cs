using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour {
  private Text textBox;

  void Start() {
    this.textBox = GetComponent<Text>();
    Clear();
  }

  public void Display(float input) {
    this.textBox.text = Math.Round(input, 2).ToString();
  }

  public void Clear() {
    this.textBox.text = "--";
  }
}
