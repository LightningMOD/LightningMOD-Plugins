namespace Turbo.Plugins.LightningMod
{
    public class MonkInnerSanctuaryPlugin : AbstractSkillHandler, ISkillHandler
	{
        public MonkInnerSanctuaryPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Monk_InnerSanctuary;

            CreateCastRule()
                .IfCanCastSkill(150, 200, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfRunning().ThenNoCastElseContinue()
                .IfIdle().ThenNoCastElseContinue()
                .IfTrue(ctx =>
                {
                    bool isDPS = hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.BaneOfTheTrappedPrimary.Sno);//РЇеп
                    return isDPS ? ctx.Skill.BuffIsActive == false : true;
                }).ThenContinueElseNoCast()
                .IfEliteOrBossIsNearby(ctx => 50).ThenCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 50, ctx => 5).ThenCastElseContinue()
                ;

        }
    }
}