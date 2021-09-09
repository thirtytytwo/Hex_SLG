using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager :BaseManagerToMono<RoundManager>{
    private List<BaseActorController> players = new List<BaseActorController>();
    
    private void Start()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(item.GetComponent<BaseActorController>());
        }
    }
    public BaseActorController GetPlayerByPlayers()
    {
        return players[0];
    }
}
