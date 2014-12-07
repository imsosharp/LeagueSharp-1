using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Jax
{
    class Jax
    {
        internal static Orbwalking.Orbwalker orb;
        internal static bool packetCast;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;   
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            SkillHandler.load();
            MenuHandler.load();
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            switch(orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    EventHandler.onCombo();
                    break;
            }
        }
    }
}
