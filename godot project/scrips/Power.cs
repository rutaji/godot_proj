using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dream.scrips
{
    public partial class Power : Node2D
    {
        //------------same for all
        public float cooldown = 7;
        public bool RefreshOnFloorv = true; // if true cooldown will reset when on floor
        //------velocity
        float VelocityPower = 900; //velocity added when pressed
        float VelocityYMult = 1; // multiplayer for Y direction
        bool ResetVelovityOnActiveVel = true; //nastaví hráčovu velocity na nula při aktivaci
        //--------dash
        public float DashTime = 3; //duration of dash in seconds
        public float DashPower = 20; //velocity added every frame
        public bool SameSpeed = true; // if true dash is constant speed => if false dash is accelerating
        //----------sandevistan
        public float sandevistanTime = 1.9f; //duration in seconds
        public float sandevistanPower = 90; //speed of sandevistan
        public int KeepAllVelocity = 2; // 0 = reset velocity at end
                                        // 1 = sum of all velocity gained (during sandevistan) at end => set KeepAllVelocityMult low  0.03f
                                        // 2 = get boost to last direction player went on end => set KeepAllVelocityMult high 90f

        public float KeepAllVelocityMult = 90f;
        //------------------groundPound
        public float GroundPoundSpeed = 120; //velocity Y set on active
        public bool ResetVelocityOnGroundPound = true; // resets X Velocity on activation
        //-------------push
        public float PushPower = 5; // basically lenght of a vector => bigger number more speeeeedd
        public bool Pull = false; //opposite direction
        public bool DistanceDontMatter = false; //Distance is normalized to 1
        public bool ResetVelocityOnPush = true; //resets velocity on ability use 
        public float BonusPower = 30; //this power is not affected by distance => making this big nad power small results in lower revelance of distance
        public float MaxDistance = 70; // if distance (player to mouse) is bigger its set to this
        //-------------charge
        //runs fast in one direction until turns or hits wall
        public float MinChargeSpeed = 90; //min x velocity to activate Charge
        public float ChargeSpeed = 150; //speed set when starting charge
        public float ChargeAcceleration = 0.1f; //acceleration when charging
        public float MaxChargespeed = 170; // max speed while charging
        public static float ChargeTime = 8f; //maxChargeTime
        public static bool ResetChargeTimeOnCoin = true;//if true collecting coins while charging will reset duration of the charge back to chargeTime.

        //------------------Explosion
        public float BulletSpeed = 90; //spped of projectile
        public static float ExplosionRadius = 50; //radius of exlposion
        public static float ExplosionPower = 400; //Velocity added to player
        public static bool ExplosionDistanceNotMatter = true; //if true explosion will bounce player by the same force. Distance bettwen player and center of explosion doesnt matter.
        //-----------chargable dash
        public static float MaxFuel = 5; //mach fuel stored inm player. Click f3 to view. (seconds)
        public static float FuelVel = 100;//velocity constant every frame
        public static float DurationPerFuel = 0.7f; //time of dash per second of charging fuel.

        //----------UI nebo spíš jeho karikatura

        Label Name;
        Label C;
        //--------------Logic
        public static player Player;
        float CurrentCooldown = 0;
        public static int  SelectedPower = 0;
        int MaxPower = 9;
        public override void _Ready()
        {
            Name = GetNode<Label>("/root/World/Player/Camera2D/PowerMenu/Name");
            C = GetNode<Label>("/root/World/Player/Camera2D/PowerMenu/C");
            Player = GetNode<player>("/root/World/Player");
        }
        public void DoPowers(double delta)
        {
            CurrentCooldown -= (float)delta;
            ShowCooldown();
            if (Input.IsActionJustPressed("F5"))
            {
                ChangePower();
            }
            if (Input.IsActionJustPressed("ability") && CurrentCooldown <= 0 && SelectedPower != 0)
            {
                PowerActive(delta);
            }
        }
        private void PowerActive(double delta)
        {
            switch (SelectedPower)
            {
                case 1:
                    if (ResetVelovityOnActiveVel) { Player.ResetVelocity(); }
                    Player.Velocity += new Vector2(Player.InputXDirection, Player.InputYDirection * VelocityYMult) * VelocityPower;
                    CurrentCooldown = cooldown;
                    break;
                case 2:
                    Player.ResetVelocity();
                    Player.DashDirection = Player.GetInputVector();
                    Player.DashTimeBuffer = DashTime;
                    CurrentCooldown = cooldown;
                    break;
                case 3:
                    Player.ResetVelocity();
                    Player.sandevistanBuffer = sandevistanTime;
                    Player.sandevistanVector = Vector2.Zero;
                    CurrentCooldown = cooldown;
                    break;
                case 4:
                    if (ResetVelocityOnGroundPound) {  Player.ResetVelocity(); }
                    Player.Velocity += new Vector2(0, GroundPoundSpeed);
                    CurrentCooldown = cooldown;
                    break;
                case 5:
                    Vector2 MousePos = GetGlobalMousePosition();
                    Vector2 Distance = Player.Position - MousePos;
                    if (DistanceDontMatter) { Distance = Distance.Normalized(); }
                    if(ResetVelocityOnPush) { Player.ResetVelocity(); }
                    if (Pull) { Distance *= -1; }
                    if(Distance.Length() > MaxDistance) { Distance = Distance.Normalized() * MaxDistance; }
                    Player.Velocity += Distance * PushPower + Distance.Normalized() *BonusPower;
                    CurrentCooldown = cooldown;
                    GD.Print("distnace:" + Distance);
                    break;
                case 6:
                    if(Mathf.Abs(Player.Velocity.X) >= MinChargeSpeed && Player.ChargeBuffer <= 0) 
                    {
                        Player.ChargeBuffer = ChargeTime;
                        Player.ChargeDirection = Math.Sign(Player.Velocity.X);
                        Player.Velocity = new Vector2(ChargeSpeed * Math.Sign(Player.Velocity.X), Player.Velocity.Y);
                        CurrentCooldown = cooldown;
                    }
                    break;
                case 7:
                    var bullet = ResourceLoader.Load<PackedScene>("res://scenes/RocketJump.tscn"); //todo brací null WHY ?"!
                    var bulletI = bullet.Instantiate();
                    GetNode("/root").AddChild(bulletI);

                    Vector2 MouseLook =  GetGlobalMousePosition() - Player.Position;
                    Bullet bulletReal = bulletI.GetNode<Bullet>(".");
                    bulletReal.Position = Player.Position;
                    bulletReal.Velocity = MouseLook.Normalized() * BulletSpeed;
                    CurrentCooldown = cooldown;
                    break;
                case 8:
                    //chargeable dash
                    break;

            }
        }
        private void ShowCooldown()
        {
            if (CurrentCooldown <= 0) { C.Text = ""; }
            else
            {
                C.Text = CurrentCooldown.ToString();
            }
        }
        private void ChangePower()
        {
            SelectedPower++;
            if (SelectedPower == MaxPower)
            {
                SelectedPower = 0;
            }
            CurrentCooldown = 0;

            switch (SelectedPower)
            {
                case 1:
                    Name.Text = "velocity";
                    break;
                case 2:
                    Name.Text = "dash";
                    break;
                case 3:
                    Name.Text = "sandevistan";
                    break;
                case 4:
                    Name.Text = "Ground Pound";
                    break;
                case 5:
                    Name.Text = "Push";
                    break;
                case 6:
                    Name.Text = "Charge";
                    break;
                case 7:
                    Name.Text = "Explosion";
                    break;
                case 8:
                    Name.Text = "chargeable dash";
                    Player.fuelBuffer = MaxFuel;
                    break;
                default:
                    Name.Text = " ";
                    break;
            }
        }
        public void Refresh()
        {
            CurrentCooldown = 0;
            Player.fuelBuffer = MaxFuel;
        }
        public void RefreshOnFloor()
        {
            if (RefreshOnFloorv) { Refresh(); }
        }
        
        
    }
}
