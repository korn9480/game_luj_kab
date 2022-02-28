using Godot;
using System;

public class NinjaType : Node2D
{
    // เล่นตอนผู้ไป หรือ ว่ากลับ
    [Export]
    public String play;
    // เป้าหมายที่โจมตี ผู้เล่น หรือ นางเอก
    [Export]
    public String target;

    [Signal]
    public delegate void my_play(String play);
    [Signal]
    public delegate void my_target(String target);
    public override void _Ready()
    {
        
    }


}
