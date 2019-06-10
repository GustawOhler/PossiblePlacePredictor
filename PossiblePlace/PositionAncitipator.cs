using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PossiblePlace
{
    class PositionAncitipator
    {
        List<Team> listOfTeams;
        int indexOfChosenTeam;
        List<Match> listOfMatches;

        public PositionAncitipator()
        {
            listOfTeams = IOFunctions.LoadTeams();
            listOfMatches = IOFunctions.LoadMatches(listOfTeams);
            indexOfChosenTeam = IOFunctions.ChooseTeam(listOfTeams);
        }

        List<List<MatchResult>> GetAllByIterating(List<MatchResult> listToWorkOn)
        {
            List<List<MatchResult>> listOfNextPossibilities = null;
            if (listToWorkOn.Count > 1)
            {
                listOfNextPossibilities = GetAllByIterating(listToWorkOn.GetRange(1, listToWorkOn.Count - 1));
            }

            List<List<MatchResult>> listOfCurrentPossibilities = new List<List<MatchResult>>();
            for (MatchResult i = MatchResult.HomeTeamWon; i<=MatchResult.AwayTeamWon; i++)
            {
                if (listOfNextPossibilities != null)
                {
                    for (int j = 0; j<listOfNextPossibilities.Count; j++)
                    {
                        List<MatchResult> newPossibility = new List<MatchResult>(listOfNextPossibilities[j]);
                        newPossibility.Insert(0, i);
                        listOfCurrentPossibilities.Add(newPossibility);
                    }
                }
                else
                {
                    List<MatchResult> newPossibility = new List<MatchResult>();
                    newPossibility.Add(i);
                    listOfCurrentPossibilities.Add(newPossibility);
                }
            }
            return listOfCurrentPossibilities;
        }

        int ConvMatchResultToInt(MatchResult target)
        {
            return (int)target;
        }

        MatchResult ConvIntToMatchResult(int target)
        {
            return (MatchResult)target;
        }

        List<MatchResult> NextPossibilityState(List<MatchResult> possibilityList)
        {
            List<int> castedPossList = possibilityList.ConvertAll(new Converter<MatchResult, int>(ConvMatchResultToInt));
            bool shouldIncrease = false;
            for (int i = castedPossList.Count - 1; i>=0; i--)
            {
                if (i == castedPossList.Count - 1)
                {
                    castedPossList[i]++;
                    if (castedPossList[i] > (int)MatchResult.AwayTeamWon)
                    {
                        castedPossList[i] = (int)MatchResult.HomeTeamWon;
                        shouldIncrease = true;
                    }
                }
                else
                {
                    if (shouldIncrease)
                    {
                        castedPossList[i]++;
                        if (castedPossList[i] > (int)MatchResult.AwayTeamWon)
                        {
                            castedPossList[i] = (int)MatchResult.HomeTeamWon;
                            shouldIncrease = true;
                        }
                        else
                            shouldIncrease = false;
                    }
                    else
                        break;
                }
            }
            return castedPossList.ConvertAll(new Converter<int, MatchResult>(ConvIntToMatchResult));
        }

        bool CheckIfReachedEnd(List<MatchResult> possibilityList)
        {
            bool reachedEnd = true;
            foreach (MatchResult mr in possibilityList)
            {
                if (mr != MatchResult.AwayTeamWon)
                {
                    reachedEnd = false;
                    break;
                }
            }
            return reachedEnd;
        }

        public List<Team> GetTeamsOutOfRangeForPositive(Team target, List<Team> workingTeamList)
        {
            List<Team> teamsOutOfRange = new List<Team>();
            foreach(Team t in workingTeamList)
            {
                if (t.Points < target.Points || t.Points > target.Points + target.MatchesLeft * 3)
                    teamsOutOfRange.Add(t);
            }
            //foreach(Team t in teamsOutOfRange)
              //  Console.WriteLine("Team out of range: " + t.Name);
            return teamsOutOfRange;
        }

        public List<Team> GetTeamsOutOfRangeForNegative(Team target, List<Team> workingTeamList)
        {
            List<Team> teamsOutOfRange = new List<Team>();
            foreach (Team t in workingTeamList)
            {
                if (t.Points + t.MatchesLeft * 3 < target.Points || t.Points > target.Points)
                    teamsOutOfRange.Add(t);
            }
            //foreach(Team t in teamsOutOfRange)
            //  Console.WriteLine("Team out of range: " + t.Name);
            return teamsOutOfRange;
        }

        public List<Match> GetIrrevelantMatches(List<Team> teamsOutOfRange, List<Match> workingMatchList)
        {
            //List<Team> teamsOutOfRange = GetTeamsOutOfRange(workingTeamList[indexOfChosenTeam], workingTeamList);
            List<Match> irrelevantMatches = new List<Match>();
            for (int i = workingMatchList.Count - 1; i>=0; i--)
            {
                if (teamsOutOfRange.Contains(workingMatchList[i].HomeTeam) && teamsOutOfRange.Contains(workingMatchList[i].AwayTeam))
                {
                    irrelevantMatches.Add(workingMatchList[i]);
                    workingMatchList.RemoveAt(i);
                }
            }
            //Console.WriteLine("Irrelevant matches:");
            //foreach (Match m in irrelevantMatches)
            //{
            //    Console.WriteLine(m.HomeTeam.Name + " vs " + m.AwayTeam.Name);
            //}
            return irrelevantMatches;
        }

        public List<Match> GetMatchesToSet(List<Team> workingTeamList, List<Match> workingMatchList, List<Team> teamsOutOfRange)
        {
            List<Match> matches = new List<Match>();
            for (int i = workingMatchList.Count - 1; i >= 0; i--)
            {
                if ((teamsOutOfRange.Contains(workingMatchList[i].HomeTeam) && !teamsOutOfRange.Contains(workingMatchList[i].AwayTeam)) || (!teamsOutOfRange.Contains(workingMatchList[i].HomeTeam) && teamsOutOfRange.Contains(workingMatchList[i].AwayTeam))) {
                    matches.Add(workingMatchList[i]);
                    workingMatchList.RemoveAt(i);
            }
            }
            //Console.WriteLine("Matches to set:");
            //foreach (Match m in matches)
            //{
            //    Console.WriteLine(m.HomeTeam.Name + " vs " + m.AwayTeam.Name);
            //}
            return matches;
        }

        public List<Match> GetMatchesOfTarget(List<Team> workingTeamList, List<Match> workingMatchList)
        {
            List<Match> matches = new List<Match>();
            Team target = workingTeamList[indexOfChosenTeam];
            for (int i = workingMatchList.Count - 1; i >= 0; i--)
            {
                if (workingMatchList[i].HomeTeam == target || workingMatchList[i].AwayTeam == target)
                {
                    matches.Add(workingMatchList[i]);
                    workingMatchList.RemoveAt(i);
                }
                    
            }
            //Console.WriteLine("Target matches:");
            //foreach (Match m in matches)
            //{
            //    Console.WriteLine(m.HomeTeam.Name + " vs " + m.AwayTeam.Name);
            //}
            return matches;
        }

        public List<MatchResult> GetStartingPossibilitiesList(int numberOfMatches)
        {
            List<MatchResult> possibilityList = new List<MatchResult>();
            for (int i = 0; i<numberOfMatches; i++)
            {
                possibilityList.Add(MatchResult.HomeTeamWon);
            }
            return possibilityList;
        }

        public int PositiveAnticipation()
        {
            List<Team> workingTeamList = listOfTeams.ConvertAll(team => new Team(team));
            Team target = workingTeamList[indexOfChosenTeam];
            List<Match> workingMatchList = listOfMatches.ConvertAll(match => new Match(workingTeamList.Find(team => team.Name == match.HomeTeam.Name), workingTeamList.Find(team => team.Name == match.AwayTeam.Name), match.Result));
            List<Team> teamsOutOfRange = GetTeamsOutOfRangeForPositive(workingTeamList[indexOfChosenTeam], workingTeamList);
            List<Match> irrelevantMatches = GetIrrevelantMatches(teamsOutOfRange, workingMatchList);
            List<Match> targetMatches = GetMatchesOfTarget(workingTeamList, workingMatchList);
            List<Match> matchesToSet = GetMatchesToSet(workingTeamList, workingMatchList, teamsOutOfRange);
            foreach (Match m in targetMatches)
            {
                if (m.HomeTeam == target)
                    m.Result = MatchResult.HomeTeamWon;
                else if (m.AwayTeam == target)
                    m.Result = MatchResult.AwayTeamWon;
            }
            foreach(Match m in matchesToSet)
            {
                if (teamsOutOfRange.Contains(m.HomeTeam))
                {
                    m.Result = MatchResult.HomeTeamWon;
                }
                else if (teamsOutOfRange.Contains(m.AwayTeam))
                {
                    m.Result = MatchResult.AwayTeamWon;
                }
            }
            int highestPosition = workingTeamList.Count;
            List<MatchResult> possibilityList = GetStartingPossibilitiesList(workingMatchList.Count);
            bool reachedEnd = false;
            while (true)
            {
                for (int i = 0; i < workingMatchList.Count; i++)
                {
                    workingMatchList[i].Result = possibilityList[i];
                }
                List<Team> sortedList = workingTeamList.OrderByDescending(t => t.Points).ToList();
                int currentTeamPosition = sortedList.IndexOf(target) + 1;
                if (highestPosition > currentTeamPosition)
                    highestPosition = currentTeamPosition;
                if (reachedEnd)
                    break;
                else
                {
                   possibilityList = NextPossibilityState(possibilityList);
                    reachedEnd = CheckIfReachedEnd(possibilityList);
                }
            }
            //foreach(List<MatchResult> currentPossibilityList in ListOfPosibilities)
            //{
            //    for (int i = 0; i<workingMatchList.Count; i++)
            //    {
            //        workingMatchList[i].Result = currentPossibilityList[i];
            //    }
            //    List<Team> sortedList = workingTeamList.OrderByDescending(t => t.Points).ToList();
            //    int currentTeamPosition = sortedList.IndexOf(target) + 1;
            //    if (highestPosition > currentTeamPosition)
            //        highestPosition = currentTeamPosition;
            //}
            return highestPosition;
        }

        public int NegativeAnticipation()
        {
            List<Team> workingTeamList = listOfTeams.ConvertAll(team => new Team(team));
            Team target = workingTeamList[indexOfChosenTeam];
            List<Match> workingMatchList = listOfMatches.ConvertAll(match => new Match(workingTeamList.Find(team => team.Name == match.HomeTeam.Name), workingTeamList.Find(team => team.Name == match.AwayTeam.Name), match.Result));
            List<Team> teamsOutOfRange = GetTeamsOutOfRangeForNegative(workingTeamList[indexOfChosenTeam], workingTeamList);
            List<Match> irrelevantMatches = GetIrrevelantMatches(teamsOutOfRange, workingMatchList);
            List<Match> targetMatches = GetMatchesOfTarget(workingTeamList, workingMatchList);
            List<Match> matchesToSet = GetMatchesToSet(workingTeamList, workingMatchList, teamsOutOfRange);
            foreach (Match m in targetMatches)
            {
                if (m.HomeTeam == target)
                    m.Result = MatchResult.AwayTeamWon;
                else if (m.AwayTeam == target)
                    m.Result = MatchResult.HomeTeamWon;
            }
            foreach (Match m in matchesToSet)
            {
                if (teamsOutOfRange.Contains(m.HomeTeam))
                {
                    m.Result = MatchResult.AwayTeamWon;
                }
                else if (teamsOutOfRange.Contains(m.AwayTeam))
                {
                    m.Result = MatchResult.HomeTeamWon;
                }
            }
            int lowestPosition = 1;
            List<MatchResult> possibilityList = GetStartingPossibilitiesList(workingMatchList.Count);
            bool reachedEnd = false;
            while (true)
            {
                for (int i = 0; i < workingMatchList.Count; i++)
                {
                    workingMatchList[i].Result = possibilityList[i];
                }
                List<Team> sortedList = workingTeamList.OrderByDescending(t => t.Points).ToList();
                int currentTeamPosition = sortedList.IndexOf(target) + 1;
                if (lowestPosition < currentTeamPosition)
                    lowestPosition = currentTeamPosition;
                if (reachedEnd)
                    break;
                else
                {
                    possibilityList = NextPossibilityState(possibilityList);
                    reachedEnd = CheckIfReachedEnd(possibilityList);
                }
            }
            //foreach (List<MatchResult> currentPossibilityList in ListOfPosibilities)
            //{
            //    for (int i = 0; i < workingMatchList.Count; i++)
            //    {
            //        workingMatchList[i].Result = currentPossibilityList[i];
            //    }
            //    List<Team> sortedList = workingTeamList.OrderByDescending(t => t.Points).ToList();
            //    int currentTeamPosition = sortedList.IndexOf(target) + 1;
            //    if (lowestPosition < currentTeamPosition)
            //        lowestPosition = currentTeamPosition;
            //}
            return lowestPosition;
        }

        public void AnticipatePlaces()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //List<MatchResult> possibilityList = new List<MatchResult>();
            ////create start array
            //for (int i = 0; i < workingMatchList.Count; i++)
            //{
            //    possibilityList.Add(MatchResult.HomeTeamWon);
            //}
            //List<List<MatchResult>> ListOfPossibilities = GetAllByIterating(possibilityList);
            int bestPlace = PositiveAnticipation();
            int worstPlace = NegativeAnticipation();
            Console.WriteLine("Best place: " + bestPlace + " worst place: " + worstPlace);
            //Console.WriteLine(workingMatchList.Count.ToString() + " working match list: ");
            //foreach(Match m in workingMatchList)
            //{
            //    Console.WriteLine(m.HomeTeam.Name + " vs " + m.AwayTeam.Name);
            //}
            //sw.Stop();
            Console.WriteLine("Time Elapsed: " + sw.Elapsed);
        }
    }
}
