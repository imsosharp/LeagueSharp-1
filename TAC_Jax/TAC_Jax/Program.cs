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
        internal static bool packetCast = true;
        internal static bool debug = false;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.PrintChat("[" + Utils.FormatTime(Game.ClockTime) + "] <font color='#00ff00'>System (Kappa):</font> <font color='#FF0000'>[v1.2]</font> <font color='#7A6EFF'>Twilight's Auto Carry: </font> <font color='#86E5E1'>Jax</font>");
            Game.PrintChat("[" + Utils.FormatTime(Game.ClockTime) + "] <font color='#00ff00'>System (Kappa):</font> Thank you for using my assembly!");
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.BaseSkinName != "Jax") return;
            SkillHandler.load();
            MenuHandler.load();
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += EventHandler.Game_OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += EventHandler.AntiGapCloser;
            Interrupter.OnPossibleToInterrupt += EventHandler.onInterrupt;
            Orbwalking.BeforeAttack += EventHandler.Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += EventHandler.Orbwalking_AfterAttack;
            Drawing.OnDraw += DrawingHandler.load;
            Drawing.OnEndScene += DrawingHandler.OnEndScene;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            packetCast = MenuHandler.Config.Item("packetCast").GetValue<bool>();
            debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            switch (GameHandler.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    EventHandler.onCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    EventHandler.onHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    EventHandler.onLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (MenuHandler.Config.Item("Flee").GetValue<bool>())
                    {
                        Orbwalking.Orbwalk(null, Game.CursorPos);
                        EventHandler.WardJump();
                    }
                    break;
            }
            if (MenuHandler.Config.Item("smartR").GetValue<bool>())
                EventHandler.smartR();

            if (MenuHandler.Config.Item("ks_enabled").GetValue<bool>())
                EventHandler.killSteal();

            if (MenuHandler.Config.Item("Ward").GetValue<KeyBind>().Active) 
                EventHandler.WardJump();
            GameHandler.updateCount();
        }
    }
}
