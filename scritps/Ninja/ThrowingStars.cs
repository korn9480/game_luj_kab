using Godot;
using System;

public class ThrowingStars : KinematicBody2D 
{
    [Signal]
    public delegate void get_player_acttack(Boolean player_acttack);
    public int speed;
    public Boolean stopTime=false;
    Timer cloodown;
    Vector2 walk = Vector2.Zero;
    public Boolean player_acttack=false;
    AudioStreamPlayer2D shoot;
    public override void _Ready()
    {
        cloodown=GetNode<Timer>("Timer");
        shoot=GetNode<AudioStreamPlayer2D>("shoot");
        followPlayer();
    }
    // -----------------------------------รับค่าจาก godot----------------------------------------------
    public Vector2 GetMyPosition(){
        return GetNode<Ninja>("../Bot").position;
    }
    public Boolean GetMove(){
        return GetNode<Node2D>("../Bot/NodeBody").Scale.x!=1;
    }
    public Boolean GetPlayerJum(){
        return GetNode<KinematicBody2D>("../Bot").IsOnWall();
    }
    public Boolean GetStart(){
        return GetNode<AnimationPlayer>("../Bot/Action").CurrentAnimation=="throwing_stars";
    }
    public Boolean GetNinjaOver(){
        return GetNode<Ninja>("../Bot/").ninja_over;
    }
    //---------------------------------------------Method ต่างๆ------------------------------------------
    public void stopAttack(Node body){
        cloodown.Stop();
        followPlayer();

    }
    public void MyActtack(){
        if (cloodown.IsStopped()){
            shoot.Play();
            cloodown.Start(0.7f);
            Position=GetGlobalPosition();
            player_acttack=true;
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
            RotationDegrees+=10;
            MoveAndSlide(walk);
            Visible=true;
        }
        else {
            RotationDegrees=0;
            followPlayer();
        }
    }
    public void followPlayer(){
        // player_acttack=false;
        Vector2 position=GetMyPosition();
        position.y-=147.157f;
        Position=position;
    }
  
    public override void _PhysicsProcess(float delta){
        Boolean start = GetStart();
        if (start || !cloodown.IsStopped()){
            MyActtack();
            if (IsOnWall()){
                // player_acttack=false;
            }
        }
        else if (GetNinjaOver()){
            SetCollisionLayerBit(6,false);
            GetNode<RemoteTransform2D>("../Bot/NodeBody/Skeleton/กระดูกสันหลัง/controlStars").UseGlobalCoordinates=true;
            player_acttack=false;
        }
        else {
            player_acttack=false;
            followPlayer();
            shoot.Stop();
        }
        
    }
}
