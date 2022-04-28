using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateMachine : BaseUnit
{
    private BattleStateMachine BSM;

    // Hero turn states
    public enum TurnState
    {
        Preparing,
        Choosing,
        Idle,
        Acting,
        Dead
    }
    public TurnState turnState;

    // For the preparing phase
    private float elapsedCooldown = 0f;
    private float turnCooldown = 5f;

    // For the acting phase
    private bool actionStarted = false;
    private float animationSpeed = 15f;
    private Vector3 startPosition;
    public GameObject objectToAttack;

    // Alive check
    private bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
