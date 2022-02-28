using Godot;
using System;

public class lao_seeb : KinematicBody2D
{
    [Signal]
    public delegate void get_player_acttack(Boolean player_acttack);
    [Signal]
    public delegate void get_animation_acttack(string animation);
    public int speed;
    public Boolean stopTime=false;
    public AudioStreamPlayer2D sound;
    public Timer cloodown;
    public Vector2 walk = Vector2.Zero;
    public Boolean player_acttack=false;
    public AudioStreamPlayer2D shoot;
    public override void _Ready()
    {
        cloodown=GetNode<Timer>("Timer");
        shoot=GetNode<AudioStreamPlayer2D>("shoot");
    }
    // -----------------------------------รับค่าจาก godot----------------------------------------------
    public Vector2 GetMyPosition(){
        return GetNode<KinematicBody2D>("../Player").Position;
    }
    public Boolean GetMove(){
        return GetNode<Node2D>("../Player/Body").Scale.x==1;
    }
    public Boolean GetPlayerJum(){
        return GetNode<KinematicBody2D>("../Player").IsOnWall();
    }
    public string GetStart(){
        return GetNode<AnimationPlayer>("../Player/Action").CurrentAnimation;
    }
    //---------------------------------------------Method ต่างๆ------------------------------------------
    public Boolean GetPlayerOver(){
        return GetNode<Player>("../Player").over==true;
    }
    public void stopAttack(Node body){
        if (!stopTime){
            stopTime=true;
        }
        else if(stopTime){
            cloodown.Stop();
            stopTime=false;
        }
    }
    public void MyActtack(){
        if (cloodown.IsStopped()){
            SetCollisionLayerBit(1,true);
            cloodown.Start(0.7f); 
            if (!GetMove()){
                player_acttack=true;
                speed=900;
            }
            else {
                player_acttack=true;
                speed=-900;
            }
        }
        else if (!(cloodown.IsStopped()) && cloodown.TimeLeft<0.6f){
            Visible=true;
            walk.x=0;
            walk.x+=speed;
            MoveAndSlide(walk);
        }
        else {
            Visible=false;
            followPlayer();
        }
    }
    public void startSoud(){
        if (cloodown.TimeLeft > 0.3f && cloodown.TimeLeft < 0.4f){
            shoot.Play();
        }
    }
    public void followPlayer(){
        // shoot.Stop();
        player_acttack=false;
        Vector2 position=GetMyPosition();
        position.y+=-65;
        Position=position;
    }
    public override void _PhysicsProcess(float delta){
        string start = GetStart();
        if (start=="lao_seeb" || !cloodown.IsStopped()){
            MyActtack();
            startSoud();
        }
        else if (GetPlayerOver()){
            SetCollisionLayerBit(1,false);
            Visible=false;
            shoot.Stop();
        }
        else {
            SetCollisionLayerBit(1,false);            
            shoot.Stop();
            followPlayer();

        }
    }
}
