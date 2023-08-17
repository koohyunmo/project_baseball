using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define 
{
	public enum UIEvent
	{
		Click,
		Pressed,
		PointerDown,
		PointerUp,
		BeginDrag,
		Drag,
		EndDrag,
	}

	public enum GameState
	{
		Home,
		Ready,
		InGround,
		End,
	}

	public enum Scene
	{
		Unknown,
		DevScene,
		GameScene,
	}

	public enum Sound
	{
		Bgm,
		Effect,
	}

	public enum CreatureState
	{
		Idle,
		Swing,
		Out
	}

    public enum ThrowType
    {
        FastBall,
        Curve,
        Slider,
        ChangUp,
        Sinker,
        ExCurve,
        NormalCurve,
        Knuckleball,
        TwoSeamFastball,
        Splitter,
        COUNT
    }


    public enum League
    {
        Major,
        Mainor,
        Amateur,
        SemiPro,
    }


}
