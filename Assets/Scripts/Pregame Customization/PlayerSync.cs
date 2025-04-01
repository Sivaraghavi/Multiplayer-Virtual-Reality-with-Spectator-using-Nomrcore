using UnityEngine;
using Normal.Realtime;

[RealtimeModel]
public partial class PlayerModel
{
    [RealtimeProperty(1, true, true)] private string _playerName;
}

public class PlayerSync : RealtimeComponent<PlayerModel>
{
    public string PlayerName
    {
        get => model.playerName; 
        set => model.playerName = value;
    }

    void Start()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
    }
}