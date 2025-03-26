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

        public const string ID_Character_Merankori = "C_Merankori";

        public static readonly string[] List_CanAddStackBuffsID = new string[]
        {
            ModItemKeys.Buff_B_FreqShift,
            ModItemKeys.Buff_B_Shield,
            ModItemKeys.Buff_B_CalmDown,
            ModItemKeys.Buff_B_DeathResist,
            ModItemKeys.Buff_B_Backflow
        };
    }
}
