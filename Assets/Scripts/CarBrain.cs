using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarBrain : MonoBehaviour
{
    public float[] genes;
    private int currentGene = 0;
    private float geneTimer = 0f;
    public float decisionTime = 0.2f;
    public bool isAlive = true;

    private Rigidbody2D rb;
    private CarroControle controleCarro;

    private int indexWaypointAtual = 0;
    private float tempoDeCorrida = 0f;
    private float lapTime = 0f;
    private float bestLapTime = Mathf.Infinity;

    public float fitnessPenaltyOnWallHit = 10f;
    public float minSpeedThreshold = 0.1f;
    public float stuckTimeThreshold = 5f;
    private float stuckTime = 0f;
    private Vector3 initialPosition;
    public float timeBeforeReset = 4f;

    private List<Transform> waypoints = new List<Transform>();
    private float timeWithoutWaypoint = 0f;

    public float sensorRange = 2f;
    private bool isReversing = false;
    private bool isObstacleAhead = false;

    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controleCarro = GetComponent<CarroControle>();
        initialPosition = transform.position;

        GameObject aiNode = GameObject.Find("AINode1");
        if (aiNode != null)
        {
            foreach (Transform child in aiNode.transform)
            {
                if (child.CompareTag("WaypointNode"))
                {
                    waypoints.Add(child);
                }
            }
            if (waypoints.Count > 0)
            {
                targetPosition = waypoints[indexWaypointAtual].position;
            }
        }
        else
        {
            Debug.LogError("AINode1 não encontrado!");
        }
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        tempoDeCorrida += Time.deltaTime;

        geneTimer += Time.deltaTime;
        if (geneTimer >= decisionTime && currentGene * 2 + 1 < genes.Length)
        {
            geneTimer = 0f;
            ApplyGene();
            currentGene++;
        }
        if (currentGene * 2 >= genes.Length)
        {
            currentGene = 0;
        }

        CheckObstacles();
        SeguirWaypoint();

        if (rb.linearVelocity.magnitude < minSpeedThreshold)
            stuckTime += Time.deltaTime;
        else
            stuckTime = 0f;

        if (stuckTime >= stuckTimeThreshold)
            Die();

        if (indexWaypointAtual == 0)
            timeWithoutWaypoint += Time.deltaTime;
        else
            timeWithoutWaypoint = 0f;

        if (timeWithoutWaypoint >= timeBeforeReset)
        {
            Die();
            timeWithoutWaypoint = 0f;
        }

        if (indexWaypointAtual == 0 && lapTime > 0)
        {
            if (lapTime < bestLapTime)
            {
                bestLapTime = lapTime;
            }
            lapTime = 0f;
        }
        else
        {
            lapTime += Time.deltaTime;
        }
    }

    void ApplyGene()
    {
        if (currentGene * 2 + 1 < genes.Length)
        {
            float inputMovimento = genes[currentGene * 2];
            float inputRotacao = genes[currentGene * 2 + 1];
            controleCarro.SetInputs(inputMovimento, inputRotacao);
        }
    }

    void SeguirWaypoint()
    {
        if (waypoints.Count == 0)
            return;

        targetPosition = waypoints[indexWaypointAtual].position;

        Vector2 direcao = targetPosition - transform.position;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg - 90f;

        Quaternion rotacaoAlvo = Quaternion.Euler(0, 0, angulo);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoAlvo, 200f * Time.deltaTime);

        float distancia = Vector2.Distance(transform.position, targetPosition);
        float velocidade = isObstacleAhead ? 0.5f : Mathf.Clamp(distancia / 5f, 0.5f, 1f);
        controleCarro.SetInputs(velocidade, 0f);

        if (distancia < 1.5f)
        {
            indexWaypointAtual = (indexWaypointAtual + 1) % waypoints.Count;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            Die();
        }
    }

    void CheckObstacles()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, transform.up, sensorRange);
        RaycastHit2D hitFrontLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * transform.up, sensorRange);
        RaycastHit2D hitFrontRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * transform.up, sensorRange);

        isObstacleAhead = hitFront.collider != null || hitFrontLeft.collider != null || hitFrontRight.collider != null;

        Debug.DrawRay(transform.position, transform.up * sensorRange, hitFront.collider ? Color.red : Color.green);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, 45) * transform.up * sensorRange, hitFrontLeft.collider ? Color.red : Color.green);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -45) * transform.up * sensorRange, hitFrontRight.collider ? Color.red : Color.green);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;

        if (other.CompareTag("WaypointNode"))
        {
            indexWaypointAtual = (indexWaypointAtual + 1) % waypoints.Count;
            timeWithoutWaypoint = 0f;
        }
    }

    void Die()
    {
        isAlive = false;
        PopulationManager.Instance.ReportDeath(this);
    }

    public float GetFitness()
    {
        float waypointBonus = indexWaypointAtual * 20;
        float timePenalty = lapTime * 0.05f;
        float speedBonus = rb.linearVelocity.magnitude;

        if (stuckTime >= stuckTimeThreshold)
        {
            return -1f;
        }

        return waypointBonus - timePenalty + speedBonus;
    }

    public void GenerateRandomGenes()
    {
        genes = new float[10];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = Random.Range(-1f, 1f);
        }
    }
}