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
    [Signal]
    public delegate void get_reset_game();
    [Signal]
    public delegate void get_start_game(Boolean start_game);
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
    public int down,save_speed;
    public string playerScale="",direcion="left";
    public Boolean fencing=false,start_game=false;
    public Vector2 position= Vector2.Zero;
    public Boolean over=false,stop_action=false;
    public Node2D body;
    public AnimationPlayer foot,action,move,cloth;
    public AudioStreamPlayer2D sound_over,sword;
    public Button button_reset,button_start;
    public Label result;
    public override void _Ready()
    {
        Vector2 my = GetNode<Node2D>("../").Position;
        SetPosition(my);
        sound_over = GetNode<AudioStreamPlayer2D>("soundOver");
        sword = GetNode<AudioStreamPlayer2D>("../swordPlayer/soundSword");
        body= GetNode<Node2D>("Body");
        foot = GetNode<AnimationPlayer>("foot");
        action = GetNode<AnimationPlayer>("Action");
        cloth = GetNode<AnimationPlayer>("cloth");
        move = GetNode<AnimationPlayer>("move");
        action.Play("not_action");
        cloth.Play("stand");
        foot.Play("stand");
        move.Play("left");
        button_reset = GetNode<Button>("reset");
        button_start = GetNode<Button>("startGame");
        result = GetNode<Label>("gameOver");
        over=false;
    }
    // -----------------------------------------------button------------------------------------------
    public void startGame(){
        button_start.Visible=false;
        button_reset.Visible=false;
        start_game=true;
        EmitSignal(nameof(get_start_game),start_game);
    }
    public void resetGame(){
        EmitSignal(nameof(get_reset_game));
    }
    // -------------------------------------------------กันชน-------------------------------------------
    public void Over(Node body){
        String name = body.Name;
        if (!over && body.Name!="Bot"){
            button_start.Visible=true;
            button_reset.Visible=true;
            action.Play("Over");
            sound_over.Play();
            over=true;
            start_game=false;
            SetCollisionLayerBit(0,false);
            SetCollisionLayerBit(7,true);
            SetCollisionMaskBit(5,false);
            GetNode<KinematicBody2D>("../swordPlayer").SetCollisionLayerBit(2,false);
            EmitSignal(nameof(get_my_over),over);
        }
    }
    public void stopActionSword(Node body){
        if (!stop_action && action.CurrentAnimation=="fencing"){
            sword.Play();
            stop_action=true;
        }

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
    public void showAction(){
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
    public override void _PhysicsProcess(float detal){
        // อนุณาติให้เล่น
        if (start_game && result.Text!="You Loss!!!"){
            Move();
            Jum();
            showAction();
            position=GetGlobalPosition();
        }
        else if (over) {
            foot.Play("stand");
            if (IsOnWall()){
                Walk = Vector2.Zero;
            }
            else{
                Walk.y+=10;
            }
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
        
        if (result.Text!=""){
            button_reset.Visible=true;
            button_start.Visible=true;
        }
        MoveAndSlide(Walk);
    }
}
