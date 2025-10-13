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
                //Debug.LogWarning($"skill {skill?.MySkill?.Name} does not has any ui, cannot create gas effect obj");
                return null;
            }
            GameObject template = GetSkillGasEffectObj();
            if (template == null)
            {
                return null;
            }
            SkillGasVisualEffectInfo skillGasVisualInfo = new SkillGasVisualEffectInfo();
            skillGasVisualInfo.controller = skillButton.GetComponentInChildren<UIGasController>();
            skillGasVisualInfo.animator = skillButton.GetComponentInChildren<UIGasAnimator>();
            if (skillGasVisualInfo.controller != null &&
                skillGasVisualInfo.animator != null && !skillGasVisualInfo.animator.IsDestorying)
            {
                skillGasVisualInfo.skillGasEffect = skillGasVisualInfo.controller.gameObject;
                skillGasVisualInfo.refCounter = skillGasVisualInfo.skillGasEffect.GetComponent<RefCounter>();
                skillGasVisualInfo.animator = skillGasVisualInfo.skillGasEffect.GetComponent<UIGasAnimator>();
            }
            else
            {
                // create !
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

                skillGasVisualInfo.animator = skillGasVisualInfo.skillGasEffect.GetComponent<UIGasAnimator>();
                if (skillGasVisualInfo.animator == null)
                {
                    skillGasVisualInfo.animator = skillGasVisualInfo.skillGasEffect.AddComponent<UIGasAnimator>();
                }
            }

            skillGasVisualInfo.skillGasEffect.SetActive(true);
            skillGasVisualInfo.refCounter.counter++;
            return skillGasVisualInfo;
        }

        public GameObject skillGasEffect;
        public UIGasController controller;
        public UIGasAnimator animator;
        private RefCounter refCounter; // counter how many info use this effect, avoid to create repeatly

        private SkillGasVisualEffectInfo() { }

        public void SetFactor(float factor)
        {
            animator.SetFactorSmooth(factor);
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
                info.animator.DestroySmooth();
            }

            info = null;
        }
    }
}
