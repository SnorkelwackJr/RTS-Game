using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AgentBehavior
{
    //run away from the target
    public override Steering GetSteering()
    {
        Steering steer = new Steering();
        steer.linear = transform.position - target.transform.position;
        steer.linear.Normalize();
        steer.linear = steer.linear * agent.maxAccel;
        return steer;
    }
}
