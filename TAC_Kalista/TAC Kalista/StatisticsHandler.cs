using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Net;
using System.IO;

namespace TAC_Kalista
{
    class StatisticsHandler
    {
        public static int gameTime = (int)Game.Time;
        public static int gameId = 0;
        public static WebClient client = new WebClient { Proxy = null };
        public static void init()
        {
            Game.PrintChat("[Auth] Welcome "+ObjectManager.Player.ChampionName+"! There are "+getGames()+" games played with this script in total! (Counting from 9:34PM GMT+2 11/23/2014");
            if (gameId == 0)
            {
                Game.PrintChat("[Auth] Your game is now being recorded, have fun!");
                insertGame();
            }
        }
        public static void insertGame(){
            gameId = Convert.ToInt32(client.DownloadString("http://team-xte.com/documentation/api/leaguesharp.php?type=1"));
        }
        public static void updateGame()
        {
            client.DownloadString("http://team-xte.com/documentation/api/leaguesharp.php?type=2&id="+gameId+"&time="+gameTime);
        }
        public static int getGames() {
            return Convert.ToInt32(client.DownloadString("http://team-xte.com/documentation/api/leaguesharp.php?type=3"));
        }
    }
}
