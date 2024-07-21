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
        //------velocity
        float VelocityPower = 90;
        float CooldwonVelocity = 7;
        bool RefreshOnFloorVelocity = true;
        float VelocityYMult = 1;
        bool ResetVelovityOnActiveVel = true; //nastaví hráčovu velocity na nula při aktivaci
        //--------dash
        public float DashTime = 3;
        public float DashPower = 20;
        public bool SameSpeed = true;
        private float DashCoolDOwn = 6;
        public bool ResetDashOnFloot = true;
        //----------UI nebo spíš jeho karikatura
        Label Name;
        Label C;
        //--------------Logic
        player Player;
        float CurrentCooldown = 0;
        int SelectedPower = 0;
        int MaxPower = 3;
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
            if (Input.IsActionJustPressed("ctrl") && CurrentCooldown <= 0 && SelectedPower != 0)
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
                    CurrentCooldown = CooldwonVelocity;
                    break;
                case 2:
                    Player.ResetVelocity();
                    Player.DashDirection = Player.GetInputVector();
                    Player.DashTimeBuffer = DashTime;
                    CurrentCooldown = DashCoolDOwn;
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
                default:
                    Name.Text = " ";
                    break;
            }
        }
        public void Refresh()
        {
            CurrentCooldown = 0;
        }
        public void RefreshOnFloor()
        {
            switch (SelectedPower)
            {
                case 1:
                    if (RefreshOnFloorVelocity) { Refresh(); }
                    break;
                case 2:
                    if(ResetDashOnFloot) { Refresh(); }
                    break;

                default: break;
            }
        }
        
        
    }
}
