using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Jax
{
    class SkillHandler
    {
        internal static Spell Q, W, E, R;
        internal static Spell Flash = new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"),500f);
        
        internal static void load()
        {
            Q = new Spell(SpellSlot.Q, 700f);
            W = new Spell(SpellSlot.Q, 125f);
            E = new Spell(SpellSlot.Q, 187.5f);
            R = new Spell(SpellSlot.Q, 125f);
        }
    }
}
