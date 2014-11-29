using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Mordekaiser
{
    class Program
    {
        internal static bool packetCast = true;
        internal static Orbwalking.Orbwalker orb;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onLoad;
            Game.PrintChat("Mordekaiser loaded!");
        }
        private static void onLoad(EventArgs args)
        {
            AntiGapcloser.OnEnemyGapcloser += AutoCarryHandler.AntiGapCloser;
            Obj_AI_Hero.OnProcessSpellCast += AutoCarryHandler.onProcessSpellCast;
            Game.OnGameUpdate += onUpdate;
        }
        private static void onUpdate(EventArgs args)
        {
            switch (orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AutoCarryHandler.onCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    AutoCarryHandler.Mixed();
                    break;
            }
        }
    }
}
