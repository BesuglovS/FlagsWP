using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagsWP.Core
{
    public class GameMode
    {
        public static GameMode CountryNames = new GameMode() { ID = 1 };
        public static GameMode CountryCapitals = new GameMode() { ID = 2 };

        public int ID;
    }
}
