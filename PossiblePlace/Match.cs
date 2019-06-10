using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PossiblePlace
{
    public enum MatchResult { NotPlayedYet, HomeTeamWon, Tie, AwayTeamWon }
    class Match
    {
        public Team HomeTeam { get; private set; }
        public Team AwayTeam { get; private set; }
        private MatchResult _result;
        public MatchResult Result
        {
            get { return _result; }
            set
            {
                if (value != _result)
                {
                    switch (_result)
                    {
                        case MatchResult.HomeTeamWon:
                            HomeTeam.AddPoints(-3);
                            break;
                        case MatchResult.Tie:
                            HomeTeam.AddPoints(-1);
                            AwayTeam.AddPoints(-1);
                            break;
                        case MatchResult.AwayTeamWon:
                            AwayTeam.AddPoints(-3);
                            break;
                        default: break;
                    }
                    switch (value)
                    {
                        case MatchResult.HomeTeamWon:
                            HomeTeam.AddPoints(3);
                            break;
                        case MatchResult.Tie:
                            HomeTeam.AddPoints(1);
                            AwayTeam.AddPoints(1);
                            break;
                        case MatchResult.AwayTeamWon:
                            AwayTeam.AddPoints(3);
                            break;
                    }
                    _result = value;
                }
            }
        }

        public Match(Team homeTeam, Team awayTeam, MatchResult result = MatchResult.NotPlayedYet)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            Result = result;
        }


    }
}
