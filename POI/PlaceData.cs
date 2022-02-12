using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A serializable class to store POI data
[System.Serializable]
public class PlaceData
{
    public int index;
    public string placeName;
    public int enemyType;
    public double latitude;
    public double longitude;
    public bool hasDefeated;
    public bool hasClaimedReward;
    public bool isSpecial;
}
