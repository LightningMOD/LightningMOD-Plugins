namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterRainofVengeancePlugin : AbstractSkillHandler, ISkillHandler
	{
        public DemonHunterRainofVengeancePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_RainOfVengeance;

            CreateCastRule()
                .IfCanCastSkill(40, 50, 100).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx => Hud.Game.Me.GetSetItemCount(635131) == 6).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.Generic_P2ItemPassiveUniqueRing053, 1, 300, 500).ThenCastElseContinue()//ÄÈËþÑÇ6¼þ
                ;
        }
    }
}