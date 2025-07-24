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
            BlowUpAttributes attr;
            attr.range = 1;
            //Debug.Log("try to compute blow up attr");

            if(PlayData.TSavedata==null)
            {
                return attr;
            }
            
            foreach (var info in PlayData.TSavedata.Party)
            {
                foreach (var equip in info.Equip)
                {
                    //Debug.Log($"Check Equip: {equip.GetName} {equip is IModifyBlowUpAttr}");
                    if (equip is IModifyBlowUpAttr modify)
                    {
                        if (modify != null)
                        {
                            modify.ModifyBlowUpAttr(ref attr);
                            //Debug.Log($"Modify blow up attr {modify} -> {attr}");
                        }
                    }
                }
            }

            return attr;
            /*
            if (BattleSystem.instance==null)
            {
                return attr;
            }
            BattleTeam allyTeam = BattleSystem.instance.AllyTeam;
            foreach(IModifyBlowUpAttr modify in BattleSystem.instance.IReturn<IModifyBlowUpAttr>())
            {
                if(modify!=null)
                {
                    modify.ModifyBlowUpAttr(ref attr);
                    Debug.Log($"Modify blow up attr {modify} -> {attr}");
                }
            }
            foreach(var c in BattleSystem.instance.AllyTeam.Chars)
            {
                Character info = c.Info;
                foreach(var equip in info.Equip)
                {
                    Debug.Log($"Check Equip: {equip.GetName} {equip is IModifyBlowUpAttr}");
                    if(equip is IModifyBlowUpAttr modify)
                    {
                        if (modify != null)
                        {
                            modify.ModifyBlowUpAttr(ref attr);
                            Debug.Log($"Modify blow up attr {modify} -> {attr}");
                        }
                    }
                }
            }

            Debug.Log($"final {attr.range}");
            return attr;
            */
        }
    }
}
