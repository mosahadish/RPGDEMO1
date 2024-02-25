using Game;
using Godot;
using System;

public partial class Bow : Weapon
{
    [Export] AnimationPlayer animPlayer;


    public void Draw()
    {
        animPlayer?.Play("Draw");
    }
    
    public void Release()
    {
        animPlayer?.Play("Release");
    }
}
