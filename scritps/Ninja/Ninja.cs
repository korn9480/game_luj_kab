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
    public Timer disappear,cool_Down_Over;
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
        cool_Down_Over=GetNode<Timer>("coolDownOver");
        sound_over = GetNode<AudioStreamPlayer2D>("soundOver");
        foot.Play("stand");
        area=GetNode<CollisionPolygon2D>("CollisionPolygon2D");
        body_area = GetNode<Area2D>("NodeBody/areaBody");
        SetNinja();
        GetNode<Node2D>("../../").Connect("PLayer_go",this,nameof(showBodyNinja));
    }
    public void showBodyNinja(string grub,string start){
        if (grub==my_start){
            GetNode<KinematicBody2D>("../ThrowingStars").SetCollisionLayerBit(6,true);
            GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,true);
            SetCollisionMaskBit(0,true);
            SetCollisionLayerBit(5,true);
            GetNode<Node2D>("../").Visible=true;
            body_area.SetMonitoring(true);
            my_moving=start;

        }
    }
    public void SetNinja(){
        my_start = GetNode<NinjaType>("../").play;
        target = GetNode<NinjaType>("../").target;
        SetCollisionMaskBit(0,false);
        GetNode<KinematicBody2D>("../ThrowingStars").SetCollisionLayerBit(6,false);
        GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,false);
        SetCollisionLayerBit(5,false);
        body_area.SetMonitoring(false);
        GetNode<Node2D>("../").Visible=false;
    }
    // -------------------------------------------กันชน----------------------------------------------
    public void abused(Node body){
        if (!ninja_over){
            if (body.Name=="swordPlayer"|| body.Name=="lao_seeb"){
                foot.Play("stand");
                action.Play("over");
                sound_over.Play();
                cool_Down_Over.Start(0.5f);
                SetCollisionLayerBit(5,false);
                SetCollisionLayerBit(7,true);
                SetCollisionMaskBit(0,false);
                SetCollisionMaskBit(1,false);
                startVisible(true);
                ninja_over=true;
                GetNode<KinematicBody2D>("../swordNinja").SetCollisionLayerBit(6,false);
                EmitSignal(nameof(get_ninja_over),my_start);
            }
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
                Vector2 my_turned=area.GetScale();
                // ระดับ
                float_speed=150;
                // เคลื่อนที่ไปทาง
                if (my_turned.x==1){
                    float_speed=-float_speed;
                }
                else float_speed=+float_speed;
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
        return GetNode<Player>("../../../Player/Player").position;
    }
    public Boolean GetPlayerJum(){
        return GetNode<KinematicBody2D>("../../../Player/Player").IsOnWall()==false;
    }
    public string getStartGame(){
        return GetNode<NinjaOutpost>("../../").bot_start;
    }
    public string getGrubBot(){
        return GetNode<NinjaOutpost>("../../").grub_bot;
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
    public void ninjaMove(Vector2 player){
        Walk.x=0;
        if (action.CurrentAnimation=="fencing"){
            Walk.x+=float_speed;
        }
        else if  (IsOnWall() && !(move.IsColliding())){//เดินบนพื้น
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
        else if (!move.IsColliding())Walk.x +=float_speed;
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
            else if (!IsOnWall()) foot.Play("jum_stop");
        }
        // ถ้าดาบชนกัน
        if (stop_action){
            action.Stop();
            stop_action=false;
        }
    }
    public void myDisappear(){
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
            if (GetPlayerJum() && distance<400){
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
            if ((my_moving=="start game" || my_moving=="Player over") && (grub==my_start)){
                Vector2 player=GetPositionPlayer();
                ninjaMove(player);
                Jum(player);
                ShowAtion();
                myDisappear();
                position=Position;
            }
        }
        else if (ninja_over){
            ninja_over=true;
            action.Play("over_stop");
            if (cool_Down_Over.IsStopped()) sound_over.Stop();
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
