using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean;

public class AnimalControl : MonoBehaviour {

    private const int MAX_AMOUNT_OF_ANIMALS = 20;
    private const float ANIMAL_SPAWN_TIME = 0.5f;
    private const int MIN_DISTANCE_AT_SPAWN = 15;
    private const int MAX_DISTANCE_AT_SPAWN = 35;

    [SerializeField]
    private Animal[] _animals;

    public List<WeighedItem<Animal>> animals = new List<WeighedItem<Animal>>();

    public List<Animal> AliveAnimals { get; private set; }

    public static AnimalControl Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            AliveAnimals = new List<Animal>();
            InitializeAnimals();
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            int rabbits = 0;
            int deer = 0;
            int bears = 0;
            foreach(Animal animal in AliveAnimals) {
                if(animal.name == "Rabbit")
                    rabbits++;
                if(animal.name == "AdultFemaleDeer")
                    deer++;
                if(animal.name == "AdultBear")
                    bears++;
            }
            Debug.Log("Rabbits: " + rabbits + ". Deer: " + deer + ". Bears: " + bears);
        }
    }

    private void InitializeAnimals() {
        foreach(Animal animal in _animals) {
            WeighedItem<Animal> newAnimal = new WeighedItem<Animal>(animal, animal.spawnWeight);
            animals.Add(newAnimal);
        }
    }

    public IEnumerator SpawnAnimals() {
        while(true) {
            if (AliveAnimals.Count >= MAX_AMOUNT_OF_ANIMALS) {
                yield return new WaitForSeconds(ANIMAL_SPAWN_TIME);
                continue;
            }

            Animal animalToSpawn = WeighedItem<Animal>.Choose(animals);
            Lean.Pool.LeanPool.Spawn(animalToSpawn.gameObject, GetSpawnPosition(), Quaternion.identity);
            
            yield return new WaitForSeconds(ANIMAL_SPAWN_TIME);
        }
    }

    private Vector3 GetSpawnPosition() {
        Vector3 playerPos = Player.Instance.transform.position;

        Vector3Int possibleSpawn = GetRandomSpawn(playerPos);
        if (Vector3.Distance(possibleSpawn, playerPos) < MIN_DISTANCE_AT_SPAWN || !WorldGenerator.Instance.IsWalkableTile(possibleSpawn)) {
            while(Vector3.Distance(possibleSpawn, playerPos) < MIN_DISTANCE_AT_SPAWN || !WorldGenerator.Instance.IsWalkableTile(possibleSpawn)) {
                possibleSpawn = GetRandomSpawn(playerPos);
            }
        }
        return possibleSpawn;
    }

    private Vector3Int GetRandomSpawn(Vector3 playerPos) {
        return new Vector3Int(Random.Range((int) playerPos.x - MAX_DISTANCE_AT_SPAWN, (int) playerPos.x + MAX_DISTANCE_AT_SPAWN), 
            Random.Range( (int)playerPos.y - MAX_DISTANCE_AT_SPAWN, (int) playerPos.y + MAX_DISTANCE_AT_SPAWN), 0);
    }
}
