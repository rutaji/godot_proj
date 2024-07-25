using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dream.scrips
{
    public partial class Bullet : AnimatableBody2D
    {
        public Vector2 Velocity;
        public override void _PhysicsProcess(double delta) 
        {
            KinematicCollision2D collision = MoveAndCollide(Velocity*(float)delta);
            if (collision != null) 
            {
                GD.Print("boom");
                var Loaded = ResourceLoader.Load<PackedScene>("res://scenes/Explosion.tscn");
                var explosion = Loaded.Instantiate();
                Explosion explosion1 = explosion.GetNode<Explosion>(".");
                explosion1.Position = Position;
                GetNode("/root").AddChild(explosion1);
                explosion1.Radius = Power.ExplosionRadius;
                QueueFree();
                
            }
            
        }
    }

    
}
