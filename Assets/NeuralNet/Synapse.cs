using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Synapse : Readable {
  Readable prev;
  public float weight;

  byte cacheKey;
  float cachedValue;

  public Synapse(Readable prev) {
    this.prev = prev;
    this.weight = UnityEngine.Random.Range(-1f, 1f);
  }

  public float Read(byte cacheKey) {
    // If we haven't seen this key before, recalculate value and store it.
    if (cacheKey == this.cacheKey) return this.cachedValue;
    this.cacheKey = cacheKey;

    this.cachedValue = this.prev.Read(cacheKey) * this.weight;
    return this.cachedValue;
  }

  // public List<float> Encode() {
  //   return new List<float>() { weight };
  // }

  // public void Decode(List<float> encoded) {
  //   this.weight = encoded[0];
  //   encoded.RemoveAt(0);
  // }
}
