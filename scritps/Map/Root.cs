using Godot;
using System;

public class Root : Node2D
{
    [Signal]
    public delegate void player_start(string start_game);
    [Signal]
    public delegate void PLayer_go(string grub_bot,string bot_start);

    public string start_game="not start game";
    public string bot_start = "not start game";
    public string grub_bot = "player go";
    //--------------------------ตัวเเปรจำนวน บอท--------------------------------------------------
    public int grub_bot_back;
    public int bot_back_over;
    public Boolean grub_bot_back_moving=false;
    public Button button_start;
    public Button button_reset;
    
    public Label result;

    public override void _Ready()
    {
        button_start = GetNode<Button>("Player/startGame");
        button_reset = GetNode<Button>("Player/reset");
        result = GetNode<Label>("Player/Player/gameOver");
        Node2D checkpoint_one = GetNode<Node2D>("checkpointOne");
        grub_bot_back= readGrubBos();
        
        GetNode<Node2D>("heroineCheckpoint").Connect("player_go_or_back",this,nameof(changGrubBosPlayer));
        GetNode<Node2D>("Player/Player").Connect("get_my_over",this,nameof(PlayerLoss));
    }
    public void updateBotOver(string my_start){
        if (my_start=="player back"){
            bot_back_over++;
        }
    }
    public int readGrubBos(){
        var grub_bot = GetNode<Node2D>("checkpointOne").GetTree().GetNodesInGroup("GrubBosPlayerBack");
        int number=0;
        string name=  GetNode<Node2D>("checkpointOne").Name;
        foreach(Node2D node in grub_bot){
            String parent = node.GetParent().Name;
            if (parent==name){
                number++;
                GetNode<Node2D>("checkpointOne/"+node.Name+"/Bot").Connect("get_ninja_over",this,nameof(updateBotOver));
            }
        }
        GD.Print("number : "+number);
        return number;
    }
    public void changGrubBosPlayer(string player_move){
        grub_bot=player_move;
    }
    // --------------------------------------------button-------------------------------------------------
    public void startGame(){
        start_game = "start game";
        button_start.Visible=false;
        button_reset.Visible=false;
    }
    public void resetGame(){
        start_game = "reset game" ;       
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
        
        if (grub_bot_back==bot_back_over){
            result.Text="You win!!!";
        }
        
    }
    public void PlayerLoss(Boolean over){
        bot_start="Player over";
        start_game ="Player over";
        result.Text="You Loss!!!";
    }
     public override void _PhysicsProcess(float delta)
    {
        string result_game= GetNode<Label>("Player/Player/gameOver").Text;
        if (start_game=="start game" && result_game==""){
            PlayerWin();
        }
        else if (result_game!=null){
            changPostionBotton(-416.162f,-143.975f,button_start);
            changPostionBotton(-415.271f,-53.994f,button_reset);
            button_start.Visible=true;
            button_reset.Visible=true;
        }
    }
}
