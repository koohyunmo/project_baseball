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
		TEST,
        Amateur,
        SemiPro,
        Mainor,
        Major,
		COUNT
    }

	public enum Prefabs
	{
		None,
		Bat,
		Ball,
	}

	public enum BatPosition
	{
        Left,
        Right,
    }

    public enum BatType
	{
        Normal,
        Strong,
        Count
    }

	public enum ItemType
	{
		None,
		Ball,
		Bat,
		Background,
	}


	public struct ReplayData
	{
        public Vector3 position;
        public float time;
    }

}
