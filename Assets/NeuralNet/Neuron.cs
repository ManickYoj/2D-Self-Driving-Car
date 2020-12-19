using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Neuron : Readable {
  public Synapse[] inputs;
  public float bias;

  byte cacheKey;
  float cachedValue;

  public Neuron() {
    this.bias = UnityEngine.Random.Range(-1f, 1f);
  }

  public float Read(byte cacheKey) {
    if (cacheKey == this.cacheKey) return this.cachedValue;
    this.cacheKey = cacheKey;

    float sum = this.bias;

    foreach(Synapse s in inputs) {
      sum += s.Read(cacheKey);
    }

    this.cachedValue = Activate(sum);
    return this.cachedValue;
  }

  protected virtual float Activate(float input) {
    return (float) Math.Tanh(input);
  }

  // public List<float> Encode() {
  //   List<float> encoded = new List<float>() { bias };

  //   foreach(IEncodable e in (IEncodable[]) inputs) {
  //     encoded.AddRange(e.Encode());
  //   }

  //   return encoded;
  // }

  // public void Decode(List<float> encoded) {
  //   this.bias = encoded[0];
  //   encoded.RemoveAt(0);

  //   foreach(IDecodable input in inputs) {
  //     input.Decode(encoded);
  //   }
  // }
}

public class OutputNeuron : Neuron {
  Actuator actuator;

  public OutputNeuron(Actuator actuator) : base() {
    this.actuator = actuator;
  }

  public void Calculate(byte cacheValue) {
    this.actuator.Set(this.Read(cacheValue));
  }

  // protected override float Activate(float input) {
  //   // For outputs, return a value between 0 and 1
  //   // Otherwise the value would be between -1 and 1
  //   return (base.Activate(input) + 1) * 0.5f;
  // }
}