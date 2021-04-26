using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{

    private AgentState state;
    [Header("Sprites de los estados")]
    [SerializeField] private Sprite susceptibleSprite;
    [SerializeField] private Sprite exposedSprite;
    [SerializeField] private Sprite infectedSprite;
    [SerializeField] private Sprite recoveredSprite;
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    private Vector2 agentVelocity;

    void Awake()
    {
        state = AgentState.SUSCEPTIBLE;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = susceptibleSprite;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void setState(AgentState newState)
    {
        this.state = newState;
        switch(newState)
        {
            case AgentState.SUSCEPTIBLE:
                spriteRenderer.sprite = susceptibleSprite;
                break;
            case AgentState.EXPOSED:
                spriteRenderer.sprite = exposedSprite;
                break;
            case AgentState.INFECTED:
                spriteRenderer.sprite = infectedSprite;
                break;
            case AgentState.RECOVERED:
                spriteRenderer.sprite = recoveredSprite;
                break;
        }
    }

    public void updateAgentState(bool nearSickPeople, float beta, float sigma, float gamma)
    {
        switch (this.state)
        {
            case AgentState.SUSCEPTIBLE:
                //Si el agente estuvo cerca de alguien enfermo entonces
                //se enfermará con una probabilidad beta. 
                if (nearSickPeople)
                {
                    float susceptibleDiceThrow = Random.value;
                    if(susceptibleDiceThrow < beta)
                    {
                        setState(AgentState.EXPOSED);
                    }
                }
                break;

            case AgentState.EXPOSED:
                float exposedDiceThrow = Random.value;
                if(exposedDiceThrow< sigma)
                {
                    setState(AgentState.INFECTED);
                }
                break;

            case AgentState.INFECTED:
                float infectedDiceThrow = Random.value;
                if(infectedDiceThrow < gamma)
                {
                    setState(AgentState.RECOVERED);
                }
                break;

            case AgentState.RECOVERED:
                break;

            default:
                break;
        }
    }

    public void setVelocity(Vector2 velocity)
    {
        this.agentVelocity = velocity;
    }

    public Vector2 getVelocity()
    {
        return agentVelocity;
    }

    public AgentState getState()
    {
        return this.state;

    }
}
