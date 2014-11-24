using System;
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
            Game.PrintChat("[2.8] Loading Twilights Kalista! If you dont see more text please press F5!");
            Game.PrintChat("[Twilight] Smite, Item handlers have been temporary removed, because they caused many bugsplats.");
            Game.PrintChat("Added auto-slow target, configure it as you want!:)");
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
//            ItemHandler.init();
            MenuHandler.init();
//            SmiteHandler.Init();
            DrawingHandler.init();
            Game.PrintChat("Twilights AutoCarry: Kalista has loaded!");
            Game.OnGameUpdate += OnGameUpdateModes;
            Obj_AI_Hero.OnProcessSpellCast += FightHandler.OnProcessSpellCast;
        }
        public static void OnGameUpdateModes(EventArgs args)
        {
            /*
            if (MenuHandler.Config.Item("exportjump1").GetValue<KeyBind>().Active && canexport)
            {
                using (var file = new System.IO.StreamWriter(@"C:\work\KalistaSpot.txt",true))
                {
                    file.Write("jumpPos.Add(new Vector3(" + ObjectManager.Player.ServerPosition.X + "f, " + ObjectManager.Player.ServerPosition.Y + "f, " + ObjectManager.Player.ServerPosition.Z + "f),");
                    file.Close();
                }
                Game.PrintChat("Debug: Position1 exported!");
                canexport = false;
                MenuHandler.Config.Item("exportjump1").SetValue<KeyBind>(new KeyBind(117, KeyBindType.Press,false));
            }
            else { canexport = true; }
            if (MenuHandler.Config.Item("exportjump2").GetValue<KeyBind>().Active && canexport)
            {
                using (var file = new System.IO.StreamWriter(@"C:\work\KalistaSpot.txt",true))
                {
                    file.WriteLine("new Vector3(" + ObjectManager.Player.ServerPosition.X + "f, " + ObjectManager.Player.ServerPosition.Y + "f, " + ObjectManager.Player.ServerPosition.Z + "f));");
                    file.Close();
                }
                Game.PrintChat("Debug: Position2 exported.");
                canexport = false;
            }
            else { canexport = true; }*/
            drawings = MenuHandler.Config.Item("enableDrawings").GetValue<bool>();
            debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            packetCast = MenuHandler.Config.Item("Packets").GetValue<bool>();
            if (ObjectManager.Player.HasBuff("Recall")) return;

            if (MenuHandler.Config.Item("DrawJumpPos").GetValue<Circle>().Active) DrawingHandler.fillPositions();
            if (MenuHandler.Config.Item("JumpTo").GetValue<KeyBind>().Active) SkillHandler.JumpTo();

            
//            int useItemModes = MenuHandler.Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;
            if (MenuHandler.Config.Item("comboActive").GetValue<KeyBind>().Active)
            {
                //                    if (useItemModes == 3 || useItemModes == 5) ItemHandler.useItem();
                FightHandler.OnCombo();
            }
            else if (MenuHandler.Config.Item("harassActive").GetValue<KeyBind>().Active)
            {
                //                    if (useItemModes == 1 || useItemModes == 5) ItemHandler.useItem();
                FightHandler.OnHarass();
            }
            else if (MenuHandler.Config.Item("waveClear").GetValue<KeyBind>().Active)
            {
                FightHandler.OnLaneClear();
            }
            else if (MenuHandler.Config.Item("flee").GetValue<KeyBind>().Active)
            {
                //                    if (useItemModes == 4 || useItemModes == 5) ItemHandler.useItem();
                FightHandler.OnFlee();
            }
//            ItemHandler.PotionHandler();

        }
    }
}
