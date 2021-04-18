using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void VoidStrDelegate (string filepath);

public class FileRow : MonoBehaviour {
  public Text targetText;
  private string fullPath;
  public VoidStrDelegate loadFileMethod;

  public void SetFilename(string name) {
    this.fullPath = name;
    this.targetText.text = Path.GetFileName(name);
  }

  public void LoadFile() {
    if (loadFileMethod != null) {
      loadFileMethod(fullPath);
    } else {
      Debug.LogError("Attempted to load a file, but no file loading delegate was specified.");
    }
  }
}
