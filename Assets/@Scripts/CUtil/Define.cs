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
        NONE,
        Tree,
		Alu,
		Sp1,
		Sp2,
		Sp3,
		Sp4,
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

	public enum ChallengeProc
	{
		None,
		Fail,
		Complete
	}

    public static readonly Color Bronze = new Color(205f / 255f, 127f / 255f, 50f / 255f, (155f/255f));
    public static readonly Color Silver = new Color(192f / 255f, 192f / 255f, 192f / 255f, (155f / 255f));
    public static readonly Color Gold = new Color(255f / 255f, 223f / 255f, 86f / 255f, 1f);
    public static readonly Color Platinum = new Color(8f / 255f, 232f / 255f, 222f / 255f, 1f);
    public static readonly Color Diamond = new Color(185f / 255f, 242f / 255f, 255f / 255f, (155f / 255f));
    public static readonly Color Master = new Color(123f / 255f, 31f / 255f, 162f / 255f, 1f);

	public enum HitType
	{
		A,
		B,
		C,
		D
	}





}
