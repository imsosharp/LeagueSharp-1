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

        internal static bool packetCast = true;
        internal static bool debug = false;
        internal static bool isCastingQ = false;
        internal static bool hasResetBuffCount = false;

        internal static int w_mode = 3;
        internal static int buffCount = 0;
        internal static int lastTick = 0;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;   
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.BaseSkinName != "Jax") return;
            SkillHandler.load();
            MenuHandler.load();
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += EventHandler.Game_OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += EventHandler.AntiGapCloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += DrawingHandler.load;
            Drawing.OnEndScene += DrawingHandler.OnEndScene;
        }
        static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var dBuffBro = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "JaxRelentlessAssaultAS").Count;
            if (dBuffBro > 0)
            {
                buffCount++;
                if (buffCount != dBuffBro && dBuffBro < 6 && dBuffBro > 1)
                    buffCount = dBuffBro;
                lastTick = Environment.TickCount;
                if(debug)
                    Game.PrintChat("(" + buffCount + ") Buff: JaxRelentlessAssaultAS Count: " + dBuffBro);
                hasResetBuffCount = false;
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            switch (orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    EventHandler.onCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
            }
            w_mode = MenuHandler.Config.Item("acW_mode").GetValue<Slider>().Value;
            packetCast = MenuHandler.Config.Item("packetCast").GetValue<bool>();
            updateCount();
        }
        internal static void updateCount()
        {
            /* Check if I have my passive and it didnt expire
             * Only expire the passive when I don't auto attack in 2.5 seconds
             */
            if (hasResetBuffCount == false && Environment.TickCount - lastTick >= 2500)
            {
                if (debug)
                    Game.PrintChat("Resetting buff counter to 0");
                lastTick = 0;
                buffCount = 0;
                hasResetBuffCount = true;
            }
        }
    }
}
