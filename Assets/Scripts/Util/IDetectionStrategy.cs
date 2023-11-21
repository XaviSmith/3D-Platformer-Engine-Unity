using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils; 


public interface IDetectionStrategy
{
    bool Execute(Transform target, Transform detector, CountdownTimer timer);
}

