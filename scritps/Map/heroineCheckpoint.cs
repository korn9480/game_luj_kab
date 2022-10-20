using Godot;
using System;

public class heroineCheckpoint : Node2D 
{
    [Signal]
    public delegate void player_go_or_back(string move);
    [Signal]
    public delegate void PLayer_go(string bot_play_this);
    [Signal]
    public delegate void NkaujCoob_start(string start_game);
    public string move="player go";
    public string bot_play_this = "";
    public string bot_play_map="";
    
    // รอหาวิธี เชื่อม class
    public string start_game="not start game";
    public string bot_start = "not start game";
    public string grub_bot = "player go";
    //--------------------------ตัวเเปรจำนวน บอท--------------------------------------------------

    public int grub_bot_go;
    public int bot_go_over;
    public int grub_bot_back;
    public AudioStreamPlayer2D sound_bos;
    public override void _Ready()
    {
        GetNode<Node2D>("../").Connect("get_grub_bot",this,nameof(changGrubBosPlayer));
        GetNode<Node2D>("../heroineCheckpoint").Connect("player_go_or_back",this,nameof(changGrubBosPlayer));
        sound_bos = GetNode<AudioStreamPlayer2D>("Bos/Bot/soundOpen");
        readGrubBos();
    }
    public void readGrubBos(){
        var grub_bot = GetChildren();
        foreach (Node item in grub_bot){
            if (item.GetScript()!=null){
                string grub= GetNode<NinjaType>(item.Name).grub;                              
                if (grub == "player go"){
                    item.GetChild(1).Connect("get_ninja_over",this,nameof(updateBotOver));
                    bot_go_over++;
                }
            }
        }
    }
    public void PlayerBack(Node body){
        move="player back";
        EmitSignal(nameof(player_go_or_back),move);
    }

    public void updateBotOver(string my_start){
        if (my_start=="player go"){
            bot_go_over--;
            if(bot_go_over==0) {
                sound_bos.Play();
            }
        }
    }
    // method รอ เชื่อม class
    public void helpNkaujCoob(Node body){
        if (bot_go_over == grub_bot_go){
            start_game ="start game";
        }
    }
    public void changGrubBosPlayer(string player_move){
        bot_play_map=player_move;
    }
    // --------------------------------------------button-------------------------------------------------
    public void playerJoinCheckpoint(Node body){
        // บัคฆ่าเสร็จโดดออก1ครั้งเเล้วเข้า ค่า bot start ถูกเปลี่ยน
        if (bot_play_this!=bot_play_map){
            bot_play_this=bot_play_map;
            GD.Print(bot_play_this);
            EmitSignal(nameof(PLayer_go),bot_play_this);
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        if (bot_go_over==0 && !sound_bos.IsPlaying()){
            EmitSignal(nameof(PLayer_go),"bos");
        }
    }
}