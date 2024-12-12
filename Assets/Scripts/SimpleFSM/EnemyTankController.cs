using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTankController : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Dead }

    public State currentState = State.Patrol;

    public float patrolSpeed = 100.0f;
    public float chaseSpeed = 200.0f;
    public float rotationSpeed = 100.0f;
    public float attackRange = 300.0f;
    public float shootRate = 1.0f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    private Transform player;
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private float shootCooldown = 0.0f;

    void Start()
    {
        // Encontrar al jugador por la etiqueta "Jugador"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Encontrar todos los puntos de patrulla
        GameObject[] wanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        waypoints = new List<Transform>();
        foreach (GameObject wp in wanderPoints)
        {
            waypoints.Add(wp.transform);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("No se encontraron puntos de patrulla con la etiqueta 'WanderPoint'.");
        }
    }

    void Update()
    {
        if (currentState == State.Dead)
        {
            // No hacer nada si está muerto
            return;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }

        // Actualizar el cooldown de disparo
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }
    }

    public void SetState(State newState)
    {
        currentState = newState;
    }

    void Patrol()
    {
        if (waypoints.Count == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position, patrolSpeed);

        // Verificar si llegó al waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance < 100.0f)
        {
            // Ir al siguiente waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        // Verificar si el jugador está dentro del rango de visión
        if (player != null)
        {
            float playerDistance = Vector3.Distance(transform.position, player.position);
            if (playerDistance <= attackRange)
            {
                SetState(State.Chase);
            }
        }
    }

    void Chase()
    {
        if (player == null)
            return;

        MoveTowards(player.position, chaseSpeed);

        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance <= attackRange)
        {
            SetState(State.Attack); // Cambia a Attack si está dentro del rango
        }
        else if (playerDistance > attackRange)
        {
            SetState(State.Patrol); // Regresa a Patrol si el jugador está fuera de rango
        }
    }

    void Attack()
{
    if (player == null) return;

    // Rotar la torreta hacia el jugador
    Vector3 direction = player.position - turret.transform.position;
    Quaternion targetRotation = Quaternion.LookRotation(direction);
    turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    // Disparar si el cooldown lo permite
    if (shootCooldown <= 0)
    {
        Shoot();
        shootCooldown = shootRate;
    }

    // Verificar distancia y cambiar de estado si es necesario
    float playerDistance = Vector3.Distance(transform.position, player.position);
    if (playerDistance > attackRange)
    {
        SetState(State.Patrol); // Regresa a patrullar si el jugador está fuera del rango de ataque
    }
}


    void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    void MoveTowards(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Mantener solo en el plano XZ
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

    // Referencia a la torreta para rotar hacia el jugador
    public GameObject turret;
}
