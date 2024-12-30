using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponStats stats;

    //let's just make them directly readable, we need it all anyways
    public WeaponStats Stats => stats;
    
    //We may need to modify things such as our colours. That's why we should be a seperate object.

}
