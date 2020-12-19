using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessEvaluator : MonoBehaviour {
  // A+ naming about to happen
  public Track track;

  public Guid guid;

  [SerializeField]
  float startTime;
  [SerializeField]
  Vector3 startingPosition;
  [SerializeField]
  Quaternion startingRotation;

  [SerializeField]
  Collider2D firstCheckpoint;
  [SerializeField]
  Collider2D lastCheckpoint;
  [SerializeField]
  Collider2D prevCheckpoint; // Checkpoint before last
  [SerializeField]
  int waypoints = 0;
  Collider2D nextCheckpoint;

  [SerializeField]
  int lapNum = 0;

  public int generation = 0;

  [SerializeField]
  public Network network;

  public float mutationChance = 0.1f;
  public float mutationMagnitue = 1f;

  private static KeyValuePair<float, Chromosome> fittest;
  private static KeyValuePair<float, Chromosome> lastFittest;

  public bool crashed = false;
  public float crashTime = 0;

  [SerializeField]
  float fitnessAtCrash = 0;

  [SerializeField]
  float distanceAtCrash = 0;

  [SerializeField]
  int secondsOfRecordsToKeep = 5;

  [SerializeField]
  float minSpeedLimit = 0.2f;

  private float nextDistanceRecordTime = 0;
  private CircularBuffer<float> distances;

  private Rigidbody2D rbody;

  void Start() {
    rbody = this.GetComponent<Rigidbody2D>();
    distances = new CircularBuffer<float>(secondsOfRecordsToKeep);
    this.startingPosition = transform.position;
    this.startingRotation = transform.rotation;
    this.guid = Guid.NewGuid();

    Reset();
  }

  public void Reset() {
    rbody.constraints = RigidbodyConstraints2D.None;
    this.startTime = Time.time;
    transform.position = this.startingPosition;
    transform.rotation = this.startingRotation;

    crashed = false;
    crashTime = 0;
    fitnessAtCrash = 0;
    distanceAtCrash = 0;

    firstCheckpoint = null;
    lastCheckpoint = null;
    prevCheckpoint = null;
    nextCheckpoint = null;
    lapNum = 0;
    waypoints = 0;
    generation++;

    this.nextDistanceRecordTime = 0;
    this.distances.Clear();
  }

  void Update() {
    // Update distances for rolling avg speed
    float lifespan = Lifespan();
    if(lifespan > this.nextDistanceRecordTime) {
      this.distances.Add(ForwardDistance());
      this.nextDistanceRecordTime = lifespan + 1f;
    }

    // Kill off this car if it's been going too slowly
    if (!crashed) {
      try {
        if (RollingAvgSpeed() < this.minSpeedLimit) {
          Crash();
        }
      } catch (NotEnoughDataException e) {}
    }
  }

  public void Crash() {
    rbody.constraints = RigidbodyConstraints2D.FreezeAll;
    distanceAtCrash = ForwardDistance();
    fitnessAtCrash = Evaluate();
    crashTime = Time.time;
    crashed = true;
    Reincarnate();
  }

  public void Reincarnate() {
    Chromosome child = Chromosome.Combine(
      FitnessEvaluator.lastFittest.Value,
      FitnessEvaluator.fittest.Value
    );

    child.Mutate(this.mutationChance, this.mutationMagnitue);

    this.network.Decode(child.data);
    this.Reset();
  }

  public float Lifespan() {
    if (!crashed) {
      return Time.time - startTime;
    } else {
      return crashTime - startTime;
    }
  }

  public float AverageSpeed() {
    float fwdDistance = ForwardDistance();
    float duration = Lifespan();
    return fwdDistance / duration;
  }

  public float RollingAvgSpeed() {
    float[] temp = this.distances.GetAll();
    if (temp.Length < 4) throw new NotEnoughDataException();
    float distanceTravelled = temp[temp.Length - 1] - temp[0];
    return distanceTravelled / temp.Length; // One record is taken (approximately every second)
  }

  public string Id() {
    return this.guid.ToString() + generation.ToString();
  }

  public float Evaluate() {
    if (!crashed) {
      float fitness = ForwardDistance() * ForwardDistance();
      //  * Mathf.Sqrt(1 + AverageSpeed())

      if (FitnessEvaluator.fittest.Equals(default(KeyValuePair<float, Chromosome>))) {
        FitnessEvaluator.fittest = new KeyValuePair<float, Chromosome>(
          fitness, new Chromosome(this.network, Id())
        );
        FitnessEvaluator.lastFittest = FitnessEvaluator.fittest;
      }

      if (fitness > FitnessEvaluator.fittest.Key) {
        // If we're already the fittest, don't recreate the chromosome,
        // Just update the fitness
        Chromosome chromosomeToSet;
        if (FitnessEvaluator.fittest.Value.id == Id()) {
          chromosomeToSet = FitnessEvaluator.fittest.Value;
        } else {
          chromosomeToSet = new Chromosome(this.network, Id());
          FitnessEvaluator.lastFittest = FitnessEvaluator.fittest;
        }
        FitnessEvaluator.fittest = new KeyValuePair<float, Chromosome>(fitness, chromosomeToSet);
      }


      return fitness;
    } else {
      return fitnessAtCrash;
    }
  }

  public float ForwardDistance() {
    if (!crashed) {
      // If we're going backwards, zero us out
      if (
        prevCheckpoint != null &&
        lastCheckpoint != null &&
        !track.IsForward(prevCheckpoint, lastCheckpoint)
      ) return 0f;

      return track.Distance(
        this.startingPosition,
        this.transform.position,
        this.firstCheckpoint,
        this.lastCheckpoint,
        this.lapNum
      );
    } else {
      return distanceAtCrash;
    }
  }

  private void OnTriggerEnter2D(Collider2D other){
    if (other.gameObject.tag == "Checkpoint"){
      if(firstCheckpoint == null) firstCheckpoint = other;
      if(lastCheckpoint == null) lastCheckpoint = other;
      if(nextCheckpoint == null) {
        nextCheckpoint = track.NextCheckpoint(lastCheckpoint);
      } else {
        if (other != nextCheckpoint) {
          // Kill off cars that go backwards
          Crash();
          this.fitnessAtCrash = 0;
        }
      }

      if(nextCheckpoint == other) {
        waypoints++;
        if(other == firstCheckpoint && lastCheckpoint != firstCheckpoint) {
          lapNum += 1;
        }

        lastCheckpoint = other;
        prevCheckpoint = lastCheckpoint;
        nextCheckpoint = track.NextCheckpoint(lastCheckpoint);
      }
    }
  }

  void OnCollisionEnter2D(Collision2D other){
    if (other.collider.gameObject.tag == "Wall"){
      Crash();
    }
  }
}

class NotEnoughDataException : Exception {
  public NotEnoughDataException() : base( "Not enough data." ) {}
}
