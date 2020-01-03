using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class CharacterSetup : MonoBehaviour
{
    public int Team { get; protected set; }

    public abstract void SetTeamRPC(int _team);


}
