namespace Turbo.Plugins.LightningMod
{
    using Turbo.Plugins.glq;
    public class CrusaderLawsOfValorPlugin : AbstractSkillHandler, ISkillHandler
	{
        public CrusaderLawsOfValorPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Crusader_LawsOfValor;

            CreateCastRule()//一般规则
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//骑马时
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_X1CrusaderLawsOfValorPassive2, 6).ThenNoCastElseContinue()//已激活
                .IfTrue(ctx =>
                (ctx.Hud.Game.Me.Powers.BuffIsActive(310678) ||//律法无边
                ctx.Skill.Player.Stats.CooldownReduction >= 0.5 || ctx.Hud.Game.Me.Powers.BuffIsActive(402459)) && //CDR大于50或带了黄道
                (!isSanGuang() ||
                (isSanGuang() && (ctx.Hud.Game.RiftPercentage < 100 || !ctx.Skill.Player.InGreaterRift)))//大秘境进度阶段
                ).ThenCastElseContinue()
                ;
            CreateCastRule()//三光BOSS战规则
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//骑马时
                .IfTrue(ctx =>
                (isSanGuang() && ctx.Hud.Game.RiftPercentage == 100 && ctx.Hud.Game.ActorQuery.IsEliteOrBossCloserThan(30) && (PublicClassPlugin.IsElementReady (Hud, 0.5, ctx.Skill.Player, 4) || //大秘境BOSS阶段，神圣前0.5秒激活
                (ctx.Hud.Game.Me.Powers.BuffIsActive(310678) &&//律法无边
                ctx.Skill.Player.Stats.CooldownReduction >= 0.6518)))//无缝释放的基础条件
                ).ThenCastElseContinue()
                ;
        }

        private bool isSanGuang()
        {
            bool isHeavensFury = Hud.Game.Me.Powers.UsedCrusaderPowers.HeavensFury?.Rune == 4; //天堂之火
            bool isAegisofValor = Hud.Game.Me.GetSetItemCount(192736) >= 6;//勇气6件套
            bool isFateoftheFell = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.FateOfTheFell.Sno, 0);//妖邪必败
            bool isConventionOfElements = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ConventionOfElements.Sno, 0);//元素戒指
            return isAegisofValor && isHeavensFury && isFateoftheFell && isConventionOfElements;
        }
    }
}