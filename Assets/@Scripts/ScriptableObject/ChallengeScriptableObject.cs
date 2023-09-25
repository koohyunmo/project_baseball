using UnityEngine;
using static Define;


[CreateAssetMenu(fileName = "ChallengeData", menuName = "Challenge/Create New Challenge")]
public class ChallengeScriptableObject : ScriptableObject
{
    public int orderID;
    public string key;
    public ChallengeType mode;
    public League league;
    public int score;
    public float speed;
    [TextArea]
    public string desc;

    public void GenerateDescription()
    {
        string colorCode = Utils.ColorToHex(Utils.GetColor(league));

        switch (mode)
        {
            case ChallengeType.ScoreMode:
                desc = $"Achieve exactly {score} points.\n<color=#{colorCode}>{league} League</color> ";
                break;
            case ChallengeType.HomeRunMode:
                desc = $"Hit {score} home runs to succeed. \n <color=#{colorCode}>{league} League</color>";
                break;
            case ChallengeType.RealMode:
                desc = $"Hit consecutively {score} times. \n <color=#{colorCode}>{league} League</color>";
                break;
        }
    }
}
