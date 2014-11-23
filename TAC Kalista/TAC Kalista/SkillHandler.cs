using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
namespace TAC_Kalista
{
    class SkillHandler
    {
        public static Spell Q, W, E, R;
        public static Spell[] spellList = { Q, W, E, R };
        public static void init()
        {
            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W, 5500);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
        }
        public static void JumpTo()
        {
            if (!Q.IsReady())
            {
                Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.80f, Color.Red,
                "Q is not ready! You can not Jump!");
                return;
            }
            Drawing.DrawText(Drawing.Width * 0.39f, Drawing.Height * 0.80f, Color.White,
            "Jumping Mode is Active! Go to the nearest jump point!");
            foreach (var xTo in from pos in DrawingHandler.jumpPos
                                where ObjectManager.Player.Distance(pos.Key) <= 35f ||
                                ObjectManager.Player.Distance(pos.Value) <= 35f
                                let xTo = pos.Value
                                select ObjectManager.Player.Distance(pos.Key) < ObjectManager.Player.Distance(pos.Value)
                                ? pos.Value
                                : pos.Key)
            {
                Q.Cast(new Vector2(xTo.X, xTo.Y), true);
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(xTo.X, xTo.Y)).Send();
            }
        }
    }
}
