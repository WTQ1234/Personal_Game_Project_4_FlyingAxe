using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;

public class TeamController : BaseController
{
    public int teamId = 1;

    public void SetTeam(int _teamId)
    {
        teamId = _teamId;
        Owner._OnSetTeam();
    }

    public void SetTeam(TeamController otherTeamController)
    {
        teamId = otherTeamController.teamId;
        Owner._OnSetTeam();
    }

    public bool DetectTeam(int _teamId)
    {
        return (teamId == _teamId);
    }
}
