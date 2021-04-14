using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWeighted<T> {
  protected KeyValuePair<double, T>[] itemArr;
  protected int head = 0;
  protected double cumSum = 0;

  public RandomWeighted(int size) {
    itemArr = new KeyValuePair<double, T>[size];
  }

  public virtual void Add(T item, double weight) {
    cumSum += weight;
    itemArr[head++] = new KeyValuePair<double, T>(cumSum, item);
  }

  // O(n). Not the most efficient. I'll improve it if it's an issue
  public T Get() {
    float selector = UnityEngine.Random.Range(0, (float) cumSum);

    for(int i = 0; i < head; i++) {

      if (itemArr[i].Key > selector) {
        return itemArr[i].Value;
      }
    }

    throw new NotEnoughDataException();
  }
}

public class RandomWeightedExp<T> : RandomWeighted <T> {
  protected double exponent;

  public RandomWeightedExp (int size, double exponent = 2) : base(size){
    this.exponent = exponent;
  }

  public override void Add(T item, double weight) {
    base.Add(item, Math.Pow(weight, this.exponent));
  }
}