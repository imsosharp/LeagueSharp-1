using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Jax
{
    class EventHandler
    {
        /**
         * 
         * 
            if (SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(true);
                if (ObjectManager.Player.IsDashing())
                {
                    SkillHandler.W.Cast(true);
                }
            }*/
        internal static void onCombo()
        {
            /* Some logic needed here.
             * Don't use Q if we can just run to target.
             * Use Q when we are out of our E range
             * Check how much enemies around before going in
             * Check when I use Q and if it lands use W + 3rd stack
             * ^ may be in combo
             * Make Q use W in mid air
             * Use R on critical damage
             * Use W as AA reset
             */
        }
        internal static float comboDamage(Obj_AI_Hero target)
        {
            /**
             * Count witch stack it is now.
             * Check if next AA is available
             * Check item damage
             * Check Q land + W + 3'rd R + E
             * Check ignite damage
             * */
            return 0f;
        }
    }
}
