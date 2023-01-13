namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterPreparationPunishmentPlugin : AbstractSkillHandler, ISkillHandler
    {
        public DemonHunterPreparationPunishmentPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_Preparation;
            Rune = 0;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfPrimaryResourcePercentageIsBelow(30).ThenCastElseContinue()
                ;
        }
    }
}