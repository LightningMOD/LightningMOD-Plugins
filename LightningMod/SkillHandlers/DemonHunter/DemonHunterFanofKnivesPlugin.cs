namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterFanofKnivesPlugin : AbstractSkillHandler, ISkillHandler
	{
        public DemonHunterFanofKnivesPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_FanOfKnives;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Rune == 4).ThenContinueElseNoCast()//µ¶ÈĞ»¤¼×
                .IfBuffIsAboutToExpire(100, 200).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 40, ctx => 1).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.Stats.CooldownReduction * 100 > 40).ThenCastElseContinue()//CDR´óÓÚ40%
                ;
        }
    }
}