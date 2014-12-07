using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

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
            Game.PrintChat("---------------------------");
            Game.PrintChat("[<font color='#FF0000'>v3.8</font>]<font color='#7A6EFF'>Twilight's Auto Carry:</font> <font color='#86E5E1'>Kalista</font>");
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Kalista")
            {
                SkillHandler.init();
                ItemHandler.init();
                MenuHandler.init();
                DrawingHandler.init();
                Game.OnGameUpdate += OnGameUpdateModes;
                AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapCloser;
                Obj_AI_Hero.OnProcessSpellCast += FightHandler.OnProcessSpellCast;
            }
            else if (KalistaInGame() && ObjectManager.Player.ChampionName != "Kalista")
            {
                SkillHandler.initOath();
                MenuHandler.initOath();
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Game.OnGameUpdate += Game_OnGameUpdateOath;
            }
        }
        public static void OnGameUpdateModes(EventArgs args)
        {
            drawings = MenuHandler.Config.Item("enableDrawings").GetValue<bool>();
            debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            packetCast = MenuHandler.Config.Item("Packets").GetValue<bool>();
            if (ObjectManager.Player.HasBuff("Recall")) return;

            if (MenuHandler.Config.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                FightHandler.OnCombo();
            }
            else if (MenuHandler.Config.Item("Farm").GetValue<KeyBind>().Active)
            {
                FightHandler.OnHarass();
            }
            else if (MenuHandler.Config.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.OnLaneClear();
            }
            if (MenuHandler.Config.Item("saveSould").GetValue<bool>())
            {
                FightHandler.saveSould();
            }
            SmiteHandler.Init();

        }
        /**
         * @author Trees
         * */
        internal static void Game_OnGameUpdateOath(EventArgs args)
        {
            if (KalistaInGame() && ObjectManager.Player.ChampionName != "Kalista")
            {
                if (!MenuHandler.Config.Item("Enabled").GetValue<KeyBind>().Active ||
                    ObjectManager.Player.HealthPercentage() < MenuHandler.Config.Item("Health").GetValue<Slider>().Value)
                {
                    return;
                }

                if (SkillHandler.Oathsworn.Instance == null || SkillHandler.Oathsworn.Instance.Name == null || SkillHandler.Oathsworn.Instance.Name == "BaseSpell" ||
                    (SkillHandler.Oathsworn.Instance.Name == "KalistaRAllyDash" && SkillHandler.State != 3))
                {
                    return;
                }

                SkillHandler.SetState();

                if (SkillHandler.State != 0)
                {
                    return;
                }

                Obj_AI_Hero targ = SimpleTs.GetTarget(350, SimpleTs.DamageType.Magical);
                Vector3 pos = targ.IsValidTarget() ? targ.ServerPosition : Game.CursorPos;
                Packet.C2S.Cast.Struct p = new Packet.C2S.Cast.Struct(0, SkillHandler.Oathsworn.Slot, -1, pos.X, pos.Y, pos.X, pos.Y, 0xF2);
                Packet.C2S.Cast.Encoded(p).Send();
            }
        }
        /**
         * @author Trees
         * */
        private static bool KalistaInGame()
        {
            return ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsAlly && hero.ChampionName == "Kalista");
        }
        /**
         * @author Trees
         * */
        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (KalistaInGame() && ObjectManager.Player.ChampionName != "Kalista")
            {
                if (args.PacketData[0] != Packet.MultiPacket.Header ||
                    args.PacketData[5] != (byte)Packet.MultiPacketType.LockCamera ||
                    !MenuHandler.Config.Item("BlockCamera").GetValue<bool>())
                {
                    return;
                }
                args.Process = false;
            }
        }
    }
}
