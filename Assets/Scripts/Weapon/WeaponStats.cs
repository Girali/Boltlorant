﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Equipment/Weapon")]
public class WeaponStats : ScriptableObject
{
    [Space(40)]
    public float reloadTime = 1f;
    public int magazin = 31;
    public int totalMagazin = 124;
    [Space(10)]
    public int rpm = 80;
    public int dmg = 25;
    public int maxRange = 30;
    [Space(10)]
    public GameObject impactPrefab;
    public GameObject trailPrefab;
}
