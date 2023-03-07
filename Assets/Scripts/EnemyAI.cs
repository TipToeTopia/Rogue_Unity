using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyAI : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent navmeshAgent;

    [SerializeField]
    private float patrolRange = 3.0f;

    private float patrolTimer = 0.0f;

    [SerializeField]
    private float waitTimer = 2.0f;

    private PlayerMovement targetInstance;

    private float targetRange;
    private float visionAngle;

    private const float COROUTINE_DELAY = 0.5f;

    private const int AREA_MASK = -1;
    private const int MINIMUM_PLAYERS_IN_SESSION = 1;

    private bool isDetecting = false;
    private bool isAware = false;

    private float afterDetectionTime = 2.0f;
    private float afterDetectionReset = 0.0f;

    private RaycastHit Hit;

    private LevelManager levelManager;
    List<PlayerMovement> PlayersInGame;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    private void Start()
    {
  
        levelManager.allPlayersDead.OnValueChanged += levelManager.YouLose;

        targetRange = levelManager.enemyTargetRange;
        visionAngle = levelManager.enemyVisionAngle;

        navmeshAgent.speed = levelManager.enemySpeed;

    }

    void Update()
    {
        // entire logic is based around finding nearest player to down, else AI will randomly patrol

        StartCoroutine(FindClosestTarget());

        if (!targetInstance)
        {
            RandomPatrol();
            return;
        }

        if (isAware == true) // chase player
        {
            navmeshAgent.SetDestination(targetInstance.transform.position);

            if (isDetecting == false) 
            {
                afterDetectionReset += Time.deltaTime;
                if (afterDetectionReset >= afterDetectionTime) 
                {
                    isAware = false;
                    afterDetectionReset = 0; 
                }
            }
        }
        else
        {
            RandomPatrol();
        }

        ConeOfVision();
    }

    private void ConeOfVision()
    {
        // if within area of sector, is not downed and there is no obstacle in the way? 

        if ((targetInstance.isDowned == false) &&
           (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(targetInstance.transform.position)) < visionAngle) &&
           (Vector3.Distance(this.transform.position, targetInstance.transform.position) < targetRange) &&
           (Physics.Linecast((transform.position + transform.up), (targetInstance.transform.position + transform.up), out Hit, AREA_MASK)) &&
           (Hit.transform.CompareTag(LevelManager.PLAYER_TAG)))
        {
            isAware = true;
            isDetecting = true;
            afterDetectionReset = 0;
        }
        else
        {
            isDetecting = false;
        }
    }

    // obtain random point around AI origin

    public Vector3 RandomPoint()
    {
        Vector3 RandomPoint = (Random.onUnitSphere * patrolRange) + this.transform.position;
        NavMeshHit Hit;
        NavMesh.SamplePosition(RandomPoint, out Hit, patrolRange, AREA_MASK);
        return new Vector3(Hit.position.x, this.transform.position.y, Hit.position.z);
    }

    // find the closest player to set as target

    IEnumerator FindClosestTarget()
    {
        yield return new WaitForSeconds(COROUTINE_DELAY);

        PlayersInGame = levelManager.PlayersInGame;
        Transform ClosestTarget = null;
        float MaximumDistance = Mathf.Infinity;

        for (int I = 0; I < PlayersInGame.Count; I++)
        {
            // has player left the game, if not we run closest distance updater
            if (PlayersInGame[I])
            {
                float TargetDistance = Vector3.Distance(this.transform.position, PlayersInGame[I].transform.position);

                if (TargetDistance < MaximumDistance)
                {
                    if (PlayersInGame.Count > MINIMUM_PLAYERS_IN_SESSION)
                    {
                        if (PlayersInGame[I].isDowned == false)
                        {
                            targetInstance = PlayersInGame[I];
                            ClosestTarget = PlayersInGame[I].transform;
                            MaximumDistance = TargetDistance;
                        }
                        else
                        {
                            targetInstance = null;
                        }
                    }
                }
            }
            
        }

        if (targetInstance && ClosestTarget)
        {
            targetInstance.transform.position = ClosestTarget.transform.position;
        }
    }

    private void RandomPatrol()
    {
        if (navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime; // timer begins

            if (patrolTimer >= waitTimer)
            {
                Vector3 Point = RandomPoint();

                navmeshAgent.SetDestination(Point);

                patrolTimer = 0.0f; // reset timer after each destination
            }
        }
        else
        {
            patrolTimer = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider Other)
    {
        if (Other.CompareTag(LevelManager.PLAYER_TAG))
        {
            if (targetInstance)
            {
                targetInstance.isDowned = true;

                isAware = false;
                isDetecting = false;

                targetInstance.EnableOrDisableReviveUIClientRpc(true);

                LevelManager.Instance.AreAllPlayersDead();

            }
        }
    }
}
