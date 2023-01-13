namespace Turbo.Plugins.LightningMod
{
    using System.Linq;
    public class WitchDoctorBigBadVoodooPlugin : AbstractSkillHandler, ISkillHandler
    {
        public WitchDoctorBigBadVoodooPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.WitchDoctor_BigBadVoodoo;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    var count = Hud.Game.Actors.Count(actor => 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_melee_a || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_doublestack_shaman_a || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_ranged_a || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_melee_itempassive || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_shaman_a || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_skeleton_a || 
                    actor.SnoActor.Sno == ActorSnoEnum._fetish_melee_sycophants);
                    return ((count > 3 && hud.Game.Me.Powers.BuffIsActive(318724)) || hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.WitchDoctor_Passive_GraveInjustice.Sno));//3个以上鬼娃并带星铁或剥削死者
                }).ThenContinueElseNoCast()
                .IfEliteOrBossIsNearby(ctx => 40).ThenCastElseContinue()
                ;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    return (hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.WitchDoctor_Passive_GraveInjustice.Sno) && hud.Game.Me.Powers.BuffIsActive(484128));//剥削死者和套蒙嘟噜咕2件
                }).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.WitchDoctor_BigBadVoodoo, 4, 500, 1000).ThenCastElseContinue()
                ;
            
        }
    }
}