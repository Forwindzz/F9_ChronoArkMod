using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHA_Merankori
{
    public static class IDs
    {
        public const string ID_Harmony_Name = "com.RHA_Merankori.patch";

        public static readonly string[] List_CanAddStackBuffsID = new string[]
        {
            ModItemKeys.Buff_B_Backup,
            ModItemKeys.Buff_B_CalmDown,
            ModItemKeys.Buff_B_Charge,
            ModItemKeys.Buff_B_DeathResist,
            ModItemKeys.Buff_B_FreqShift,
            ModItemKeys.Buff_B_Reflow,
            ModItemKeys.Buff_B_Shield,
            ModItemKeys.Buff_B_WarmFire
        };

        public static readonly string[] List_MerankoriSkills = new string[]
        {
            ModItemKeys.Skill_S_Attack_All,
            ModItemKeys.Skill_S_Backup,
            ModItemKeys.Skill_S_Care,
            ModItemKeys.Skill_S_Charge,
            ModItemKeys.Skill_S_ConvertCard,
            ModItemKeys.Skill_S_ElementHeal,
            ModItemKeys.Skill_S_Manifold,
            ModItemKeys.Skill_S_OverClocking,
            ModItemKeys.Skill_S_Reconstruct,
            ModItemKeys.Skill_S_Rectification,
            ModItemKeys.Skill_S_Retreat,
            ModItemKeys.Skill_S_Shield,
            ModItemKeys.Skill_S_WarmFire
        };
    }
}
