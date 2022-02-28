using Godot;
using System;

public class heroineCheckpoint : Node2D
{
    [Signal]
    public delegate void player_go_or_back(string move);
    public string move="player go";
    public override void _Ready()
    {
    
    }
    public void PlayerBack(Node body){
        move="player back";
        EmitSignal(nameof(player_go_or_back),move);
    }
}