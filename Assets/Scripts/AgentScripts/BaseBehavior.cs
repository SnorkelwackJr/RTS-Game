using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehavior : MonoBehaviour
{

    public int team;

    //links to the different behavior components
    //public idle_script idle;
    public SeekScript seek;
    //public attack_script attack;

    //gps is our general pathfinding script
    //public general_pathfinding gps;

    //intelligent movement scripts
    public Agent agentScript;

    public Seek seekScript;
    public Flee fleeScript;

    public float maxSpeed;

    public GameObject target;
    public UnitFSM state;

    public enum UnitFSM //states
    {
        Attack,
        Seek,
        Idle
    }

    // Start is called before the first frame update
    void Start()
    {
        agentScript = gameObject.AddComponent<Agent>(); //add agent
        agentScript.maxSpeed = 10.0f;

        changeState(UnitFSM.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if(state == UnitFSM.Idle)
            {
                changeState(UnitFSM.Seek);
            }
            else
            {
                changeState(UnitFSM.Idle);
            }
        }
    }

    public void changeState(UnitFSM new_state)
    {

        state = new_state;

        switch (new_state)
        {
            case UnitFSM.Idle:

                //if(gameObject.GetComponent<idle_script>() == null)
                //{
                //    idle = gameObject.AddComponent<idle_script>();
                //}
                //DestroyImmediate(seek);
                //DestroyImmediate(attack);

                break;

            case UnitFSM.Seek:

                if (gameObject.GetComponent<SeekScript>() == null)
                {
                    seek = gameObject.AddComponent<SeekScript>();
                }
                //DestroyImmediate(idle);
                //DestroyImmediate(attack);

                break;

            case UnitFSM.Attack:

                //if (gameObject.GetComponent<attack_script>() == null)
                //{
                //    attack = gameObject.AddComponent<attack_script>();
                //}
                //DestroyImmediate(seek);
                //DestroyImmediate(idle);

                break;



        }
    }
}