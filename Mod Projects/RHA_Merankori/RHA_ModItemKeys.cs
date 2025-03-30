using ChronoArkMod;
namespace RHA_Merankori
{
    public static class ModItemKeys
    {
		/// <summary>
		/// 逆流的燐焰晶
		/// “湮裂的燐焰晶”会根据队友已损失的体力值增加伤害，每1点增加1%伤害
		/// （已增加&a）
		/// 回合结束时，若手中有“湮裂的燐焰晶”，移除这个效果。
		/// </summary>
        public static string Buff_B_Backflow = "B_Backflow";
		/// <summary>
		/// 冷静
		/// 照料可能因自己受伤的队友...
		/// </summary>
        public static string Buff_B_Calm = "B_Calm";
		/// <summary>
		/// 镇定
		/// 阻止进入慌张状态，每次阻止会减少1层。
		/// </summary>
        public static string Buff_B_CalmDown = "B_CalmDown";
		/// <summary>
		/// 蓄能
		/// 释放“湮裂的燐焰晶”时，每层增加其25%的伤害。
		/// 释放后移除此效果。
		/// </summary>
        public static string Buff_B_Charge = "B_Charge";
		/// <summary>
		/// 无法战斗抵抗
		/// </summary>
        public static string Buff_B_DeathResist = "B_DeathResist";
		/// <summary>
		/// 频移微调
		/// 受到“湮裂的燐焰晶”的攻击时，将这个攻击转化为等量的治疗，然后移除1层。
		/// </summary>
        public static string Buff_B_FreqShift = "B_FreqShift";
		/// <summary>
		/// 焰晶生成
		/// 每当有人脱离或进入濒死状态时，将“湮裂的燐焰晶”放入手中。
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
		/// 越频晶体
		/// 下一次“湮裂的燐焰晶”会附带“震荡冲击”效果
		/// </summary>
        public static string Buff_B_OverClocking = "B_OverClocking";
		/// <summary>
		/// 慌张
		/// 燐焰晶逐渐炽烈...
		/// </summary>
        public static string Buff_B_Panic = "B_Panic";
		/// <summary>
		/// 折射
		/// 将“湮裂的燐焰晶”的伤害和附加效果，折射给生命值最低的敌人。
		/// </summary>
        public static string Buff_B_Refraction = "B_Refraction";
		/// <summary>
		/// 燐色存护
		/// 每层能抵抗1次不能战斗效果，必定不能战斗的效果也能抵抗。
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
		/// 每当获得等体力值的治疗量，赋予1层燐色存护。
		/// （已累计&a）
		/// </summary>
        public static string Buff_B_WarmFire = "B_WarmFire";
		/// <summary>
		/// 梅朗柯莉
		/// Passive:
		/// 情绪高涨的梅朗柯莉会激化身边的红色结晶，会对周围不分敌我地产生毁灭性的伤害，甚至摧毁地形。
		/// 但冷静下来的梅朗柯莉又会悉心照料因自己而受伤的队友。
		/// 任何攻击都会让她变得情绪慌张，但在回合开始时，梅朗柯莉会冷静下来。
		/// 固定技能始终为“湮裂的燐焰晶”
		/// lv.2获得物品“活化燐焰晶”
		/// </summary>
        public static string Character_C_Merankori = "C_Merankori";
		/// <summary>
		/// 逆流的燐焰晶
		/// 根据队友已损失的体力值增加伤害，每1点增加1%伤害。
		/// 已增加&a
		/// </summary>
        public static string SkillExtended_SE_Backflow = "SE_Backflow";
		/// <summary>
		/// 越频晶体
		/// 根据蓄能效果层数，每层增加25%的伤害。
		/// （已增加&a）
		/// </summary>
        public static string SkillExtended_SE_Charge = "SE_Charge";
		/// <summary>
		/// 越频晶体
		/// 下一次“湮裂的燐焰晶”会附带“震荡冲击”效果。
		/// </summary>
        public static string SkillExtended_SE_OverClocking = "SE_OverClocking";
		/// <summary>
		/// 整流
		/// 除了已指向的队友，技能也会应用给其他全体队友
		/// </summary>
        public static string SkillExtended_SE_Rectification = "SE_Rectification";
        public static string SkillEffect_SE_S_S_Charge = "SE_S_S_Charge";
        public static string SkillEffect_SE_S_S_OverClocking = "SE_S_S_OverClocking";
        public static string SkillEffect_SE_S_S_Rectification = "SE_S_S_Rectification";
        public static string SkillEffect_SE_S_S_Shield = "SE_S_S_Shield";
        public static string SkillEffect_SE_S_S_WarmFire = "SE_S_S_WarmFire";
        public static string SkillEffect_SE_Tick_B_Backflow = "SE_Tick_B_Backflow";
        public static string SkillEffect_SE_Tick_B_Calm = "SE_Tick_B_Calm";
        public static string SkillEffect_SE_Tick_B_CalmDown = "SE_Tick_B_CalmDown";
        public static string SkillEffect_SE_Tick_B_Panic = "SE_Tick_B_Panic";
        public static string SkillEffect_SE_T_B_Attack_All = "SE_T_B_Attack_All";
        public static string SkillEffect_SE_T_S_Care = "SE_T_S_Care";
        public static string SkillEffect_SE_T_S_D_RefractionAtk = "SE_T_S_D_RefractionAtk";
        public static string SkillEffect_SE_T_S_OverClocking = "SE_T_S_OverClocking";
        public static string SkillEffect_SE_T_S_P_Shock = "SE_T_S_P_Shock";
        public static string SkillEffect_SE_T_S_Shield = "SE_T_S_Shield";
        public static string SkillEffect_SE_T_S_WarmFire = "SE_T_S_WarmFire";
		/// <summary>
		/// 冷静
		/// 梅朗柯莉处于“冷静”状态时触发
		/// </summary>
        public static string SkillKeyword_SK_Calm = "SK_Calm";
		/// <summary>
		/// 慌张
		/// 处于“慌张”状态时触发
		/// </summary>
        public static string SkillKeyword_SK_Panic = "SK_Panic";
		/// <summary>
		/// 湮裂的燐焰晶
		/// 对所有人造成伤害 ,除了梅朗柯莉。
		/// 这个伤害不会导致队友不能战斗。
		/// 不作为固定技能使用时，赋予放逐。
		/// 慌张：技能附加迅速。
		/// </summary>
        public static string Skill_S_Attack_All = "S_Attack_All";
		/// <summary>
		/// 悉心照料
		/// 冷静：目标持有“燐焰晶”相关的正面效果，再额外增加1层
		/// </summary>
        public static string Skill_S_Care = "S_Care";
		/// <summary>
		/// 蓄能
		/// 队友每拥有2层“燐色存护”，倒计时增加1点。每行进1点倒计时，获得1层蓄能。
		/// 进入慌张状态时，停止蓄能。
		/// </summary>
        public static string Skill_S_Charge = "S_Charge";
		/// <summary>
		/// 折射
		/// 折射来自“湮裂的燐焰晶”的伤害
		/// </summary>
        public static string Skill_S_D_RefractionAtk = "S_D_RefractionAtk";
		/// <summary>
		/// 越频晶体
		/// 冷静：延长“折射”1回合
		/// </summary>
        public static string Skill_S_OverClocking = "S_OverClocking";
		/// <summary>
		/// 震荡冲击
		/// 预览效果
		/// </summary>
        public static string Skill_S_P_Shock = "S_P_Shock";
		/// <summary>
		/// 整流
		/// 指向梅朗柯莉“指向队友”或“其他友军”的技能时，改成“全体友军” 。
		/// 指向不符合条件的卡时，将一张“湮裂的燐焰晶”放入手中。
		/// </summary>
        public static string Skill_S_Rectification = "S_Rectification";
		/// <summary>
		/// 燐晶盾
		/// 冷静：处于濒死状态的队友额外获得3层“燐色存护”
		/// </summary>
        public static string Skill_S_Shield = "S_Shield";
		/// <summary>
		/// 暖焰
		/// </summary>
        public static string Skill_S_WarmFire = "S_WarmFire";
		/// <summary>
		/// 碎晶重构
		/// </summary>
        public static string Skill_S_Reconstruct = "S_Reconstruct";
        public static string SkillEffect_SE_T_S_Reconstruct = "SE_T_S_Reconstruct";
		/// <summary>
		/// 燐焰晶回流
		/// 溢出的燐色存护，会转化为折射。
		/// 回合开始时，若没有燐色存护，获得1层燐色存护。
		/// </summary>
        public static string Buff_B_Reflow = "B_Reflow";

    }

    public static class ModLocalization
    {

    }
}