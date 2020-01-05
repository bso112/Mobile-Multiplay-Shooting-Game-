using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class CharacterSetup : MonoBehaviour
{
    /// <summary>
    /// GameManager의 playerInfo와 대응되는 id(외래키)
    /// </summary>
    public int playerID;
    public int Team { get; protected set; }

    public abstract void SetTeamRPC(int _team);


}
