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
        Bronze,
        Silver,
        Gold,
        Platinum,
		Diamond,
        Master,
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
		BALL,
		BAT,
		Background,
		SKILL
	}


	public struct CustomReplayData
	{
        public Vector3 position;
        public float time;
    }


	public enum GameMode
    {
		None,
		Normal,
		Challenge
	}

	public enum HawkeyeLevel
	{
		ZERO = 0,
		ONE = 10,
		TWO = 20,
		THREE = 30,
		FOUR = 40,
		FIVE = 50,
		SIX = 60,
		SEVEN = 70,
        EIGHT = 80,
		NINE = 90,
		TEN = 95
	}

	public enum Grade
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
	}
	public enum ChallengeType
	{
		None,
        Score,
        HomeRun,
        RealMode
    }


    public enum ScollViewType
    {
        Ball,
        Bat,
        Skill
    }
}
