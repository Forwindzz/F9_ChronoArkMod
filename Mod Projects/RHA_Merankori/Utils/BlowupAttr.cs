using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    


    public static class BlowUpAttr
    {
        public struct BlowUpAttributes
        {
            public int range;
        }

        public interface IModifyBlowUpAttr
        {
            void ModifyBlowUpAttr(ref BlowUpAttributes attr);
        }


        public static BlowUpAttributes GetBlowUpAttr()
        {
            BlowUpAttributes attr = new BlowUpAttributes();
            attr.range = 1;
            Debug.Log("try to compute blow up attr");

            if(PlayData.TSavedata==null)
            {
                Debug.Log("Null save data!");
                return attr;
            }

            if (PlayData.TSavedata.Party == null)
            {
                Debug.Log("Null party save data!");
                return attr;
            }

            Utils.AllyTeamEquipRunIfType<IModifyBlowUpAttr>(
                modifier => modifier.ModifyBlowUpAttr(ref attr)
                );

            return attr;
        }
    }
}
