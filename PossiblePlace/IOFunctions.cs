using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PossiblePlace
{
    class IOFunctions
    {
        public static List<Team> LoadTeams()
        {
            List<Team> ListOfTeams = new List<Team>();
            try
            {
                using (StreamReader sr = new StreamReader(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "//tabela.txt", Encoding.Default, true))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] splitedLine = line.Split(null);
                        string teamName = "";
                        int teamPoints = 0;
                        foreach (string text in splitedLine)
                        {
                            if (Regex.IsMatch(text, @"^[\p{L}]+"))
                            {
                                if (teamName.Count() > 0)
                                    teamName = teamName + " " + text.Trim();
                                else
                                    teamName = text.Trim();
                            }
                            else if (Regex.IsMatch(text, @"^[0-9]+"))
                            {
                                teamPoints = int.Parse(text);
                            }
                        }
                        if (teamName != "")
                        {
                            Team currentTeam = new Team(teamName, teamPoints);
                            ListOfTeams.Add(currentTeam);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            if (ListOfTeams.Count > 0)
            {
                return ListOfTeams;
            }
            return null;
        }

        public static List<Match> LoadMatches(List<Team> listOfTeams)
        {
            List<Match> listOfMatches = new List<Match>();
            try
            {
                using (StreamReader sr = new StreamReader(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "//mecze.txt"))
                {
                    string line;
                    string previousLine = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(line, @"[\p{L}][\p{L} ]+$"))
                        {
                            if (previousLine == null)
                                previousLine = line;
                            else
                            {
                                Team homeTeam = listOfTeams.Find((team) => (team.Name == previousLine.Trim()));
                                Team awayTeam = listOfTeams.Find((team) => (team.Name == line.Trim()));
                                Match createdMatch = new Match(homeTeam, awayTeam);
                                homeTeam.MatchesLeft++;
                                awayTeam.MatchesLeft++;
                                listOfMatches.Add(createdMatch);
                                previousLine = null;
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            if (listOfMatches.Count > 0)
                return listOfMatches;
            return null;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static int ChooseTeam(List<Team> listOfTeams)
        {
            int index = 0;
            ConsoleKeyInfo pressedKey;
            Console.WriteLine("What team position would you like to anticipate?");
            do
            {
                ClearCurrentConsoleLine();
                Console.Write(listOfTeams[index].Name);
                pressedKey = Console.ReadKey();
                if (pressedKey.Key == ConsoleKey.UpArrow && index != 0)
                    index--;
                else if (pressedKey.Key == ConsoleKey.DownArrow && index != listOfTeams.Count - 1)
                    index++;
            }
            while (pressedKey.Key != ConsoleKey.Enter);
            Console.SetCursorPosition(0, Console.CursorTop+1);
            return index;
        }
    }
}
