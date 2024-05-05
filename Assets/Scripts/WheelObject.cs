using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object", menuName = "WheelObject")]
public class WheelObject : ScriptableObject
{
    public Sprite Icon;
    public string Label;
    public string Rarity;

    [Tooltip("Reward amount")] public float Amount;

    [Tooltip("Probability in %")]
    [Range(0f, 100f)]
    public float Chance = 10f;

    [HideInInspector] public int Index;
    [HideInInspector] public double _weight = 0f;
}
