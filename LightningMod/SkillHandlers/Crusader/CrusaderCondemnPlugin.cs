namespace Turbo.Plugins.LightningMod
{
    public class CrusaderCondemnPlugin : AbstractSkillHandler, ISkillHandler
	{
        public CrusaderCondemnPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Collect, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Crusader_Condemn;

            CreateCastRule()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//ÆïÂíÊ±
                .IfPrimaryResourceIsEnough(0, ctx => 40).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => (int)(20 * (1 + Hud.Game.Me.Stats.MoveSpeed / 100)) * 3 + ((ctx.Skill.Rune == 1 && ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.FrydehrsWrath.Sno)) ? 10 : -15), ctx => 1).ThenCastElseContinue()
                ;
        }
    }
}