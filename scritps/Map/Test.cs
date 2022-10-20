using Godot;
using System;

public class Test : Node2D
{
    
    private AnimationPlayer player,bot,map;
    public override void _Ready()
    {
        player = GetNode<AnimationPlayer>("Player/Player/Action");
        bot = GetNode<AnimationPlayer>("Bos/Bos/Action");
        map = GetNode<AnimationPlayer>("AnimationPlayer");
        map.Play("test");
    }
    public void PlayerFencing(){
        player.Play("fencing");
    }
    public void botFencing(){
        bot.Play("fencing");
    }

}
