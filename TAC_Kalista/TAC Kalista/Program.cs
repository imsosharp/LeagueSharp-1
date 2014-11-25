﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kalista
{
    class Kalista
    {
        public static bool packetCast;
        public static bool debug;
        public static bool drawings;
        public static bool canexport = true;
        static void Main(string[] args)
        {
            Game.PrintChat("[2.9.2] Loading Twilights Kalista! If you dont see more text please press F5!");
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Kalista")
            {
                Game.PrintChat("Kalista not found! Assembly failed to load!");
                return;
            }
            Game.PrintChat("[Twilight] Item handlers have been temporary removed, because they caused many bugsplats.");
            Game.PrintChat("Fixed E damage calculation, @Hellsing you are terrible at math.");
            SkillHandler.init();
//            ItemHandler.init();
            MenuHandler.init();
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
            if (ObjectManager.Player.HasBuff("Recall")) return;

            if (MenuHandler.Config.Item("DrawJumpPos").GetValue<Circle>().Active) DrawingHandler.fillPositions();
            if (MenuHandler.Config.Item("JumpTo").GetValue<KeyBind>().Active) SkillHandler.JumpTo();

            
//            int useItemModes = MenuHandler.Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;
            if (MenuHandler.Config.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                //                    if (useItemModes == 3 || useItemModes == 5) ItemHandler.useItem();
                FightHandler.OnCombo();
            }
            else if (MenuHandler.Config.Item("Farm").GetValue<KeyBind>().Active)
            {
                //                    if (useItemModes == 1 || useItemModes == 5) ItemHandler.useItem();
                FightHandler.OnHarass();
            }
            else if (MenuHandler.Config.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.OnLaneClear();
            }
            //FightHandler.OnFlee();
            SmiteHandler.Init();

        }
    }
}
