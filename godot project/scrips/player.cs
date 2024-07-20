using dream.scrips;
using dream.scrips.enums;
using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class player : CharacterBody2D
{
	//----new

	//----------------speed

	public float maxSpeed = 90;
	public float acceleration = 100;
	public float turningAcceleration = 300; //accelerace pokud hráč otáčí (může trvat několik snímků)
	public float deceleration = 170;

	//todo
	public float airXaccelerationMult = 1;
    public float jumpXaccelerationMult = 1;
    public float hangXaccelerationMult = 1;
    public float fallXaccelerationMult = 1;
    public float standingXaccelerationMult = 1;

    public float airXDeaccelerationMult = 1;
    public float standingXDEaccelerationMult = 1;
    public float jumpXDEaccelerationMult = 1;
    public float hangXDEaccelerationMult = 1;
    public float fallXDEaccelerationMult = 1;

    //---------------------gravity
    public float gravityAcceleration = 100;
    public float gravity_max = 210; 
	public bool coyoteDontFall = false;//gravity doenst affect player while coyote time

    //todo
    public float StandingGravityMult = 1;
	public float JumpGravityMultiplayer = 1;
    public float hang_gravity_mult = 0.9f;
	public float fall_gravity_mult = 1.9f;

    //-------------------jump

    public float jump_force = 90; //NewVelocity.Y = -jump_force; 
    public float jump_cut = 0.25f; // % okolik se zmenší velocity když hráč zruší skok (1 == okamžitý stop ; větší než 1 => hráč začne padat dolů rychlostí jump_cut * velocity.y) NewVelocity.Y += (jump_cut * NewVelocity.Y);

	public float jump_hang_theshold = 3;//pokud abs(velovity.Y) < jump_hang_theshold => hang

    public int DoubleJump = 1;
    public float DoubleJumpForceMult = 1;//násobí jump_force => síla double jump

    //-------hitCeil myslim že nic nedělá
    public float hitCeilBonus = 0; // přidá se k Velocity.Y pokud hráč narazí do stropu
    public float ceilTimer = 3;//cooldown pro hitCeilBonus resetuje se při dotyku země


    //--------------timer
    public float jump_coyote = 0.08f;
	public float jump_buffer = 0.2f;

	//-----------------DO NOT CHANGE
	float jump_coyote_timer = 0;
	float jump_buffer_timer = 0;
    float ceil_buffer_timer = 0;
	public PLayerState state = PLayerState.falling;
	Vector2 NewVelocity;

	public int LastXDirection = 0;
    public int InputXDirection = 0;
	public int LastYDirection = 0;
	public int InputYDirection = 0;
    public bool just_jump_pressed = false;
	public bool just_jump_release = false;
    public int DoubleJumpBuffer = 0;


    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    Power power;
	public Vector2 RespawnPosition = Vector2.Zero;

    public override void _PhysicsProcess(double delta)
	{
        NewVelocity = Velocity;
        DoInputs();
        Jumping();
        state = CheckState();
        ApplyGravity(delta);
        XMovement(delta);
		DoTimers(delta);
        //done
        Velocity = NewVelocity;
        power.DoPowers(delta);
        


		#region collisions
		MoveAndSlide();
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
            KinematicCollision2D collision = GetSlideCollision(i);
			Vector2I pos = (TileManager.getPosition(collision.GetColliderRid()));
            int tileVal = TileManager.GetTileValue(pos);
			if (tileVal != 0) 
			{
				SolveCollision(tileVal, pos);
			}
		}
        Vector2I position = TileManager.getPosition(Position);
        int t = TileManager.GetTileValue(position);
        SolvePossition(t, position);

        #endregion
    }
    public void ResetVelocity() 
    {
        Velocity = Vector2.Zero;
    }
	private void DoInputs() 
	{
        //X
        if (Input.IsActionPressed("ui_left"))
        {
            if (LastXDirection == -1 || Input.IsActionJustPressed("ui_left"))
            {
                LastXDirection = -1;
                InputXDirection = -1;
            }
            else if (LastXDirection == 1 || Input.IsActionJustPressed("ui_right"))
            {
                LastXDirection = 1;
                InputXDirection = 1;
            }
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            LastXDirection = 1;
            InputXDirection = 1;
        }
        else { InputXDirection = 0; }
        //Y
        if (Input.IsActionPressed("ui_up"))
        {
            if (LastYDirection == -1 || Input.IsActionJustPressed("ui_up"))
            {
                LastYDirection = -1;
                InputYDirection = -1;
            }
            else if (LastYDirection == 1 || Input.IsActionJustPressed("ui_down"))
            {
                LastYDirection = 1;
                InputYDirection = 1;
            }
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            LastYDirection = 1;
            InputYDirection = 1;
        }
        else { InputYDirection = 0; }
        just_jump_pressed = Input.IsActionJustPressed("Space");
        just_jump_release = Input.IsActionJustReleased("Space");

    }
	private void Jumping() 
	{
        // jump
        if (IsOnFloor())
        {
            RefreshOnFloor();

        }
        if (just_jump_pressed)
        {
            jump_buffer_timer = jump_buffer;
        }

        if (jump_buffer_timer > 0 && (jump_coyote_timer > 0 || CanJump()))
        {
            state = PLayerState.jumping;
            jump_coyote_timer = 0;
            jump_buffer_timer = 0;
            NewVelocity.Y = -jump_force;

        }
        else if (just_jump_pressed && DoubleJumpBuffer > 0)//todo možná upravit aby se neaktivoal nechtěně
        {
            DoubleJumpBuffer -= 1;
            NewVelocity.Y = -jump_force * DoubleJumpForceMult;
        }

        if (just_jump_release && Velocity.Y < 0)
        {
            NewVelocity.Y += (jump_cut * NewVelocity.Y);
        }
        if (IsOnCeiling() && ceil_buffer_timer < 0) 
        { 
            NewVelocity.Y -= hitCeilBonus;
            ceil_buffer_timer = ceilTimer;
        }
    }
    private void RefreshOnFloor() 
    {
        state = PLayerState.standing;
        jump_coyote_timer = jump_coyote;
        ceil_buffer_timer = 0;
        DoubleJumpBuffer = DoubleJump;
        power.RefreshOnFloor();
    }
    private PLayerState CheckState() 
    {
        if (IsOnFloor()) { return PLayerState.standing; }
        if(Math.Abs(Velocity.Y) <= jump_hang_theshold) {  return PLayerState.hang; }
        if(Velocity.Y < 0) { return PLayerState.jumping; }
        return PLayerState.falling;
    }
    private bool CanJump() 
    {
        return state == PLayerState.standing; 
    }
    private void ApplyGravity(double delta)
    {
        //gravity
        if (coyoteDontFall && jump_coyote_timer > 0 || !CanAplyGravity()) 
        {
            return;
        }
        if(NewVelocity.Y >= gravity_max) { GD.Print("max gravity reach");return;}
        float apllidedGravity = (float)(gravityAcceleration * delta);
        switch (state) 
        {
            case PLayerState.standing://enemy stand !
                apllidedGravity *= StandingGravityMult;
                break;
            case PLayerState.jumping:
                apllidedGravity *= JumpGravityMultiplayer;
                break;
            case PLayerState.falling:
                apllidedGravity *= fall_gravity_mult;
                break;
            case PLayerState.hang:
                apllidedGravity *= hang_gravity_mult;
                break;
        }
        if (apllidedGravity + NewVelocity.Y > gravity_max)
        {
            NewVelocity.Y = gravity_max;
        }
        else 
        {
            NewVelocity.Y += apllidedGravity;
        }

    }
	
	private void XMovement(double delta)
	{
        //x movement
        if (InputXDirection == 0)
        {
            float deacelerationMult = 0;
            switch (state)
            {
                case PLayerState.standing:
                    deacelerationMult = standingXDEaccelerationMult;
                    break;
                case PLayerState.jumping:
                    deacelerationMult = jumpXDEaccelerationMult * airXDeaccelerationMult;
                    break;
                case PLayerState.falling:
                    deacelerationMult = fallXDEaccelerationMult * airXDeaccelerationMult;
                    break;
                case PLayerState.hang:
                    deacelerationMult = hangXDEaccelerationMult * airXDeaccelerationMult;
                    break;
            }
            NewVelocity.X = new Vector2(NewVelocity.X, 0).MoveToward(new Vector2(0, 0), (float)(deceleration * deacelerationMult * delta)).X;
            return;
        }
        if (Math.Sign(NewVelocity.X) == InputXDirection && Math.Abs(NewVelocity.X) > maxSpeed) //neotáčí se překoval max rychlost
        {
            return;
        }
        float speedMult = 0;
        switch (state)
        {
            case PLayerState.standing:
                speedMult = standingXaccelerationMult;
                break;
            case PLayerState.jumping:
                speedMult = jumpXaccelerationMult * airXaccelerationMult;
                break;
            case PLayerState.falling:
                speedMult = fallXaccelerationMult * airXaccelerationMult;
                break;
            case PLayerState.hang:
                speedMult = hangXaccelerationMult * airXaccelerationMult;
                break;
        }
        float acc = Math.Sign(NewVelocity.X) == InputXDirection ? acceleration : turningAcceleration;
        float final_speedTurn = NewVelocity.X + (float)(InputXDirection * acc * speedMult * delta);
        if(Math.Abs(final_speedTurn) > maxSpeed) { NewVelocity.X = InputXDirection * maxSpeed;return; }
        NewVelocity.X = final_speedTurn;
    }
    private void DoTimers(double delta)
    {
        jump_coyote_timer -= (float)delta;
        jump_buffer_timer -= (float)delta;
        ceil_buffer_timer -= (float)delta;
    }



    #region random shit
    public void SolvePossition(int tileVal,Vector2I pos) 
	{
        switch (tileVal)
        {
            case 2:
                SetSpawn(pos);
                break;
			case 4:
				Refresh();
				break;
			




        }
    }
	public bool CanAplyGravity() 
	{
		return 5 != TileManager.GetTileValue(TileManager.getPosition(Position));
    }
    public void SolveCollision(int tileVal, Vector2I pos)
    {
        switch (tileVal)
        {
            case 1:
                Die();
                break;
			case 6:
				Vector2 diff = Position - TileManager.getGlobal(pos);
				Velocity = diff.Normalized() * VariableManagger.bouncyBlock;
				break;


        }
    }
    public void SetSpawn(Vector2I pos) 
	{
		TileManager.RemoveSpawn(RespawnPosition);
		TileManager.SetSpawn(pos);
		RespawnPosition = Position;
	}
	public void Die() 
	{
		Position = RespawnPosition;
	}
	public void Refresh() 
	{
        DoubleJumpBuffer = DoubleJump;
        power.Refresh();
	}
    #endregion
    public override void _Ready()
    {
        power = GetNode<Power>("/root/World/Player/Power");
        base._Ready();
    }


}
