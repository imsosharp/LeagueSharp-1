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
        internal static bool isCastingQ = false;
        internal static bool hasResetBuffCount = false;
        internal static bool isCastingE = false;
        internal static bool canCastE = false;

        internal static int buffCount = 0;
        internal static int buffCountBeforeQ = 0;
        internal static int lastTick = 0;
        internal static int lastTickE = 0;

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
            //Obj_AI_Hero.OnProcessSpellCast += EventHandler.Game_OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += EventHandler.AntiGapCloser;
            LXOrbwalker.BeforeAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += DrawingHandler.load;
            Drawing.OnEndScene += DrawingHandler.OnEndScene;
        }
        static void Orbwalking_BeforeAttack(LXOrbwalker.BeforeAttackEventArgs args)
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
            packetCast = MenuHandler.Config.Item("packetCast").GetValue<bool>();
            debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    EventHandler.onCombo();
                    break;
                case LXOrbwalker.Mode.Harass:
                    EventHandler.onHarass();
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    EventHandler.onLaneClear();
                    break;
            }
            if (MenuHandler.Config.Item("Ward").GetValue<KeyBind>().Active) Jumper.wardJump(Game.CursorPos.To2D());
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
                buffCountBeforeQ = 0;
                hasResetBuffCount = true;
            }
            /* Skill E time shit */
            // TODO:
            // double check this shit, i think its not working properaly
            if(isCastingE && !canCastE && lastTickE > 0 && Environment.TickCount - lastTickE >= 1500)
            {
                lastTickE = 0;
                canCastE = true;
                if (debug)
                    Game.PrintChat("Jax can cast E, resetting counter to 0");
            }
            if(lastTickE > 0 && Environment.TickCount - lastTickE >= 2000)
            {
                lastTick = 0;
                canCastE = false;
                isCastingE = false;
                if (debug)
                    Game.PrintChat("Resetting jax counter strike counter to 0");
            }
        }
    }
}
