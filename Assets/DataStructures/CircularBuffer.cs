using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CircularBuffer<T> {
  private T[] data;
  private int tail = 0;
  private int head = 0;

  public CircularBuffer(int length) {
    data = new T[length];
    tail = 0;
    head = 0;
  }

  private int IncInd(int ind) {
    ind++;
    ind = ind % data.Length;
    return ind;
  }

  private bool IsFull() {
    return head == (tail - 1);
  }

  private bool IsEmpty() {
    return head == tail;
  }

  public void Clear() {
    Array.Clear(data, 0, data.Length);
    head = 0;
    tail = 0;
  }

  public void Add(T datum) {
    data[head] = datum;
    head = IncInd(head);
    if (IsFull()) tail = IncInd(tail);
  }

  // Peeks at end
  public T Get(){
    if (IsEmpty()) throw new IndexOutOfRangeException();
    return data[head - 1];
  }

  public T GetLast() {
    if (IsEmpty()) throw new IndexOutOfRangeException();
    return data[tail];
  }

  public T[] GetAll() {
    List<T> result = new List<T>();

    int i = tail;
    while (i != head) {
      result.Add(data[i]);
      i = IncInd(i);
    }

    return result.ToArray();
  }
}

