using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeaguePlusPlusLoader
{
    public class LeaguePlusPlusLoader
    {
        /**
         * Import the dll and it's functions
         * */
        #region Menu
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onGameLoadMenu", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void onGameLoadMenu();
        #endregion
        #region onGame
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onGame", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void onGame();
        #endregion
        #region onGameUpdate
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onGameUpdate", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void onGameUpdate(EventArgs args);
        #endregion
        #region onDraw
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onDraw", CharSet = CharSet.Unicode)]
        public static extern void onDraw(EventArgs args);
        #endregion
        #region Enemy Gap closer
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onEnemyGapCloser", CharSet = CharSet.Unicode)]
        public static extern void onEnemyGapCloser(ActiveGapcloser gapcloser);
        #endregion
        #region Possilbe to interrupt
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onPossibleToInterrupt", CharSet = CharSet.Unicode)]
        public static extern void onPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell);
        #endregion
        #region on Process Spell Cast
        [DllImport("LeaguePlusPlus.dll", EntryPoint = "onProcessSpellCast", CharSet = CharSet.Unicode)]
        public static extern void onProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args);
        #endregion

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
            onGame();
            // C++ DLL Menu Handler
            onGameLoadMenu();
            // C++ DLL Game Logic
            Game.OnGameUpdate += onGameUpdate;
            // C++ DLL Gap Closer Logic
            AntiGapcloser.OnEnemyGapcloser += onEnemyGapCloser;
            // C++ DLL Interrupter Logic
            Interrupter.OnPossibleToInterrupt += onPossibleToInterrupt;
            // C++ DLL Spell cast logic
            Obj_AI_Hero.OnProcessSpellCast += onProcessSpellCast;
            // C++ DLL Drawings
            Drawing.OnDraw += onDraw;
        }
    }
}