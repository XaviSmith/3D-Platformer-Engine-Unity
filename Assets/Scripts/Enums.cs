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
        PLAYERDIE,
        ENDGAME,
        PAUSED,
        UNPAUSED
    }

[System.Serializable]
public enum ColliderState { INACTIVE, ACTIVE, COLLIDING }

public enum GemColours { BRONZE, SILVER, GOLD, PLAT}

