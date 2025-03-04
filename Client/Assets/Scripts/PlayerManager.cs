using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer myPlayer;
    Dictionary<int, Player> players = new();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList playerList)
    {
        Object obj = Resources.Load("Player");
        foreach (var p in playerList.players) 
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if(p.isSelf)
            {
                var myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                this.myPlayer = myPlayer;
            }
            else
            {
                var player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                players.Add(p.playerId, player);
            }            
        }
    }

    public void EnterGame(S_BroadcastEnterGame broadcastEnterGame)
    {
        if(broadcastEnterGame.playerId == myPlayer.PlayerId)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        var player = go.AddComponent<Player>();
        player.PlayerId = broadcastEnterGame.playerId;
        player.transform.position = new Vector3(broadcastEnterGame.posX, broadcastEnterGame.posY, broadcastEnterGame.posZ);
        players.Add(broadcastEnterGame.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame broadcastLeaveGame)
    {
        if(myPlayer.PlayerId == broadcastLeaveGame.playerId)
        {
            GameObject.Destroy(myPlayer.gameObject);
            myPlayer = null;
        }
        else
        {
            if(players.TryGetValue(broadcastLeaveGame.playerId, out var player))
            {
                GameObject.Destroy(player.gameObject);
                players.Remove(broadcastLeaveGame.playerId);
            }
        }
    }

    public void Move(S_BroadcastMove broadcastMove)
    {
        if (myPlayer.PlayerId == broadcastMove.playerId)
        {
            myPlayer.transform.position = new Vector3(broadcastMove.posX, broadcastMove.posY, broadcastMove.posZ);
        }
        else
        {
            if(players.TryGetValue(broadcastMove.playerId, out var player))
            {
                player.transform.position = new Vector3(broadcastMove.posX, broadcastMove.posY, broadcastMove.posZ);
            }
        }
    }
}
