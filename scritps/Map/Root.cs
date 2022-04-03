using Godot;
using System;

public class Root : Node2D
{
    [Signal]
    public delegate void get_grub_bot(string move);
    public string bot_start = "not start game";

    //--------------------------ตัวเเปรจำนวน บอท--------------------------------------------------
    public int grub_bot_back;
    public int bot_back_over;
    public Boolean grub_bot_back_moving=false;
    public Button button_start;
    public Button button_reset;
    
    public Label result;

    public override void _Ready()
    {
        GD.Print("----------------------------");
        button_start = GetNode<Button>("Player/Player/startGame");
        button_reset = GetNode<Button>("Player/Player/reset");
        result = GetNode<Label>("Player/Player/gameOver");
        Node2D checkpoint_one = GetNode<Node2D>("checkpointOne");
        readGrubBos();
        
        GetNode<Node2D>("Player/Player").Connect("get_my_over",this,nameof(PlayerLoss));
        GetNode<KinematicBody2D>("Player/Player").Connect("get_start_game",this,nameof(startGame));
        GetNode<KinematicBody2D>("Player/Player").Connect("get_reset_game",this,nameof(resetGame));
        GetNode<Node2D>("heroineCheckpoint/NkaujCoob/NkaujCoob").Connect("get_my_over",this,nameof(PlayerLoss));
    }
    public void updateBotOver(string my_start){
        if (my_start=="player back"){
            grub_bot_back--;
        }
    }
    public async void readGrubBos(){
        var grub_bot = GetNode<Node2D>("checkpointOne").GetChildren();
        foreach (Node item in grub_bot){
            if (item.GetScript()!=null){
                string grub= GetNode<NinjaType>("checkpointOne/"+item.Name).grub;                              
                if (grub == "player back"){
                    item.GetChild(1).Connect("get_ninja_over",this,nameof(updateBotOver));
                    grub_bot_back++;
                }
            }
        }
    }
    // --------------------------------------------button-------------------------------------------------
    public void startGame(Boolean reset){
        if (result.Text==""){
            EmitSignal(nameof(get_grub_bot),"player go");
        }
    }
    public void resetGame(){
        GetTree().ReloadCurrentScene();
    }
    public void changPostionBotton(float x,float y,Button name){
        Vector2 player_positon=GetPositionPlayer();
        Vector2 set = Vector2.Zero;
        set.x=player_positon.x+x;
        set.y=player_positon.y+y;
        name.SetPosition(set);     
    }
    // -------------------------------------------getter-----------------------------------------------
    public Vector2 GetPositionPlayer(){
        return GetNode<KinematicBody2D>("Player/Player").Position;
    }
    // -------------------------------------------Method------------------------------------------------
    public void PlayerWin(){
        if (grub_bot_back==0){
            result.Text="You win!!!";
            
        }
        
    }
    public void PlayerLoss(Boolean over){
        bot_start="Player over";
        result.Text="You Loss!!!";
    }
     public override void _PhysicsProcess(float delta)
    {
        PlayerWin();
    }
}
