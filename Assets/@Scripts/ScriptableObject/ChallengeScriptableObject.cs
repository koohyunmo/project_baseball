using UnityEngine;
using static Define;


[CreateAssetMenu(fileName = "ChallengeData", menuName = "Challenge/Create New Challenge")]
public class ChallengeScriptableObject : ScriptableObject
{
    public int id;
    public ChallengeType mode;
    public int score;
    public float speed;
    [TextArea]
    public string desc;

    public void GenerateDescription()
    {
        switch (mode)
        {
            case ChallengeType.Score:
                desc = $"Achieve exactly {score} points.";
                break;
            case ChallengeType.HomeRun:
                desc = $"Hit {score} home runs to succeed.";
                break;
            case ChallengeType.RealMode:
                desc = $"Hit consecutively {score} times.";
                break;
        }
    }
}
