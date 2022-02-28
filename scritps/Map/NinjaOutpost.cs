using Godot;
using System;

public class NinjaOutpost : Node2D
{
    [Signal]
    public delegate void player_start(string start_game);
    [Signal]
    public delegate void PLayer_go(string grub_bot,string bot_start);
    public string start_game="not start game";
    public string bot_start = "not start game";
    public string grub_bot = "player go";
    //--------------------------ตัวเเปรจำนวน บอท--------------------------------------------------
    public int grub_bot_go;
    public int grub_bot_back;
    public int bot_go_over;
    public int bot_back_over;

    public override void _Ready()
    {
        GetNode<Node2D>("../heroineCheckpoint").Connect("player_go_or_back",this,nameof(changGrubBosPlayer));
        grub_bot_go= readGrubBos("GrubBosPlayerGo");
        grub_bot_back = readGrubBos("GrubBosPlayerBack");
        
    }
    public void updateBotOver(string my_start){
        if (my_start=="player go"){
            bot_go_over++;
        }
        else if (my_start=="player back"){
            bot_back_over++;
        }
    }
    public int readGrubBos(string grub){
        var grub_bot = GetTree().GetNodesInGroup(grub);
        int number=0;
        foreach(Node2D node in grub_bot){
            string parent = node.GetParent().Name;
            if (parent==Name){
                number++;
                GetNode<Node2D>(node.Name+"/Bot").Connect("get_ninja_over",this,nameof(updateBotOver));
            }
        }
        return number;
    }
    public void changGrubBosPlayer(string player_move){
        grub_bot=player_move;
    }
    // --------------------------------------------button-------------------------------------------------
    public void playerJoinCheckpoint(Node body){
        if (bot_start!="start game"){
            bot_start = "start game";
            EmitSignal(nameof(PLayer_go),grub_bot,bot_start);
        }
    }
    public void playerOutCheckpoint(Node body){
        if (grub_bot_go==bot_go_over && grub_bot=="player go"){
            bot_start="not start game";
        }
    }
}
