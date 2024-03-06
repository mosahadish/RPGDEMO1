using Game;
using Godot;
using System;

public partial class Bow : Weapon
{
    [Export] AnimationPlayer animPlayer;


    public void Default()
    {
        if (animPlayer.IsPlaying())
        {
            animPlayer.Stop();
        }

        animPlayer?.Play("Default");
    }

    public void Draw()
    {
        if (animPlayer.IsPlaying())
        {
            animPlayer.Stop();
        }
        animPlayer?.Play("Draw");
    }
    
    public void Release()
    {
        if (animPlayer.IsPlaying())
        {
            animPlayer.Stop();
        }

        animPlayer?.Play("Release");
    }
}
