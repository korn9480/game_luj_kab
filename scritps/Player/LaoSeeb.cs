using Godot;
using System;

public class LaoSeeb : KinematicBody2D
{
    [Signal]
    public delegate void get_player_acttack(Boolean player_acttack);
    [Signal]
    public delegate void get_animation_acttack(string animation);
    public int speed;
    public string name_effect="";
    public AudioStreamPlayer2D sound;
    public Timer cloodown;
    public Vector2 walk = Vector2.Zero;
    public Boolean player_acttack=false;
    public AudioStreamPlayer2D shoot,sword,lao_seej;
    public override void _Ready()
    {
        cloodown=GetNode<Timer>("Timer");
        shoot=GetNode<AudioStreamPlayer2D>("shoot");
        sword = GetNode<AudioStreamPlayer2D>("../swordPlayer/soundSword");
        lao_seej = GetNode<AudioStreamPlayer2D>("lao_seeb");
    }
    // -----------------------------------รับค่าจาก godot----------------------------------------------
    public Vector2 GetMyPosition(){
        return GetNode<KinematicBody2D>("../Player").Position;
    }
    public Boolean GetMove(){
        return GetNode<Node2D>("../Player/Body").Scale.x==1;
    }
    public string GetStart(){
        return GetNode<AnimationPlayer>("../Player/Action").CurrentAnimation;
    }
    //---------------------------------------------Method ต่างๆ------------------------------------------
    public Boolean GetPlayerOver(){
        return GetNode<Player>("../Player").over==true;
    }
    public void stopAttack(Node body){
        if (!cloodown.IsStopped()){
            name_effect=body.Name;
            if (name_effect=="ThrowingStars") lao_seej.Play();
            else if (name_effect=="swordNinja") sword.Play();
        }
        
    }
    public void MyActtack(){
        if (cloodown.IsStopped()){
            SetCollisionLayerBit(1,true);
            cloodown.Start(0.7f); 
            shoot.Play();
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
    public void soundCrash(){
        if(name_effect!=""){
            cloodown.Stop();
            name_effect="";
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
            soundCrash();
        }
        else if (GetPlayerOver()){
            SetCollisionLayerBit(1,false);
            Visible=false;
        }
        else {
            SetCollisionLayerBit(1,false);            
            followPlayer();

        }
    }
}
