
using System.Collections.Generic;
using InControl;
using UnityEngine;

public enum EControllerBtns
{
    None,
    Sure,
    Cancel,

    Left,
    Right,
    Up,
    Down,

	Shoot,
	Explosion,
	SlowMove,

	TurnLeft,
	TurnRight,
	TurnUp,
	TurnDown,
}

public class ControllerActions : PlayerActionSet
{
    private Dictionary<EControllerBtns, PlayerAction> _joyActions = new Dictionary<EControllerBtns, PlayerAction>();
    public PlayerTwoAxisAction Move;
    public PlayerTwoAxisAction Turn;


	//摇杆按钮定义
    public ControllerActions()
	{
	    _joyActions[EControllerBtns.Sure] = CreatePlayerAction("Sure");
	    _joyActions[EControllerBtns.Cancel] = CreatePlayerAction("Cancel");

		_joyActions[EControllerBtns.Shoot] = CreatePlayerAction("Shoot");
		_joyActions[EControllerBtns.Explosion] = CreatePlayerAction("Explosion");
		_joyActions[EControllerBtns.SlowMove] = CreatePlayerAction("SlowMove");


		var left = CreatePlayerAction("Move Left");
        var right = CreatePlayerAction("Move Right");
        var up = CreatePlayerAction("Move Up");
        var down = CreatePlayerAction("Move Down");

        Move = CreateTwoAxisPlayerAction(left, right, down, up);
	    _joyActions[EControllerBtns.Left] = left;
	    _joyActions[EControllerBtns.Right] = right;
	    _joyActions[EControllerBtns.Up] = up;
	    _joyActions[EControllerBtns.Down] = down;

        var turnLeft = CreatePlayerAction("Turn Left");
	    var turnRight = CreatePlayerAction("Turn Right");
	    var turnUp = CreatePlayerAction("Turn Up");
	    var turnDown = CreatePlayerAction("Turn Down");
	    Turn = CreateTwoAxisPlayerAction(turnLeft, turnRight, turnDown, turnUp);

	    _joyActions[EControllerBtns.TurnLeft] = turnLeft;
	    _joyActions[EControllerBtns.TurnRight] = turnRight;
	    _joyActions[EControllerBtns.TurnUp] = turnUp;
	    _joyActions[EControllerBtns.TurnDown] = turnDown;
    }

    public PlayerAction Get(EControllerBtns e)
    {
        if (_joyActions.TryGetValue(e, out var action))
        {
            return action;
        }
        Debug.LogError("按键未注册："+e);
        return null;
    }

	//摇杆行为绑定按键
	public static ControllerActions CreateWithDefaultBindings()
	{
		var p = new ControllerActions();

        //sure
        p.Get(EControllerBtns.Sure).AddDefaultBinding(Key.Return);
		p.Get(EControllerBtns.Sure).AddDefaultBinding(Key.Z);
		p.Get(EControllerBtns.Sure).AddDefaultBinding(InputControlType.Action1);

        //back
        p.Get(EControllerBtns.Cancel).AddDefaultBinding(Key.Escape);
		p.Get(EControllerBtns.Cancel).AddDefaultBinding(Key.X);
		p.Get(EControllerBtns.Cancel).AddDefaultBinding(InputControlType.Action2);

		//shoot
		p.Get(EControllerBtns.Shoot).AddDefaultBinding(Key.Z);
		p.Get(EControllerBtns.Shoot).AddDefaultBinding(InputControlType.Action1);

		//slowmove
		p.Get(EControllerBtns.SlowMove).AddDefaultBinding(Key.LeftShift);
		p.Get(EControllerBtns.SlowMove).AddDefaultBinding(InputControlType.Action3);

		//move
		p.Get(EControllerBtns.Up).AddDefaultBinding( Key.UpArrow);
		p.Get(EControllerBtns.Down).AddDefaultBinding( Key.DownArrow);
		p.Get(EControllerBtns.Left).AddDefaultBinding( Key.LeftArrow);
		p.Get(EControllerBtns.Right).AddDefaultBinding( Key.RightArrow);

	    p.Get(EControllerBtns.Up).AddDefaultBinding(InputControlType.LeftStickUp);
	    p.Get(EControllerBtns.Down).AddDefaultBinding(InputControlType.LeftStickDown);
        p.Get(EControllerBtns.Left).AddDefaultBinding( InputControlType.LeftStickLeft );
		p.Get(EControllerBtns.Right).AddDefaultBinding( InputControlType.LeftStickRight );

        //turn
	    p.Get(EControllerBtns.TurnLeft).AddDefaultBinding(InputControlType.RightStickLeft);
	    p.Get(EControllerBtns.TurnRight).AddDefaultBinding(InputControlType.RightStickRight);
	    p.Get(EControllerBtns.TurnUp).AddDefaultBinding(InputControlType.RightStickUp);
	    p.Get(EControllerBtns.TurnDown).AddDefaultBinding(InputControlType.RightStickDown);


        p.ListenOptions.IncludeUnknownControllers = true;
	    p.ListenOptions.MaxAllowedBindings = 4;
	    //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
	    //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
	    p.ListenOptions.UnsetDuplicateBindingsOnSet = true;
	    //playerActions.ListenOptions.IncludeMouseButtons = true;
	    //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
	    //playerActions.ListenOptions.IncludeMouseButtons = true;
	    //playerActions.ListenOptions.IncludeMouseScrollWheel = true;

        /*
	    playerActions.ListenOptions.OnBindingFound = (action, binding) => {
	        if (binding == new KeyBindingSource(Key.Escape))
	        {
	            action.StopListeningForBinding();
	            return false;
	        }
	        return true;
	    };*/


        p.ListenOptions.OnBindingAdded += ( action, binding ) => {
			Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
		};

		p.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
			Debug.Log( "Binding rejected... " + reason );
		};

		return p;
	}
}