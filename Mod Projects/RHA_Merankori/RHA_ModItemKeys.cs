using ChronoArkMod;
namespace RHA_Merankori
{
    public static class ModItemKeys
    {
		/// <summary>
		/// 逆流的燐焰晶
		/// <color=#FF6767>湮裂的燐焰晶</color>会根据队友已损失的体力值增加伤害，每2点增加1%伤害。
		/// <color=#5061A4>（预期增加&a）</color>
		/// </summary>
        public static string Buff_B_Backflow = "B_Backflow";
		/// <summary>
		/// 后备的燐焰晶
		/// 回合开始时，生成1张<color=#FF6767>湮裂的燐焰晶</color>并放入手牌中。
		/// </summary>
        public static string Buff_B_Backup = "B_Backup";
		/// <summary>
		/// 冷静
		/// <color=#BAC8FF>照料可能因自己受伤的队友...</color>
		/// 回合开始时梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 当有人攻击时，进入<color=#FFC5BA><b>慌张</b></color>状态。
		/// </summary>
        public static string Buff_B_Calm = "B_Calm";
		/// <summary>
		/// 镇定
		/// 阻止进入慌张状态，每次阻止会减少1层。
		/// </summary>
        public static string Buff_B_CalmDown = "B_CalmDown";
		/// <summary>
		/// 蓄能
		/// 释放<color=#FF6767>湮裂的燐焰晶</color>时，每层增加其10%的伤害。
		/// 释放后移除一半的层数。
		/// </summary>
        public static string Buff_B_Charge = "B_Charge";
		/// <summary>
		/// 无法战斗抵抗
		/// </summary>
        public static string Buff_B_DeathResist = "B_DeathResist";
		/// <summary>
		/// 导流回路
		/// 释放<color=#FF6767>湮裂的燐焰晶</color>时，获得1点法力值。
		/// </summary>
        public static string Buff_B_DLoop = "B_DLoop";
		/// <summary>
		/// 频移微调
		/// 受到<color=#FF6767>湮裂的燐焰晶</color>的攻击时，将受到的伤害转化为等量的治疗，然后移除1层。
		/// </summary>
        public static string Buff_B_FreqShift = "B_FreqShift";
		/// <summary>
		/// 焰晶生成
		/// 每当队友脱离濒死状态时，将<color=#FF6767>湮裂的燐焰晶</color>放入手中。
		/// </summary>
        public static string Buff_B_GenCrystal = "B_GenCrystal";
		/// <summary>
		/// 频移微调-露西
		/// 引导燐焰晶
		/// </summary>
        public static string Buff_B_H_FreqShift_Lucy = "B_H_FreqShift_Lucy";
		/// <summary>
		/// 烈燐焰晶
		/// 燐焰晶的伤害不会致命
		/// </summary>
        public static string Buff_B_NotDeadlyAtk = "B_NotDeadlyAtk";
		/// <summary>
		/// 越频晶石
		/// 下一次<color=#FF6767>湮裂的燐焰晶</color>会附带“震荡冲击”干扰减益
		/// </summary>
        public static string Buff_B_OverClocking = "B_OverClocking";
		/// <summary>
		/// 慌张
		/// <color=#FFC5BA>燐焰晶逐渐炽烈...</color>
		/// 回合开始时，梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 当有人攻击时，进入<color=#FFC5BA><b>慌张</b></color>状态。
		/// </summary>
        public static string Buff_B_Panic = "B_Panic";
		/// <summary>
		/// 燐焰晶回流
		/// 回合开始时，赋予1层燐色存护。
		/// 溢出的燐色存护会转化为折射。
		/// </summary>
        public static string Buff_B_Reflow = "B_Reflow";
		/// <summary>
		/// 折射
		/// 将<color=#FF6767>湮裂的燐焰晶</color>的伤害和附加效果，折射给生命值最低的敌人。
		/// </summary>
        public static string Buff_B_Refraction = "B_Refraction";
		/// <summary>
		/// 燐色存护
		/// 每层能抵抗1次无法战斗效果。
		/// 濒死状态时，无法战斗抗性会增加受到的治疗量<color=#5061A4>（&a%）</color>。
		/// </summary>
        public static string Buff_B_Shield = "B_Shield";
		/// <summary>
		/// 震荡冲击
		/// 无法使用攻击技能，若是敌人则停止行动一回合。
		/// 持有此效果的角色可以通过丢弃自己的技能来移除此效果。
		/// </summary>
        public static string Buff_B_Shock = "B_Shock";
		/// <summary>
		/// 暖焰
		/// 每当受到<color=#5061A4>&a</color>点以上的治疗量时，赋予1层燐色存护。
		/// 无法战斗抗性<color=#5061A4>（&b%）</color>会减少此治疗量阈值（已减少<color=#5061A4>&c</color>）。
		/// </summary>
        public static string Buff_B_WarmFire = "B_WarmFire";
		/// <summary>
		/// 梅朗柯莉
		/// Passive:
		/// 任何攻击会让梅朗柯莉变得<color=#FFC5BA><b>慌张</b></color>，但在回合开始时，梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 固定技能始终为<color=#FF6767>湮裂的燐焰晶</color>，会攻击除自己以外的所有人，并会摧毁周边障碍。
		/// 升级获得物品“燐焰晶”2个，进入营地会获得1个，可以摧毁地形并概率发现箱子。
		/// </summary>
        public static string Character_C_Merankori = "C_Merankori";
		/// <summary>
		/// 燐焰晶
		/// 右击可以让梅朗柯莉摧毁周边的墙体，可能发现小型物资。
		/// <color=#5061A4>  --- 赤红的晶石上透射着淡淡的回路，似乎很危险的样子...</color>
		/// </summary>
        public static string Item_Consume_I_RHA = "I_RHA";
		/// <summary>
		/// 逆流的燐焰晶
		/// 根据所有队友已损失的体力值增加伤害，每2点增加1%伤害。
		/// <color=#5061A4>（预期增加&a）</color>
		/// </summary>
        public static string SkillExtended_SE_Backflow = "SE_Backflow";
		/// <summary>
		/// 冷静
		/// 可以触发冷静效果
		/// </summary>
        public static string SkillExtended_SE_Calm = "SE_Calm";
		/// <summary>
		/// 蓄能
		/// 根据蓄能效果层数，每层增加10%的伤害。
		/// <color=#5061A4>（预期增加&a）</color>
		/// </summary>
        public static string SkillExtended_SE_Charge = "SE_Charge";
		/// <summary>
		/// 越频晶石
		/// 下一次<color=#FF6767>湮裂的燐焰晶</color>会附带“震荡冲击”效果。
		/// </summary>
        public static string SkillExtended_SE_OverClocking = "SE_OverClocking";
		/// <summary>
		/// 慌张
		/// 可以触发慌张效果
		/// </summary>
        public static string SkillExtended_SE_Panic = "SE_Panic";
		/// <summary>
		/// 整流
		/// 除了已指向的队友，技能也会应用给其他全体队友
		/// </summary>
        public static string SkillExtended_SE_Rectification = "SE_Rectification";
        public static string SkillEffect_SE_S_S_Charge = "SE_S_S_Charge";
        public static string SkillEffect_SE_S_S_ElementHeal = "SE_S_S_ElementHeal";
        public static string SkillEffect_SE_S_S_Manifold = "SE_S_S_Manifold";
        public static string SkillEffect_SE_S_S_OverClocking = "SE_S_S_OverClocking";
        public static string SkillEffect_SE_S_S_Rectification = "SE_S_S_Rectification";
        public static string SkillEffect_SE_S_S_Shield = "SE_S_S_Shield";
        public static string SkillEffect_SE_S_S_WarmFire = "SE_S_S_WarmFire";
        public static string SkillEffect_SE_Tick_B_Backflow = "SE_Tick_B_Backflow";
        public static string SkillEffect_SE_Tick_B_Calm = "SE_Tick_B_Calm";
        public static string SkillEffect_SE_Tick_B_CalmDown = "SE_Tick_B_CalmDown";
        public static string SkillEffect_SE_Tick_B_Panic = "SE_Tick_B_Panic";
        public static string SkillEffect_SE_T_B_Attack_All = "SE_T_B_Attack_All";
        public static string SkillEffect_SE_T_S_Backup = "SE_T_S_Backup";
        public static string SkillEffect_SE_T_S_Care = "SE_T_S_Care";
        public static string SkillEffect_SE_T_S_D_RefractionAtk = "SE_T_S_D_RefractionAtk";
        public static string SkillEffect_SE_T_S_ElementHeal = "SE_T_S_ElementHeal";
        public static string SkillEffect_SE_T_S_ItemRHA = "SE_T_S_ItemRHA";
        public static string SkillEffect_SE_T_S_Manifold = "SE_T_S_Manifold";
        public static string SkillEffect_SE_T_S_OverClocking = "SE_T_S_OverClocking";
        public static string SkillEffect_SE_T_S_P_Shock = "SE_T_S_P_Shock";
        public static string SkillEffect_SE_T_S_Reconstruct = "SE_T_S_Reconstruct";
        public static string SkillEffect_SE_T_S_Retreat = "SE_T_S_Retreat";
        public static string SkillEffect_SE_T_S_Shield = "SE_T_S_Shield";
        public static string SkillEffect_SE_T_S_WarmFire = "SE_T_S_WarmFire";
		/// <summary>
		/// 赋予折射
		/// 获得1回合折射
		/// 过载，指向单体队友的技能
		/// </summary>
        public static string SkillExtended_SE_U_Refraction = "SE_U_Refraction";
		/// <summary>
		/// 赋予燐色存护
		/// 获得1层燐色存护
		/// 1费及以上的技能
		/// </summary>
        public static string SkillExtended_SE_U_Shield = "SE_U_Shield";
		/// <summary>
		/// 冷静
		/// 梅朗柯莉处于“<color=#BAC8FF><b>冷静</b></color>”状态时触发。
		/// </summary>
        public static string SkillKeyword_SK_Calm = "SK_Calm";
		/// <summary>
		/// 慌张
		/// 处于“<color=#FFC5BA><b>慌张</b></color>”状态时触发
		/// </summary>
        public static string SkillKeyword_SK_Panic = "SK_Panic";
		/// <summary>
		/// <color=#FF6767>湮裂的燐焰晶</color>
		/// 对所有人造成伤害 ,除了梅朗柯莉。此伤害不会导致队友无法战斗。
		/// 在手中时，附加放逐。
		/// <color=#FFC5BA><b>慌张</b>：技能附加迅速</color>
		/// </summary>
        public static string Skill_S_Attack_All = "S_Attack_All";
		/// <summary>
		/// 后备的燐焰晶
		/// 抽1张牌，
		/// 将<color=#FF6767>湮裂的燐焰晶</color>放入手中，直到填满手牌。
		/// </summary>
        public static string Skill_S_Backup = "S_Backup";
		/// <summary>
		/// 悉心照料
		/// <color=#BAC8FF><b>冷静</b>：目标持有“燐焰晶”相关的正面效果，会额外增加1层</color>
		/// </summary>
        public static string Skill_S_Care = "S_Care";
		/// <summary>
		/// 蓄能
		/// 每行进1点倒计时，获得3层蓄能
		/// 慌张时，停止蓄能。
		/// 冷静时完成蓄能，将2张<color=#FF6767>湮裂的燐焰晶</color>放入手中
		/// <color=#FFC5BA>慌张：冷静下来，若完成蓄能，再抽取1个技能</color>
		/// </summary>
        public static string Skill_S_Charge = "S_Charge";
		/// <summary>
		/// 焰晶转化
		/// 丢弃选择的技能，抽取1个技能。
		/// 将1张<color=#FF6767>湮裂的燐焰晶</color>放入手中。
		/// 丢弃技能的拥有者获得1层折射和1层频移微调。
		/// </summary>
        public static string Skill_S_ConvertCard = "S_ConvertCard";
		/// <summary>
		/// 折射
		/// 折射来自“湮裂的燐焰晶”的伤害
		/// </summary>
        public static string Skill_S_D_RefractionAtk = "S_D_RefractionAtk";
		/// <summary>
		/// 元素调石
		/// 恢复全部体力极限。
		/// 每恢复&a体力极限，赋予自身1层“蓄力”。
		/// <color=#5061A4>（预计提供&b层）</color>
		/// </summary>
        public static string Skill_S_ElementHeal = "S_ElementHeal";
		/// <summary>
		/// 晶石生成
		/// 抽1张牌，获得燐焰晶物品1个，可以用于摧毁地图上的墙体。
		/// </summary>
        public static string Skill_S_ItemRHA = "S_ItemRHA";
		/// <summary>
		/// 晶体流形
		/// <color=#BAC8FF><b>冷静</b>：手中每个<color=#FF6767>湮裂的燐焰晶</color>会额外提供3层“蓄力”</color>
		/// <color=#5061A4>（预计提供&a层）</color>
		/// </summary>
        public static string Skill_S_Manifold = "S_Manifold";
		/// <summary>
		/// 越频晶石
		/// <color=#BAC8FF><b>冷静</b>：延长“折射”1回合</color>
		/// </summary>
        public static string Skill_S_OverClocking = "S_OverClocking";
		/// <summary>
		/// 震荡冲击
		/// 预览效果
		/// </summary>
        public static string Skill_S_P_Shock = "S_P_Shock";
		/// <summary>
		/// 碎晶重构
		/// </summary>
        public static string Skill_S_Reconstruct = "S_Reconstruct";
		/// <summary>
		/// 回路整流
		/// 可以将梅朗柯莉指向单体队友的专属技能更改为对“全体友军”释放。
		/// 将1张<color=#FF6767>湮裂的燐焰晶</color>放入手中。
		/// <color=#BAC8FF>冷静：抽取1个技能</color>
		/// </summary>
        public static string Skill_S_Rectification = "S_Rectification";
		/// <summary>
		/// 后撤！
		/// 抽取2个技能。
		/// 若指向的队友拥有至少5个正面效果，额外抽取1张牌。
		/// </summary>
        public static string Skill_S_Retreat = "S_Retreat";
		/// <summary>
		/// 燐晶电场
		/// <color=#BAC8FF><b>冷静</b>：处于濒死状态的队友额外获得2层“燐色存护”</color>
		/// </summary>
        public static string Skill_S_Shield = "S_Shield";
		/// <summary>
		/// 暖焰
		/// </summary>
        public static string Skill_S_WarmFire = "S_WarmFire";

    }

    public static class ModLocalization
    {

    }
}