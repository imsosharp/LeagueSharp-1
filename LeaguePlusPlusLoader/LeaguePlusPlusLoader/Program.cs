using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using LeaguePlusPlus;

namespace LeaguePlusPlusLoader
{
    public class LeaguePlusPlusLoader
    {
        /**
         * Load the handlers
         * */
        public static void Main()
        {
            if (!File.Exists("LeaguePlusPlus.dll"))
            {
                Game.PrintChat("Failed to load DLL");
                return;
            }
            else
            {
                Game.PrintChat("Loading L++!");
                // Load C++ DLL Logic
                CustomEvents.Game.OnGameLoad += onGameLoad;
            }
        }
        internal static void onGameLoad(EventArgs args)
        {
            // C++ DLL Skill Handler
            LPP.onGame();
            // C++ DLL Menu Handler
            LPP.onGameLoadMenu();
            // C++ DLL Game Logic
            Game.OnGameUpdate += LPP.onGameUpdate;
            // C++ DLL Gap Closer Logic
            AntiGapcloser.OnEnemyGapcloser += LPP.onEnemyGapCloser;
            // C++ DLL Interrupter Logic
            Interrupter.OnPossibleToInterrupt += LPP.onPossibleToInterrupt;
            // C++ DLL Spell cast logic
            Obj_AI_Hero.OnProcessSpellCast += LPP.onProcessSpellCast;
            // C++ DLL Drawings
            Drawing.OnDraw += LPP.onDraw;
        }
    }
}