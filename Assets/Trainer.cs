using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trainer : MonoBehaviour {
  [SerializeField]
  int carsPerGeneration = 100;
  // [SerializeField]
  // int survivorsPerGeneration = 10;
  [SerializeField]
  float mutationRate = 0.1f;
  // [SerializeField]
  float mutationMagnitude = 1f;
  [SerializeField]
  Transform[] spawnPoints= null;

  public Guid guid;
  float averageFitness = 0;
  float bestFitness = 0;


  float startTime;
  int generation = 0;

  [SerializeField]
  GameObject carPrefab = null;

  [SerializeField]
  Track track = null;

  GameObject[] cars = null;
  Network[] networks = null;
  public FitnessEvaluator[] fitnessEvaluators = null;

  void Start() {
    this.guid = Guid.NewGuid();
    Reset();
  }

  void Update() {
    if (Array.TrueForAll(fitnessEvaluators, fitnessEvaluator => fitnessEvaluator.crashed)) {
      Reset();
    }
  }

  public void Reset() {
    this.startTime = Time.time;

    if (cars == null) {
      cars = new GameObject[carsPerGeneration];
      networks = new Network[carsPerGeneration];
      fitnessEvaluators = new FitnessEvaluator[carsPerGeneration];

      for( int i = 0; i < carsPerGeneration; i++) {
        Transform spawnPoint = RandomSpawn();
        cars[i] = (GameObject) Instantiate(
          carPrefab,
          spawnPoint.position,
          spawnPoint.rotation,
          this.transform
        );

        networks[i] = cars[i].GetComponent<Network>();
        fitnessEvaluators[i] = cars[i].GetComponent<FitnessEvaluator>();
        fitnessEvaluators[i].network = networks[i];
        fitnessEvaluators[i].track = track;
      }
    }
    else {
      Chromosome[] babies = MakeBabies(networks, fitnessEvaluators);

      for(int i = 0; i < cars.Length; i++) {
        networks[i].Decode(babies[i].data);

        Transform spawnPoint = RandomSpawn();
        cars[i].transform.position = spawnPoint.position;
        cars[i].transform.rotation = spawnPoint.rotation;

        fitnessEvaluators[i].Reset();
      }

      Chromosome[] chromosomes = new Chromosome[networks.Length];

      for(int i=0; i< networks.Length; i++) {
        chromosomes[i] = new Chromosome(networks[i].Encode());
      }

      Generation g = new Generation(
        guid.ToString(),
        generation,
        chromosomes,
        this.averageFitness,
        this.bestFitness
      );
      g.Save();

      Debug.Log("Generation " + generation + " complete.");
      Debug.Log("Best fitness: " + this.bestFitness);
      Debug.Log("Avg fitness: " + this.averageFitness);
      Debug.Log("-------------------------");

      this.generation++;
    }
  }

  Transform RandomSpawn() {
    return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
  }

  public Chromosome[] MakeBabies(Network[] networks, FitnessEvaluator[] fitnessEvaluators) {
    Chromosome[] children = new Chromosome[networks.Length];
    float totalFitness = 0;
    this.averageFitness = 0;
    this.bestFitness = 0;

    // Create and populate a random weighted distribution for parent selection
    RandomWeighted<Network> parents = new RandomWeightedExp<Network>(networks.Length);
    for(int i = 0; i < networks.Length; i++) {
      float fitness = fitnessEvaluators[i].Evaluate();
      totalFitness += fitness;
      if (fitness > this.bestFitness) this.bestFitness = fitness;
      parents.Add(networks[i], (double) fitness);
    }

    this.averageFitness = totalFitness / networks.Length;

    for(int childNo = 0; childNo < networks.Length; childNo++) {

      // Step 1: Find parents
      // The child always inherits half from the car that spawns it
      Network mother = networks[childNo];
      Network father = parents.Get();

      // Step 2: Combine Parent DNA
      children[childNo] = Chromosome.Combine(
        new Chromosome(mother),
        new Chromosome(father)
      );

      // Step 3: Introduce mutations
      children[childNo].Mutate(this.mutationRate, this.mutationMagnitude);
    }

    return children;
  }
}

public class Generation {
  [SerializeField]
  string runId;

  [SerializeField]
  int generation;

  [SerializeField]
  float avgFitness;

  [SerializeField]
  float bestFitness;

  [SerializeField]
  Chromosome[] chromosomes;

  public Generation(
    string runId,
    int generation,
    Chromosome[] chromosomes,
    float avgFitness,
    float bestFitness
  ) {
    this.runId = runId;
    this.generation = generation;
    this.chromosomes = chromosomes;
    this.avgFitness = avgFitness;
    this.bestFitness = bestFitness;
  }

  public void Save() {
    string filename = (
      "run_" + this.runId +
      "-gen_" + this.generation +
      "-avg_fit_" + this.avgFitness
    );

    string json = JsonUtility.ToJson(this, true);
    FileUtility.SaveJsonString(filename, json);
  }

  public static Generation Load(String filename) {
    string json = FileUtility.LoadJsonString(filename);
    return JsonUtility.FromJson<Generation>(json);
  }
}

[Serializable]
public class Chromosome {

  [SerializeField]
  public string id;

  [SerializeField]
  public float[] data;

  public Chromosome(IEncodable encodable, string id) {
    this.data = encodable.Encode();
    this.id = id;
  }

  public Chromosome(IEncodable encodable) {
    this.data = encodable.Encode();
  }

  public Chromosome(float[] data) {
    this.data = data;
  }

  public static Chromosome Combine(Chromosome first, Chromosome second) {
    float[][] zippedChromosomes = new float[][] {first.data, second.data};
    float[] newChromosomeData = new float[first.data.Length];

    for(int i = 0; i < first.data.Length; i++) {
      int selection = UnityEngine.Random.Range(0, 2); // Returns 0 or 1
      newChromosomeData[i] = zippedChromosomes[selection][i];
    }

    return new Chromosome(newChromosomeData);
  }

  public void Mutate(float chance, float magnitude) {
    for(int i = 0; i < this.data.Length; i++) {
      if (UnityEngine.Random.value < chance) {
        this.data[i] += UnityEngine.Random.Range(-magnitude, magnitude);
        this.data[i] = Mathf.Clamp(this.data[i], -1f, 1f);
      }
    }
  }
}

public static class FileUtility {
  public static void SaveJsonString(string filename, string data) {
    string destination = Path.Combine(Application.persistentDataPath, filename + ".json");

    // Create the filepath if it does not exist.
    DirectoryInfo di = new FileInfo(destination).Directory;
    if (!di.Exists) di.Create();

    File.WriteAllText(destination, data);

    Debug.Log("Saved data to " + destination);
  }

  public static string LoadJsonString(string filename) {
    string destination = Path.Combine(Application.persistentDataPath, filename + ".json");
    string json;

    using (StreamReader r = new StreamReader(destination)) {
      json = r.ReadToEnd();
    }

    return json;
  }
}