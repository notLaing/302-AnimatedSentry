using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    CharacterController pawn;
    NavMeshAgent agent;
    Transform navTarget;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2;

        PlayerTargeting player = FindObjectOfType<PlayerTargeting>();
        navTarget = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(navTarget) agent.destination = navTarget.transform.position;
    }
}
