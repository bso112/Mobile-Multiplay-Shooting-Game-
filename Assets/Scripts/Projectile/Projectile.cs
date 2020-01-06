using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{

    protected CharacterStats ownerStats;
    public Transform owner;

    [Header("기본공격에 붙는 추가데미지")]
    [SerializeField]
    protected float damage;



}
