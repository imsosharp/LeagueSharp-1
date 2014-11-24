using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using xSLx_Orbwalker;

namespace TAC_Kalista
{
    class MenuHandler
    {
        public static Menu Config;
        public static void init()
        {
            Config = new Menu("Twilight Kalista Rework", "Kalista", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            xSLxOrbwalker.AddToMenu(Config.SubMenu("Orbwalking"));

            var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
            SimpleTs.AddToMenu(targetselectormenu);
            Config.AddSubMenu(targetselectormenu);
            
            Config.AddSubMenu(new Menu("AutoCarry options", "ac"));
            Config.SubMenu("ac").AddItem(new MenuItem("Focus_Target", "Force Selected Target").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("UseQAC", "Use Q").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("UseQACM", "Q Prediction").SetValue(new StringList(new[] { "Low", "Medium", "High" }, 2)));
            Config.SubMenu("ac").AddItem(new MenuItem("UseEAC", "Use E").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("E4K", "E only for kill").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("sex1", "-----------"));
            Config.SubMenu("ac").AddItem(new MenuItem("minE", "Min stacks to E").SetValue(new Slider(1, 1, 20)));
            Config.SubMenu("ac").AddItem(new MenuItem("minEE", "Min stacks enabled?").SetValue(false));
            
            Config.AddSubMenu(new Menu("Harass options", "harass")); 
            Config.SubMenu("harass").AddItem(new MenuItem("stackE", "E stacks to cast").SetValue(new Slider(1, 1, 10)));
            Config.SubMenu("harass").AddItem(new MenuItem("manaPercent", "Mana %").SetValue(new Slider(40, 1, 100)));
            /*
            Config.AddSubMenu(new Menu("Wave Clear options", "wc"));
            Config.SubMenu("wc").AddItem(new MenuItem("useEwc", "Use E to clear").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("enableClear", "WaveClear enabled?").SetValue(false));
            
            Config.AddSubMenu(new Menu("Smite options", "smite"));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Baron", "Baron Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Dragon", "Dragon Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Gromp", "Gromp Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Murkwolf", "Murkwolf Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Krug", "Krug Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Razorbeak", "Razorbeak Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("Sru_Crab", "Crab Enabled").SetValue(false));

            Config.SubMenu("smite").AddItem(new MenuItem("smite", "Auto-Smite enabled").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press,false)));
            
            Config.AddSubMenu(new Menu("Item options", "Items"));
            Config.SubMenu("Items").AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
            Config.SubMenu("Items").AddItem(new MenuItem("GHOSTBLADE", "Ghostblade").SetValue(true));
            Config.SubMenu("Items").AddItem(new MenuItem("SWORD", "Sword of the Divine").SetValue(true));
            Config.SubMenu("Items").AddItem(new MenuItem("IGNITE", "Ignite").SetValue(true));
            
            Config.SubMenu("Items").AddSubMenu(new Menu("QSS", "QuickSilverSash"));
            Config.SubMenu("Items").SubMenu("QuickSilverSash").AddItem(new MenuItem("AnyStun", "Any Stun").SetValue(true));
            Config.SubMenu("Items").SubMenu("QuickSilverSash").AddItem(new MenuItem("AnySnare", "Any Snare").SetValue(true));
            Config.SubMenu("Items").SubMenu("QuickSilverSash").AddItem(new MenuItem("AnyTaunt", "Any Taunt").SetValue(true));
            foreach (var t in ItemHandler.BuffList)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                {
                    if (t.ChampionName == enemy.ChampionName)
                        Config.SubMenu("Items").SubMenu("QuickSilverSash").AddItem(new MenuItem(t.BuffName, t.DisplayName).SetValue(t.DefaultValue));
                }
            }
            Config.SubMenu("Items").SubMenu("QuickSilverSash").AddItem(new MenuItem("UseItemsMode", "Items usage").SetValue(new StringList(new[] { "No", "Harass mode", "Combo mode", "Flee mode", "All" }, 2)));
            

            var potions = ItemHandler.potions.OrderBy(x => x.Priority).ToList();
            Config.AddSubMenu(new Menu("Potion Manager", "PotionManager"));
            Config.SubMenu("PotionManager").AddSubMenu(new Menu("Health", "Health"));
            Config.SubMenu("PotionManager").SubMenu("Health").AddItem(new MenuItem("HealthPotion", "Use Health Potion").SetValue(true));
            Config.SubMenu("PotionManager").SubMenu("Health").AddItem(new MenuItem("HealthPercent", "HP Trigger Percent").SetValue(new Slider(30,15,100)));
            Config.SubMenu("PotionManager").AddSubMenu(new Menu("Mana", "Mana"));
            Config.SubMenu("PotionManager").SubMenu("Mana").AddItem(new MenuItem("ManaPotion", "Use Mana Potion").SetValue(true));
            Config.SubMenu("PotionManager").SubMenu("Mana").AddItem(new MenuItem("ManaPercent", "MP Trigger Percent").SetValue(new Slider(30,15,100)));
            */
            Config.AddSubMenu(new Menu("Wall Hop options", "wh"));
            Config.SubMenu("wh").AddItem(new MenuItem("JumpTo", "Jump key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(false, Color.FromArgb(100, Color.Coral))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(false, Color.FromArgb(100, Color.Blue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawJumpPos", "Draw wallhop spots")).SetValue(new Circle(false, Color.HotPink));
            Config.SubMenu("Drawings").AddItem(new MenuItem("test1", "Spot draw range by skill:"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawJumpPosRange", "----------->").SetValue(new StringList(new[] { "Q", "E", "R" }, 2)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "Draw damage on HP bar")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawStacks", "Draw total stacks")).SetValue(true);

            Config.SubMenu("Drawings").AddItem(new MenuItem("test2", "----------------")); 
            Config.SubMenu("Drawings").AddItem(new MenuItem("enableDrawings", "Enable all drawings").SetValue(true));          

            Config.AddItem(new MenuItem("Packets", "Packet Casting").SetValue(true));

            Config.AddItem(new MenuItem("debug", "Debug").SetValue(false));
            Config.AddItem(new MenuItem("showPos", "Server Position").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
            
            Config.AddToMainMenu();

        }
    }
}
