using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    //TODO: finish this part!
    public class HedgehogNPC : MonoBehaviour
    {
        public CustomNPC npc;

        public static HedgehogNPC Create(Vector3 localPos)
        {
            CustomNPC customNPC = CustomNPC.Create(localPos);
            HedgehogNPC hedgehogNPC = customNPC.eventObjectGO.AddComponent<HedgehogNPC>();
            hedgehogNPC.npc = customNPC;
            return hedgehogNPC;
        }
    }
}
