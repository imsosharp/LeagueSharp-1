using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

namespace TAC_Kalista
{
    class Kalista
    {
        public static bool packetCast;
        public static bool debug;
        public static bool drawings;
        static void Main(string[] args)
        {
            Game.PrintChat("Loading Twilights Kalista!");
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Kalista")
            {
                Game.PrintChat("Kalista not found! Assembly failed to load!");
                return;
            }
            SkillHandler.init();
            ItemHandler.init();
            MenuHandler.init();
            SmiteHandler.Init();
            DrawingHandler.init();
            Game.PrintChat("Twilights AutoCarry: Kalista has loaded!");
            Game.OnGameUpdate += OnGameUpdateModes;
            Obj_AI_Hero.OnProcessSpellCast += FightHandler.OnProcessSpellCast;
        }
        public static void OnGameUpdateModes(EventArgs args)
        {
            drawings = MenuHandler.Config.Item("enableDrawings").GetValue<bool>();
            debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            packetCast = MenuHandler.Config.Item("Packets").GetValue<bool>();
            
            if (MenuHandler.Config.Item("whKey").GetValue<KeyBind>().Active) SkillHandler.JumpTo();

            if (ObjectManager.Player.HasBuff("Recall")) return;
            
            int useItemModes = MenuHandler.Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;

            switch (xSLxOrbwalker.CurrentMode)
            {
                case xSLxOrbwalker.Mode.Combo:
                    if (useItemModes == 3 || useItemModes == 5) ItemHandler.useItem();
                    FightHandler.OnCombo();
                    break;
                case xSLxOrbwalker.Mode.Flee:
                    if (useItemModes == 4 || useItemModes == 5) ItemHandler.useItem();
                    FightHandler.OnFlee();
                    break;
                case xSLxOrbwalker.Mode.Harass:
                    if (useItemModes == 1 || useItemModes == 5) ItemHandler.useItem();
                    FightHandler.OnHarass();
                    break;
                case xSLxOrbwalker.Mode.LaneClear:
                    FightHandler.OnLaneClear();
                    break;
            }
            FightHandler.OnPassive();
            ItemHandler.PotionHandler();
            if (MenuHandler.Config.Item("showPos").GetValue<KeyBind>().Active)
                Game.PrintChat("Position on server: " + ObjectManager.Player.ServerPosition);
        }
    }
}
