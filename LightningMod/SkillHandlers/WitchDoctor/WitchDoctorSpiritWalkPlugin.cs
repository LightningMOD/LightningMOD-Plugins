namespace Turbo.Plugins.LightningMod
{
    using System.Linq;

    public class WitchDoctorSpiritWalkPlugin : AbstractSkillHandler, ISkillHandler
    {
        public WitchDoctorSpiritWalkPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.WitchDoctor_SpiritWalk;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => Hud.Game.Me.GetSetItemCount(585361) >= 4).ThenContinueElseNoCast()//√…‡Ω‡‡πæ4º˛
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.Generic_P68ItemPassiveUniqueRing009, 1,500,1000).ThenCastElseContinue()
                ;

        }
    }
}