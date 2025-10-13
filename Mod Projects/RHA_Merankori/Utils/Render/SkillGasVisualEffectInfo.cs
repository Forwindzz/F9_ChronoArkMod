using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{

    public class SkillGasVisualEffectInfo
    {

        public static string ASSET_SKILL_GAS_EFFECT_PATH = "Assets/ModAssets/Content/Sprites/UI/Fluid/GasSkillUIEffect Variant.prefab";
        private static GameObject skillGasEffectObjTemplate = null;


        public static GameObject GetSkillGasEffectObj()
        {
            if (skillGasEffectObjTemplate == null)
            {
                //load
                skillGasEffectObjTemplate = ResUtils.LoadModPrefab(ASSET_SKILL_GAS_EFFECT_PATH);
                if (skillGasEffectObjTemplate == null)
                {
                    Debug.LogWarning($"Cannot load {ASSET_SKILL_GAS_EFFECT_PATH}");
                    return null;
                }
                skillGasEffectObjTemplate.SetActive(false);
            }
            return skillGasEffectObjTemplate;
        }

        public static SkillGasVisualEffectInfo CloneSKillGasEffectObj(Skill skill)
        {
            if (skill == null)
            {
                Debug.LogWarning("Try to create null skill gas effect obj");
                return null;
            }
            SkillButton skillButton = skill.MyButton;
            if (skillButton == null)
            {
                Debug.LogWarning($"skill {skill?.MySkill?.Name} does not has any ui, cannot create gas effect obj");
                return null;
            }
            GameObject template = GetSkillGasEffectObj();
            if (template == null)
            {
                return null;
            }
            SkillGasVisualEffectInfo skillGasVisualInfo;
            UIGasController controller = skillButton.GetComponentInChildren<UIGasController>();
            if (controller != null)
            {
                skillGasVisualInfo = new SkillGasVisualEffectInfo();
                skillGasVisualInfo.skillGasEffect = controller.gameObject;
                skillGasVisualInfo.controller = controller;
                skillGasVisualInfo.refCounter = skillGasVisualInfo.skillGasEffect.GetComponent<RefCounter>();
            }
            else
            {
                // create !
                skillGasVisualInfo = new SkillGasVisualEffectInfo();
                skillGasVisualInfo.skillGasEffect = UnityEngine.GameObject.Instantiate(template, skillButton.transform);
                GameObject skillGasEffect = skillGasVisualInfo.skillGasEffect;
                skillGasVisualInfo.controller = skillGasEffect.GetComponent<UIGasController>();
                if (skillGasVisualInfo.controller == null)
                {
                    skillGasVisualInfo.controller = skillGasEffect.AddComponent<UIGasController>();
                }
                skillGasVisualInfo.refCounter = skillGasEffect.GetComponent<RefCounter>();
                if (skillGasVisualInfo.refCounter == null)
                {
                    skillGasVisualInfo.refCounter = skillGasEffect.AddComponent<RefCounter>();
                }
            }

            skillGasVisualInfo.skillGasEffect.SetActive(true);
            skillGasVisualInfo.refCounter.counter++;
            skillGasVisualInfo.skill = skill;
            return skillGasVisualInfo;
        }

        public Skill skill;
        public GameObject skillGasEffect;
        public UIGasController controller;
        private RefCounter refCounter; // counter how many info use this effect, avoid to create repeatly

        private SkillGasVisualEffectInfo() { }

        public void SetFactor(float factor)
        {
            controller.SetFactor(factor);
        }

        public static void Destroy(ref SkillGasVisualEffectInfo info)
        {
            if (info == null)
            {
                return;
            }
            info.refCounter.counter--;
            if (info.refCounter.counter <= 0)
            {
                GameObject.Destroy(info.skillGasEffect);
            }
            info = null;
        }
    }
}
