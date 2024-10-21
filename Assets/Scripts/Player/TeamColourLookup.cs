using UnityEngine;


// MAke SO asset that Tanks can reference

[CreateAssetMenu(fileName ="NewTeamColourLookup", menuName="Team Colour Lookup")]
public class TeamColourLookup : ScriptableObject
{

    [SerializeField] private Color[] teamColours;


    public Color GetTeamColour(int teamIndex)
    {
        if(teamIndex < 0 || teamIndex >= teamColours.Length)
        {
            // When not using teams
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            //teams
            return teamColours[teamIndex];

        }


    }
}
