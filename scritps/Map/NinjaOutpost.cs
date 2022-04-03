using Godot;
using System;

public class NinjaOutpost : Node2D
{
    [Signal]
    public delegate void player_start(string start_game);
    [Signal]
    public delegate void PLayer_go(string bot_play_this);
    public string start_game="not start game";
    public string bot_play_this = "";
    public string bot_play_map="";
    //--------------------------ตัวเเปรจำนวน บอท--------------------------------------------------
    public int grub_bot_go;
    public int grub_bot_back;

    public override void _Ready()
    {
        GetNode<Node2D>("../").Connect("get_grub_bot",this,nameof(changGrubBosPlayer));
        GetNode<Node2D>("../heroineCheckpoint").Connect("player_go_or_back",this,nameof(changGrubBosPlayer));

    }
    public void changGrubBosPlayer(string player_move){
        bot_play_map=player_move;
    }
    // --------------------------------------------button-------------------------------------------------
    public void playerJoinCheckpoint(Node body){
        // บัคฆ่าเสร็จโดดออก1ครั้งเเล้วเข้า ค่า bot start ถูกเปลี่ยน
        if (bot_play_this!=bot_play_map){
            bot_play_this=bot_play_map;
            EmitSignal(nameof(PLayer_go),bot_play_this);
        }
    }
}
