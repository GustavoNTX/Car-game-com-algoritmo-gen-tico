using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    public GameObject[] carPrefabs;
    public int populationSize = 10;
    public float generationTime = 15f;
    public float mutationRate = 0.05f;
    public Vector3 carScale = new Vector3(0.5f, 0.5f, 0.5f);

    private List<GameObject> cars = new List<GameObject>();
    private int generation = 1;
    private int carsAlive;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CreateInitialPopulation();
        StartCoroutine(EvolvePopulation());
    }

    void CreateInitialPopulation()
    {
        carsAlive = populationSize;
        for (int i = 0; i < populationSize; i++)
        {
            GameObject car = SpawnCar();
            car.GetComponent<CarBrain>().GenerateRandomGenes();
            cars.Add(car);
            LapManager.instance.CarroEntrouNaCorrida(car);
        }
    }

    IEnumerator EvolvePopulation()
    {
        while (true)
        {
            float timeLeft = generationTime;

            while (timeLeft > 0 && carsAlive > 0)
            {
                yield return new WaitForSeconds(1f);
                timeLeft -= 1f;
            }

            List<CarBrain> sortedCars = cars
                .Select(c => c.GetComponent<CarBrain>())
                .OrderByDescending(c => c.GetFitness())
                .ToList();

            Debug.Log($"Geração {generation} concluída! Melhor fitness: {sortedCars[0].GetFitness()}");

            LapManager.instance.CarroCompletouVolta(sortedCars[0].gameObject, sortedCars[0].GetFitness());

            List<float[]> newGenes = GenerateNewPopulationGenes(sortedCars);

            DestroyOldGeneration();
            CreateNewGeneration(newGenes);

            generation++;
        }
    }

    List<float[]> GenerateNewPopulationGenes(List<CarBrain> sortedCars)
    {
        List<float[]> newGenes = new List<float[]>();

        newGenes.Add(sortedCars[0].genes);
        newGenes.Add(sortedCars[1].genes);

        int eliteCount = Mathf.Max(2, populationSize / 2);
        for (int i = 2; i < populationSize; i += 2)
        {
            float[] parentA = sortedCars[i % eliteCount].genes;
            float[] parentB = sortedCars[(i + 1) % eliteCount].genes;

            (float[] child1, float[] child2) = CrossoverUniform(parentA, parentB);
            Mutate(child1);
            Mutate(child2);

            newGenes.Add(child1);
            newGenes.Add(child2);
        }

        return newGenes;
    }

    void DestroyOldGeneration()
    {
        foreach (GameObject car in cars)
        {
            LapManager.instance.CarroMorreu(car, "nova geração");
            Destroy(car);
        }
        cars.Clear();
    }

    void CreateNewGeneration(List<float[]> newGenes)
    {
        carsAlive = populationSize;
        for (int i = 0; i < populationSize; i++)
        {
            GameObject car = SpawnCar();
            car.GetComponent<CarBrain>().genes = newGenes[i];
            cars.Add(car);
            LapManager.instance.CarroEntrouNaCorrida(car);
        }
    }

    GameObject SpawnCar()
    {
        if (carPrefabs.Length == 0)
        {
            Debug.LogError("Nenhum prefab de carro foi atribuído ao PopulationManager!");
            return null;
        }

        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(prefab, transform.position, Quaternion.identity);
        car.transform.localScale = carScale;
        return car;
    }

    (float[], float[]) CrossoverUniform(float[] parentA, float[] parentB)
    {
        float[] child1 = new float[parentA.Length];
        float[] child2 = new float[parentA.Length];

        for (int i = 0; i < parentA.Length; i++)
        {
            if (Random.value > 0.5f)
            {
                child1[i] = parentA[i];
                child2[i] = parentB[i];
            }
            else
            {
                child1[i] = parentB[i];
                child2[i] = parentA[i];
            }
        }
        return (child1, child2);
    }

    void Mutate(float[] genes)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                genes[i] = Random.Range(-1f, 1f);
            }
        }
    }

    public void ReportDeath(CarBrain car)
    {
        carsAlive--;
        LapManager.instance.CarroMorreu(car.gameObject, "bateu na parede");
    }
}