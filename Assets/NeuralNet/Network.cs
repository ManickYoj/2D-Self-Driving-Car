using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Network : MonoBehaviour, IEncodable, IDecodable {
  [SerializeField]
  int hiddenLayerCount = 1;

  [SerializeField]
  Sensor[] inputs = null;

  [SerializeField]
  Actuator[] outputs = null;

  // Output neurons map 1:1 with the output actuators
  OutputNeuron[] outputLayer;

  Neuron[][] hiddenLayers;

  // Used as a cache key. Wraps at its max value (255) back to 0
  // Start at 255 to makes sure values are recalculated on first tick
  byte tick = 255;

  void Start() {
    this.Setup();
    // Debug.Log(String.Join(",", this.Encode()));
  }

  void Update() {
    Calculate();
  }

  public void Setup() {
    // In theory a good rule-of-thumb for the number of hidden nodes in a
    // neural net is about 2/3 the input nodes + the number of output nodes
    int neuronsPerHiddenLayer = (int) (0.66f * inputs.Length + outputs.Length);

    // Initialize hidden layers
    this.hiddenLayers = new Neuron[hiddenLayerCount][];
    for(int i = 0; i < hiddenLayers.Length; i++) {
      this.hiddenLayers[i] = new Neuron[neuronsPerHiddenLayer];

      for(int j = 0; j < hiddenLayers[i].Length; j++) {
        hiddenLayers[i][j] = new Neuron();
      }
    }

    // Initialize output layer
    this.outputLayer = new OutputNeuron[outputs.Length];
    for(int i = 0; i < outputLayer.Length; i++) {
      outputLayer[i] = new OutputNeuron(outputs[i]);
    }

    // Link up all the layers with synapses
    Readable[] prevLayer = (Readable[]) inputs;

    for(int layerInd = 0; layerInd < hiddenLayers.Length; layerInd++) {
      Neuron[] nextLayer = hiddenLayers[layerInd];
      this.ConnectWithSynapses(prevLayer, nextLayer);
      prevLayer = (Readable[]) nextLayer;
    }

    this.ConnectWithSynapses(prevLayer, (Neuron[]) outputLayer);
  }

  public void Calculate() {
    foreach(OutputNeuron outputNeuron in this.outputLayer) {
      outputNeuron.Calculate(this.tick);
    }

    this.tick++;
  }

  private void ConnectWithSynapses(Readable[] prevLayer, Neuron[] nextLayer) {
    for (int i = 0; i < nextLayer.Length; i++) {
      Neuron neuron = nextLayer[i];
      neuron.inputs = new Synapse[prevLayer.Length];

      for (int j = 0; j < prevLayer.Length; j++) {
        neuron.inputs[j] = new Synapse(prevLayer[j]);
      }
    }
  }

  // Shared memory for encode to avoid reallocating MB
  // of memory hundreds of time per generation
  // This saves legitimately about a GB of memory allocations
  // per generation on a generation of 150 cars
  private static float[] encodedNetwork;

  public float[] Encode() {
    int hiddenNeuronsPerLayer;
    int outputSynapseCount;
    int hiddenLayerNeuronCount;
    int hiddenSynapseCount;
    int inputSynapseCount;

    if (hiddenLayers.Length == 0) {
      hiddenNeuronsPerLayer = 0;
      outputSynapseCount = outputLayer.Length * inputs.Length;
      hiddenLayerNeuronCount = 0;
      hiddenSynapseCount = 0;
      inputSynapseCount = 0; // Same as output Synapses
    } else {
      hiddenNeuronsPerLayer = hiddenLayers[0].Length;
      outputSynapseCount = outputLayer.Length * hiddenNeuronsPerLayer;
      hiddenLayerNeuronCount = hiddenLayerCount * hiddenNeuronsPerLayer;
      hiddenSynapseCount = (hiddenLayerCount - 1) * hiddenNeuronsPerLayer * hiddenNeuronsPerLayer;
      inputSynapseCount = inputs.Length * hiddenNeuronsPerLayer;
    }

    int outputNeuronCount = outputLayer.Length;


    int totalGeneCount = (
      outputNeuronCount +
      outputSynapseCount +
      hiddenLayerNeuronCount +
      hiddenSynapseCount +
      inputSynapseCount
    );

    // Reuse the preallocated memory if it's the right size
    if (Network.encodedNetwork == null || (Network.encodedNetwork.Length != totalGeneCount)) {
      Network.encodedNetwork = new float[totalGeneCount];
    }

    int i = 0;
    foreach(Neuron n in this.outputLayer) {
      Network.encodedNetwork[i++] = n.bias;
      foreach(Synapse s in n.inputs) {
        Network.encodedNetwork[i++] = s.weight;
      }
    }

    foreach(Neuron[] layer in this.hiddenLayers) {
      foreach(Neuron n in layer) {
        Network.encodedNetwork[i++] = n.bias;

        foreach(Synapse s in n.inputs) {
          Network.encodedNetwork[i++] = s.weight;
        }
      }
    }

    // Arrays are referenced. If we use the static copy, all returned values
    // will reference the last in-memory run. Copy instead, but be aware this
    // takes lots of memory
    float[] returnCopy = new float[totalGeneCount];
    Array.Copy(Network.encodedNetwork, returnCopy, totalGeneCount);
    return returnCopy;
  }

  public void Decode(float[] data) {
    int i = 0;
    foreach(Neuron n in this.outputLayer) {
      n.bias = data[i++];

      foreach(Synapse s in n.inputs) {
        s.weight = data[i++];
      }
    }

    foreach(Neuron[] layer in this.hiddenLayers) {
      foreach(Neuron n in layer) {
        n.bias = data[i++];

        foreach(Synapse s in n.inputs) {
          s.weight = data[i++];
        }
      }
    }
  }

  // public List<float> Encode() {
  //   List<float> encoded = new List<float>();

  //   foreach(Neuron[] layer in hiddenLayers) {
  //     foreach(IEncodable neuron in layer) {
  //       encoded.AddRange(neuron.Encode());
  //     }
  //   }

  //   foreach(IEncodable neuron in outputLayer) {
  //     encoded.AddRange(neuron.Encode());
  //   }

  //   return encoded;
  // }

  // public void Decode(List<float> encoded) {
  //   foreach(Neuron[] layer in hiddenLayers) {
  //     foreach(IDecodable neuron in layer) {
  //       neuron.Decode(encoded);
  //     }
  //   }

  //   foreach(IDecodable neuron in outputLayer) {
  //     neuron.Decode(encoded);
  //   }
  // }
}

public interface IEncodable {
  float[] Encode();
}

public interface IDecodable {
  void Decode(float[] encoded);
}