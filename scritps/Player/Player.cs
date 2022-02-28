using Godot;
using System;

public class Player : KinematicBody2D
{
    // -------------------------------------------------ส่งค่าตัวเเปร-------------------------------------------
    [Signal]
    public delegate void get_animation_fencing(Boolean fencing);
    [Signal]
    public delegate void get_my_direcion(string direcion);
    [Signal]
    public delegate void get_my_positon(Vector2 postion);
    [Signal]
    public delegate void get_my_over(Boolean over);
    // -------------------------------------------------กำหนดตัวเเปร-----------------------------------------
    [Export]
    public Vector2 Walk=Vector2.Zero;
    [Export]
    public int speed;
    [Export]
    public int poverspeed;
    [Export]
    public int jum;
    [Export]
    public int down;
    public int save_speed;
    public string playerScale="";
    public string direcion="left";
    public Boolean fencing=false;
    public Vector2 position= Vector2.Zero;
    public Boolean over=false;
    public Boolean stop_action=false;
    public Node2D body;
    public Timer cool_down;
    public AnimationPlayer foot;
    public AnimationPlayer action;
    public AnimationPlayer cloth;
    public AnimationPlayer move;
    public AudioStreamPlayer2D sound_over;
    public override void _Ready()
    {
        Vector2 my = GetNode<Node2D>("../").Position;
        SetPosition(my);
        sound_over = GetNode<AudioStreamPlayer2D>("soundOver");
        cool_down = GetNode<Timer>("Timer");
        body= GetNode<Node2D>("Body");
        foot = GetNode<AnimationPlayer>("foot");
        action = GetNode<AnimationPlayer>("Action");
        cloth = GetNode<AnimationPlayer>("cloth");
        move = GetNode<AnimationPlayer>("move");
        action.Play("not_action");
        cloth.Play("stand");
        foot.Play("stand");
        move.Play("left");
        over=false;
    }
    // -------------------------------------------------กันชน-------------------------------------------
    public void PlayerOver(Node body){
        String name = body.Name;
        if (!over){
            action.Play("Over");
            sound_over.Play();
            cool_down.Start(0.5f);
            over=true;
            SetCollisionLayerBit(0,false);
            SetCollisionLayerBit(7,true);
            SetCollisionMaskBit(5,false);
            GetNode<KinematicBody2D>("../swordPlayer").SetCollisionLayerBit(2,false);
            EmitSignal(nameof(get_my_over),over);
        }
    }
    public void stopActionLao_seeb(Node body){
            stop_action=true;

    }
    //----------------------------------------------เคลื่อนที่-----------------------------------------------
    public void Move(){
        Walk.x=0;
        if (!(IsOnWall())){
            speed=poverspeed;
        }
        if (action.CurrentAnimation!="lao_seeb") { // ถ้าไม่ได้ยิง หรือโจมตี
            if(Input.IsActionPressed("left")){
                move.Play("left");
                Walk.x-=speed;
            }
            else if (Input.IsActionPressed("right")){
                Walk.x+=speed;
                move.Play("right");
            }
        }
        else if (action.CurrentAnimation=="lao_seeb"){ // ถ้าโจมตี
            if (Input.IsActionPressed("left") && body.Scale.x==1) Walk.x-=speed;
            else if (Input.IsActionPressed("right") && body.Scale.x==-1) Walk.x+=speed;
        }
    }
    public void Jum(){
        if (IsOnWall()){
            if(Input.IsActionPressed("up")){
                Walk.y=0;
                Walk.y-=jum;
            }
        }
        else{

            Walk.y+=down;
        }
    }
    // --------------------------------------------เเสดงท่าทาง---------------------------------------------
    public void ActionHend(){
        if (Input.IsActionJustPressed("dodge")){
                action.Play("dodge");
            }
        if (action.CurrentAnimation=="" || action.CurrentAnimation=="not_action"){
            if (Input.IsActionJustPressed("lao_seeb")){
                action.Play("fencing");
            }
            else if (Input.IsActionJustPressed("sword")) {
                action.Play("lao_seeb");
            }
            else if (action.CurrentAnimation=="") {
                fencing=false;
                action.Play("not_action");
            }
        }
    }
    public void ActionFoot(){
        if(action.IsPlaying()){
            if (IsOnWall()){
                if (Input.IsActionPressed("up")) foot.Play("jum");
                else if (Walk.x==0) foot.Play("stand");
                else foot.Play("walk");
            }
            else if(!IsOnWall()) foot.Play("jum_stop");
        }
    }
    public void PlayerAtion(){
        // สกิล
        ActionHend();
        // เท้า
        ActionFoot();        
        // ผ้า
        if (IsOnWall()) cloth.Play("stand");
        else cloth.Play("stop");
        // ถ้าดาบชนกัน
        if (stop_action){
            action.Stop();
            stop_action=false;
        }
    }
    // ---------------------------------------------------getter----------------------------------------
    public String getStartGame(){
        return GetNode<Root>("../../").start_game;
    }
    public override void _PhysicsProcess(float detal){
        if (getStartGame()=="start game" && !over){
            Move();
            Jum();
            PlayerAtion();
            position=GetGlobalPosition();
        }
        else if (getStartGame()=="Player over") {
            foot.Play("stand");
            if (cool_down.IsStopped()){
                sound_over.Stop();
            }
            if (IsOnWall()){
                Walk = Vector2.Zero;
            }
            else{
                Walk.y+=10;
            }
            over=true;
        }
        else{
            SetCollisionLayerBit(0,true);
            SetCollisionLayerBit(7,false);
            action.Play("not_action");
            cloth.Play("stand");
            foot.Play("stand");
            move.Play("left");
            over=false;
        }
        MoveAndSlide(Walk);
    }
}
