using Godot;
using System;

public class NinjaType : Node2D
{
    // เล่นตอนผู้ไป หรือ ว่ากลับ
    [Export]
    public String grub;
    // เป้าหมายที่โจมตี ผู้เล่น หรือ นางเอก
    [Export]
    public String target;

    [Signal]
    public delegate void my_play(String grub);
    [Signal]
    public delegate void my_target(String target);
    public override void _Ready()
    {
        EmitSignal("my_play",grub);
    }


}
