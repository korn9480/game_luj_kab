using Godot;
using System;

public class Knife : KinematicBody2D
{
    public Timer cloodown;
    public AudioStreamPlayer2D shoot;
    public bool player_acttack;
    public int speed;
    public Vector2 walk;

    public override void _Ready()
    {
        cloodown = GetNode<Timer>("cloodown");
        shoot = GetNode<AudioStreamPlayer2D>("shoot");
    }
    // ---------------------------- getter --------------------------------------------
    public bool GetMove(){
        return GetNode<Node2D>("../NkaujCoob/move").Scale.x==1;
    }
    public Vector2 GetMyPosition(){
        return GetNode<NkaujCoob>("../NkaujCoob").position;
    }
    public bool getStart(){
        return GetNode<AnimationPlayer>("../NkaujCoob/action").CurrentAnimation=="Knife";
    }
    // ------------------------------------- method --------------------------------------------
    public void MyActtack(){
        if (cloodown.IsStopped()){
            shoot.Play();
            cloodown.Start(0.7f);
            // player_acttack=true;
            
            // followPlayer();
            if (!GetMove()){
                speed=900;
            }
            else {
                speed=-900;
            }
        }
        else if (player_acttack && cloodown.TimeLeft<0.6f){
         
            walk.x=0;
            walk.x+=speed;
            // Visible=true;
            MoveAndSlide(walk);

        }
        else {
            shoot.Stop();
            followPlayer();
        }
    }
    public void followPlayer(){
        Vector2 position=GetMyPosition();
        position.y-=100;
        Position=position;

    }
    public override void _PhysicsProcess(float delta){
        if(getStart() || !cloodown.IsStopped()){
            
            MyActtack();
        }
        else {
            followPlayer();
        }
    }
  
}
