using Godot;
using System;

public class NkaujCoob : KinematicBody2D
{
    [Signal]
    public delegate void get_my_over(bool over);
    [Signal]
    public delegate void get_my_positon(Vector2 position);
    [Export]
    public int speed;
    [Export]
    public int jum;
    [Export]
    public int down;
    [Export]
    public Vector2 Walk,position;
    public int float_speed;
    public bool over=false;
    public AnimationPlayer foot,hair,cloth,action;
    public RayCast2D right_attack,left_attack,jum_body,jum_foot,moving;
    public Node2D my_img;
    public override void _Ready()
    {
        foot = GetNode<AnimationPlayer>("Foot");
        hair = GetNode<AnimationPlayer>("Hair");
        cloth = GetNode<AnimationPlayer>("Cloth");
        action = GetNode<AnimationPlayer>("action");
        right_attack = GetNode<RayCast2D>("scanBotRight");
        left_attack = GetNode<RayCast2D>("scanBotLeft");
        jum_body = GetNode<RayCast2D>("move/scanJumBody");
        jum_foot = GetNode<RayCast2D>("move/scanJumFoot");
        moving = GetNode<RayCast2D>("move/move");
        my_img = GetNode<Node2D>("move");

        foot.Play("stand");
        cloth.Play("stand");
        hair.Play("stand");
        action.Play("stand");
    }
    // ------------------------------ getter ---------------------------------------------
    public Vector2 GetNodePositonPlayer(){
        return GetNode<Player>("../../../Player/Player").position;
    }
    public String getStartGame(){
        return GetNode<heroineCheckpoint>("../../").start_game;
    } 
    // ------------------------------ กันชน ------------------------------------------------
    public void Over(Node body){
        if (getStartGame()=="start game"){
            action.Play("over");
            foot.Play("over");
            cloth.Play("over");
            hair.Play("over");
            over=true;
            SetCollisionLayerBit(0,false);
            SetCollisionLayerBit(7,true);
            SetCollisionMaskBit(5,false);
            EmitSignal(nameof(get_my_over),over);
        }
    }
    public void Dodge(Node body){
        if (!over){
            if (body.Name=="swordNinja"){
                Vector2 direction = GetScale();
                if (direction.x>0) Walk.x-=speed+100;
                else if (direction.x<0) Walk.x+=speed;
            }
            // GD.Print("dodge ");
            action.Play("dodge");
            
        }
    }
    public void resetAction(Node body){
        if (!over){
            action.PlayBackwards("dodge");
            // action.Play("stand");
        }
    }
    public void jumDodge(Node body){
        if (getStartGame()=="start game"){
            GD.Print("jum");
            Vector2 n = my_img.GetScale();
            n.x = n.x *-1;
            my_img.SetScale(n);
            action.Play("dodge");
        }
    }
    // ------------------------------ method move-------------------------------------------
    public void Move(){
        if (IsOnWall()){
            Vector2 side = Vector2.One; // หันซ้ายขวา
            if (right_attack.IsColliding()){
                Walk.x = speed;
                side.x=-1;
            }
            else if (left_attack.IsColliding()){
                Walk.x = -speed;
            }
            else {
                Vector2 player = GetNodePositonPlayer();
                Vector2 position = GetGlobalPosition();
                if (position.x>player.x){
                    Walk.x= -speed;
                }
                else if (position.x<player.x){
                    Walk.x=speed;
                    side.x = -1;
                }
            }
            my_img.SetScale(side);
            float_speed = (int) Walk.x;
        }
        else if (!moving.IsColliding()){
            Walk.x = float_speed;
        }
    }
    public void MoveBack(){
        Vector2 my_turned=my_img.GetScale();
        float_speed=150;
            // เคลื่อนที่ไปทาง
        if (my_turned.x==1){
            float_speed=-float_speed;
        }
        else float_speed=+float_speed;
}
    public void Jum(){
        if (IsOnWall()){
            Walk.y=0;
            if(jum_foot.IsColliding()){
                // GD.Print(jum_foot.IsColliding());
                Walk.y-=jum;
            }
        }
        else {
            Walk.y+=down;
        }
    }
    public void showAction(){
        // action food Cloth
        if (!cloth.IsPlaying()){
            if (IsOnWall()){
                if(jum_body.IsColliding() || jum_foot.IsColliding()){
                    foot.Play("jum");
                    cloth.Play("jum");
                    hair.Play("jum");
                }
                else {
                    // GD.Print("wakl");
                    hair.Play("stand");
                    foot.Play("walk");
                    cloth.Play("walk");
                }
            }
            else if(Walk.y<0) {
                // GD.Print("jum stop");
                foot.Play("jum_stop");
                cloth.Play("jum_stop");
                hair.Play("jum_stop");
            }
        }
        if (action.CurrentAnimation!="dodge"){
            if (left_attack.IsColliding() || right_attack.IsColliding()){
                // action.Play("stand");
                action.Play("Knife");
                Jum();
            }
            else {
                // action.Play("stand");
                action.Play("walk");
            }
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        if (getStartGame()=="start game" && !over){
            Move();
            showAction();
            Jum();
            position=GetGlobalPosition();
        }
        else if (over){
            if (!(IsOnWall())) {
                Walk.y+=down;
                }
            else{
                Walk= Vector2.Zero;
            }
            if(!action.IsPlaying()){
                action.Stop();
                foot.Stop();
                cloth.Stop();
                hair.Stop();
            }
        }
        MoveAndSlide(Walk); 
    }

}
