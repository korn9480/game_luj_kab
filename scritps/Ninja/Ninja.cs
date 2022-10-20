using Godot;
using System;

public class Ninja : KinematicBody2D
{
    [Signal]
    public delegate void get_ninja_over(string my_start);
    [Signal]
    public delegate void get_ninja_position(Vector2 position);

    [Export]
    public Vector2 Walk = Vector2.Zero;
    [Export]
    public int speed;
    [Export]
    public int float_speed;
    [Export]
    public int jum;
    [Export]
    public int down;
    public Boolean ninja_over=false;
    public Boolean not_move=false;
    public Vector2 position;
    public string my_moving;
    // ชนิด
    public string my_start;
    public string target;

    public AnimationPlayer action;
    public AnimationPlayer foot;
    public RayCast2D move;
    public RayCast2D damage;
    public CollisionPolygon2D area;
    public Area2D body_area;
    public Timer disappear;// เวลาล่องหน
    public AudioStreamPlayer2D sound_over;
    public string name_action;
    public Boolean stop_action;

    public override void _Ready()
    {
        Vector2 position = GetNode<Node2D>("../").Position;
        SetPosition(position);
        action=GetNode<AnimationPlayer>("Action");
        foot = GetNode<AnimationPlayer>("Foot");
        move = GetNode<RayCast2D>("NodeBody/move");
        damage = GetNode<RayCast2D>("NodeBody/damage");
        disappear = GetNode<Timer>("disappear");
        sound_over = GetNode<AudioStreamPlayer2D>("soundOver");
        foot.Play("stand");
        area=GetNode<CollisionPolygon2D>("CollisionPolygon2D");
        body_area = GetNode<Area2D>("NodeBody/areaBody");
        SetNinja();
        GetNode<Node2D>("../../").Connect("PLayer_go",this,nameof(showBodyNinja));
    }
    public void showBodyNinja(string grub){
        if (grub==my_start && !ninja_over){
            GetNode<KinematicBody2D>("../ThrowingStars").SetCollisionLayerBit(6,true);
            GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,true);
            SetCollisionMaskBit(0,true);
            SetCollisionLayerBit(5,true);
            GetNode<Node2D>("../").Visible=true;
            body_area.SetMonitoring(true);
            my_moving="start game";

        }
    }
    public void SetNinja(){
        my_start = GetNode<NinjaType>("../").grub;
        target = GetNode<NinjaType>("../").target;
        SetCollisionMaskBit(0,false);
        GetNode<KinematicBody2D>("../ThrowingStars").SetCollisionLayerBit(6,false);
        GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,false);
        SetCollisionLayerBit(5,false);
        body_area.SetMonitoring(false);
        GetNode<Node2D>("../").Visible=false;
    }

    // -------------------------------------------กันชน----------------------------------------------
    public void Over(Node body){
        if (!ninja_over){
                foot.Play("stand");
                action.Play("over");
                sound_over.Play();
                SetCollisionLayerBit(5,false);
                SetCollisionLayerBit(7,true);
                SetCollisionMaskBit(0,false);
                SetCollisionMaskBit(1,false);
                startVisible(true);
                ninja_over=true;
                GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,false);
                EmitSignal(nameof(get_ninja_over),my_start);
            // }
        }
    }
    public void resetAction(Node body){
        if (!ninja_over){
            if(body.Name=="lao_seeb" || body.Name=="Player"){
                action.Play("not_action");
            }
        }
    }
    public void StartFencing(Node body){
        if (!ninja_over){
            GetNode<KinematicBody2D>("../swordNinja").Visible=true;
            action.Play("fencing");
            if (body.Name=="swordPlayer"){
                MoveBack();
            }
        }
    }
    public void startDodge(Node body){
        if (!ninja_over && body.Name=="lao_seeb"){
            action.Play("dodge");
        }
    }
    public void stopActionThrowingStars(Node body){
        stop_action=true;
    }
    //-------------------------------------------getter----------------------------------------------
    public Vector2 GetPositionPlayer(){
        Vector2 position = Vector2.Zero;
        if (target=="player") position=GetNode<Player>("../../../Player/Player").position;
        else if (target=="nkauj coob"){
            position = GetNode<NkaujCoob>("../../../heroineCheckpoint/NkaujCoob/NkaujCoob").position;
        }
        return position;

    }
    public Boolean GetPlayerJum(){
        return GetNode<KinematicBody2D>("../../../Player/Player").IsOnWall()==false;
    }
    public string getGrubBot(){
        try
        {
            return GetNode<NinjaOutpost>("../../").bot_play_this;
        }
        catch (System.Exception)
        {
            return GetNode<heroineCheckpoint>("../../").bot_play_this;
            // throw;
        }
    }
    // ------------------------------------------Method----------------------------------------------
    public void changDirection(int scale){
        if (area.Scale.x!=scale){
            Vector2 vector = Vector2.One;
            vector.x=scale;
            area.SetScale(vector);
            GetNode<Node2D>("NodeBody").SetScale(vector);
        }
    }
    public void MoveBack(){
        Vector2 my_turned=area.GetScale();
        float_speed=150;
            // เคลื่อนที่ไปทาง
        if (my_turned.x==1){
            float_speed=-float_speed;
        }
        else float_speed=+float_speed;
    }
    public void Move(Vector2 player){
        Walk.x=0;
        if (!move.IsColliding()){
            if (action.CurrentAnimation=="fencing" || !IsOnWall()){
                Walk.x+=float_speed;
            }
            else if  (IsOnWall()){//เดินบนพื้น
                Vector2 my_positon = GetGlobalPosition();
                if (player.x>my_positon.x){//เดินไปขวา
                    changDirection(1);
                    Walk.x+=speed; // 2<5
                    float_speed=300;
                }
                else {// เดินไปซ้าย
                    changDirection(-1);
                    Walk.x-=speed;
                    float_speed=-300;
                }
            }
        }
        else{
            MoveBack();
            
        }
        // else if (!)Walk.x +=float_speed;
    }
    public void ShowAtion(){
        //Ation
        if (!action.IsPlaying()){
            if (damage.IsColliding()){
                action.Play("throwing_stars");
                if (disappear.IsStopped()){
                    float time = disappear.WaitTime;
                    disappear.Start(time);
                }
            }
            else action.Play("not_action");
        }
        //fool
        if (!(foot.IsPlaying())){
            if (IsOnWall()){
                if (Walk.x!=0) foot.Play("walk");
                else foot.Play("stand");
            }
            // else if (!IsOnWall()) foot.Play("jum_stop");
            else if (!IsOnWall()) foot.Stop();
        }
        // ถ้าดาบชนกัน
        if (stop_action){
            action.Stop();
            stop_action=false;
        }
    }
    public void Disappear(){
        if (!disappear.IsStopped()){
            float time = disappear.WaitTime;
            if (disappear.TimeLeft==time){
                startVisible(false);
            }
            else if (disappear.TimeLeft<=(time-0.5)){
                startVisible(true);
            }
        }
    }
    public void startVisible(Boolean r){
        Visible=r;
        GetNode<KinematicBody2D>("../ThrowingStars").Visible=r;
        GetNode<KinematicBody2D>("../swordNinja").Visible=r;
    }
    public void Jum(Vector2 player){
        if (IsOnWall()){
            Walk.y=0;
            Vector2 my = GetGlobalPosition();
            float distance = Math.Abs(player.x - my.x);
            if ( distance<400){
                Walk.y-=jum;
                foot.Play("jum");
            }
        }
        else {
            Walk.y+=down;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        // string my_moving= getStartGame();
        string grub = getGrubBot();
        if (!ninja_over){
            if ((my_moving=="start game")){
                Vector2 player=GetPositionPlayer();
                Move(player);
                Jum(player);
                ShowAtion();
                Disappear();
                position=Position;
            }
        }
        else if (ninja_over){
            ninja_over=true;
            if (action.CurrentAnimation==""){
                action.Play("over_stop");
            }
            // action.Stop();
            if (!(IsOnWall())) {
                Walk.y+=down;
                }
            else{
                Walk= Vector2.Zero;
            }
        }
        MoveAndSlide(Walk);   
    }

}
