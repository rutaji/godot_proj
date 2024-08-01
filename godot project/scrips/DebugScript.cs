using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dream.scrips
{
    internal partial class DebugScript : Node
    {
        int SelectedMenu = 0;
        player p;
        //-----menu1
        BoxContainer[] Menu = new BoxContainer[4];
        Label PositionX;
        Label PositionY;
        Label VelocityX;
        Label VelocityY;
        Label State;
        //----menu2
        Label DirectionX;
        Label DirectionY;
        Label JumpPressed;
        Label JumpRelleased;
        //----menu3
        Label Fuel;

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("F1")) { SetSelected(1); }
            else if (Input.IsActionJustPressed("F2")) { SetSelected(2); }
            else if (Input.IsActionJustPressed("F3")) { SetSelected(3); }
            else if (Input.IsActionJustPressed("F4")) { SetSelected(4); }

            switch (SelectedMenu)
            {
                case 1:
                    PositionX.Text = "Position X: " + p.Position.X.ToString();
                    PositionY.Text = "Position Y: " + p.Position.Y.ToString();
                    VelocityX.Text = "Velocity X: " + p.Velocity.X.ToString();
                    VelocityY.Text = "Velocity Y: " + p.Velocity.Y.ToString();
                    State.Text = p.state.ToString();
                    break;
                case 2:
                    DirectionX.Text = "Direction X" + p.InputXDirection.ToString();
                    DirectionY.Text = "DirectionY Y" + p.InputYDirection.ToString();
                    JumpPressed.Text = "Jump just pressed" + p.just_jump_pressed.ToString();
                    JumpRelleased.Text ="Jump just release" + p.just_jump_release.ToString();
                    break;
                case 3:
                    Fuel.Text = p.fuelBuffer.ToString();
                    break;

            }
            
        }
        private void SetSelected(int f) 
        {
            if (SelectedMenu != 0) { Menu[SelectedMenu - 1].Visible = false; }
            SelectedMenu = f!=SelectedMenu ? f : 0;
            if (SelectedMenu != 0) { Menu[SelectedMenu - 1].Visible = true; }

        }
        public override void _Ready()
        {
            Menu[0] = GetNode<BoxContainer>("/root/World/Player/Camera2D/Menu1");
            Menu[1] = GetNode<BoxContainer>("/root/World/Player/Camera2D/Menu2");
            Menu[2] = GetNode<BoxContainer>("/root/World/Player/Camera2D/Menu3");
            Menu[3] = GetNode<BoxContainer>("/root/World/Player/Camera2D/Menu4");


            PositionX = GetNode<Label>("/root/World/Player/Camera2D/Menu1/PositionX");
            PositionY = GetNode<Label>("/root/World/Player/Camera2D/Menu1/PositionY");
            VelocityX = GetNode<Label>("/root/World/Player/Camera2D/Menu1/VelocityX");
            VelocityY = GetNode<Label>("/root/World/Player/Camera2D/Menu1/VelocityY");
            State = GetNode<Label>("/root/World/Player/Camera2D/Menu1/State");
            p = GetNode<player>("/root/World/Player");

            DirectionX = GetNode<Label>("/root/World/Player/Camera2D/Menu2/DirectionX");
            DirectionY = GetNode<Label>("/root/World/Player/Camera2D/Menu2/DirectionY");
            JumpPressed = GetNode<Label>("/root/World/Player/Camera2D/Menu2/JumpPressed");
            JumpRelleased = GetNode<Label>("/root/World/Player/Camera2D/Menu2/JumpRelease");

            Fuel = GetNode<Label>("/root/World/Player/Camera2D/Menu3/Fuel");
        }
        

    }
}
