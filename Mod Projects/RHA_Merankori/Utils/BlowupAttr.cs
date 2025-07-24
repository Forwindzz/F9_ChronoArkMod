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

            foreach (var info in PlayData.TSavedata.Party)
            {
                if(info==null || info?.Equip==null)
                {
                    continue;
                }
                foreach (var equip in info.Equip)
                {
                    if(equip==null)
                    {
                        continue;
                    }
                    Item_Equip equip_item = equip as Item_Equip;
                    if(equip_item==null)
                    {
                        continue;
                    }
                    EquipBase equipBase = equip_item.ItemScript;
                    if(equipBase==null)
                    {
                        Debug.Log("Use snap shot");
                        equipBase = equip_item.ItemScript_Snapshot;
                        if(equipBase==null)
                        {
                            continue;
                        }
                    }

                    Debug.Log($"Check Equip: {equip_item?.GetName} {equipBase?.GetType()?.Name} {equipBase is IModifyBlowUpAttr}");
                    if (equipBase is IModifyBlowUpAttr modifier)
                    {
                        Debug.Log("is blow up attr!");
                        if (modifier != null)
                        {
                            Debug.Log("modify!");
                            modifier.ModifyBlowUpAttr(ref attr);
                            Debug.Log($"Modify blow up attr {modifier} -> {attr.range}");
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
