using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWeighted<T> {
  KeyValuePair<double, T>[] itemArr;
  int head = 0;
  double cumSum = 0;

  public RandomWeighted(int size) {
    itemArr = new KeyValuePair<double, T>[size];
  }

  public void Add(T item, double weight) {
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