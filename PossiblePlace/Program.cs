using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PossiblePlace
{
    class Program
    {
        static void Main(string[] args)
        {
            PositionAncitipator mainClass = new PositionAncitipator();
            mainClass.AnticipatePlaces();
            Console.ReadLine();
        }
    }
}
