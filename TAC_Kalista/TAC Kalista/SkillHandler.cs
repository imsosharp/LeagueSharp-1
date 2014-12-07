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
        internal static Spell Q, W, E, R, Oathsworn;
        internal static Spell[] spellList = { Q, W, E, R };
        internal static int State = 0;
        internal static void init()
        {
            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W, 5500);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
        }
        /**
         * @author Trees
         * */
        internal static void initOath()
        {
            Oathsworn = new Spell((SpellSlot)0x3C, 300);
            Oathsworn.SetSkillshot(.862f, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        /**
         * @author Trees
         * */
        internal static void SetState()
        {
            switch (Oathsworn.Instance.Name)
            {
                case "KalistaRAllyDashCantCast1":
                    State = 1;
                    break;
                case "KalistaRAllyDashCantCast2":
                    State = 2;
                    break;
                case "KalistaRAllyDashCantCast3":
                    State = 3;
                    break;
                case "KalistaRAllyDash":
                    State = 0;
                    break;
            }
        }
    }
}
