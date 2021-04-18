using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshFileListing : MonoBehaviour {
  public GameObject targetView;
  public Trainer trainer;
  public GameObject objToDeactivateOnLoad;

  public FileRow rowPrefab;
  private List<FileRow> rows;

  void OnEnable() {
    Refresh();
  }

  public void Refresh() {
    string[] filenames = Directory.GetFiles(Application.persistentDataPath);

    // Create and activate the right number of rows with as much efficiency
    // as reasonably possible
    SyncRows(filenames.Length);

    for(int i = 0; i < filenames.Length; i++) {
      rows[i].SetFilename(filenames[i]);
    }
  }

  /*
  Create or deactivate rows according to if there are more or fewer than we
  need. Avoids uneccessary row creation and deletion, which would take longer
  and cause heap fragmentation for no good reason.
  */
  private void SyncRows(int rowsNeeded) {
    if (rows == null) {
      Debug.Log("Reinitializing List!");
      rows = new List<FileRow>();
    }

    if (rows.Count > rowsNeeded) {
      for(int i = rowsNeeded; i < rows.Count; i++) {
        rows[i].gameObject.SetActive(false);
      }
    }

    if (rows.Count < rowsNeeded) { // If we don't have enough rows
      for(int i = rows.Count; i < rowsNeeded; i++) {
        FileRow newRow = Instantiate(rowPrefab, targetView.transform).GetComponent<FileRow>();
        newRow.loadFileMethod = trainer.LoadFile;
        newRow.closeFileDelegate = delegate() { objToDeactivateOnLoad.SetActive(false); };
        rows.Add(newRow);
      }

      for(int i = 0; i < rowsNeeded; i++) {
        rows[i].gameObject.SetActive(true);
      }
    }
  }
}
