using dream.scrips;
using Godot;
using System;

public partial class Explosion : Node2D
{
    public float Radius = 30f;
    public float TimeToLive = 3f;
    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, new Color(0.5f, 0.6f, 0.7f));
    }
    public override void _Process(double delta)
    {
        TimeToLive -= (float)delta;
        if (TimeToLive < 0) { QueueFree(); }
        base._Process(delta);
    }
    public override void _Ready()
    {
        Vector2 distance =  Power.Player.Position -Position;
        var test = distance.Length();
        if (distance.Length() < Power.ExplosionRadius)
        {
            Vector2 blastOff = distance.Normalized() * Power.ExplosionPower;
            if (!Power.ExplosionDistanceNotMatter) 
            {
                blastOff /= distance.Length();
            }
            Power.Player.Velocity += blastOff;
        }
        base._Ready();
    }
}
