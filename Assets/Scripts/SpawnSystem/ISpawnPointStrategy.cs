using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// How we actually spawn things in (randomly, or linearly)
    /// </summary>
    public interface ISpawnPointStrategy 
    {
        Transform NextSpawnPoint();
    }

}

