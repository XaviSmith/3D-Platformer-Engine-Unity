using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums { }

    [System.Serializable]
    public enum Events
    {
        TEST,
        DAMAGE,
        UPDATEPLAYERHEALTH,
        UPDATESCORE,
        GETSTAR,
        GETCOIN,
        UPDATESTAR,
        UPDATECOIN,
        PLAYERDIE
    }

[System.Serializable]
public enum ColliderState { INACTIVE, ACTIVE, COLLIDING }

