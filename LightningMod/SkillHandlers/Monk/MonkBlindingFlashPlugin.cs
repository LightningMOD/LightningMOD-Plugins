namespace Turbo.Plugins.LightningMod
{
    public class MonkBlindingFlashPlugin : AbstractSkillHandler, ISkillHandler
	{
        public MonkBlindingFlashPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Monk_BlindingFlash;

            CreateCastRule()
                .IfCanCastSkill(150, 200, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfRunning().ThenNoCastElseContinue()
                .IfIdle().ThenNoCastElseContinue()
                .IfBossIsNearby(ctx => 20).ThenNoCastElseContinue()
                .IfEliteIsNearby(ctx => 20).ThenCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 20, ctx => 5).ThenCastElseContinue()
                ;

            CreateCastRule()//ɪ��֮��������
                .IfCanCastSkill(150, 200, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfRunning().ThenNoCastElseContinue()
                .IfIdle().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.TheLawsOfSeph).ThenContinueElseNoCast()
                .IfPrimaryResourcePercentageIsBelow(20).ThenCastElseContinue()
                ;
        }
    }
}