using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PossiblePlace
{
    class Team
    {
        public string Name {get;private set;}
        public int Points { get; private set; }
        public int MatchesLeft { get; set; }

        public Team(string name, int points)
        {
            Name = name;
            Points = points;
            MatchesLeft = 0;
        }

        public Team(Team toCopy)
        {
            Name = toCopy.Name;
            Points = toCopy.Points;
            MatchesLeft = toCopy.MatchesLeft;
        }

        public void AddPoints(int valueToAdd)
        {
            Points += valueToAdd;
        }
    }
}
