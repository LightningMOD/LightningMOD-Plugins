namespace Turbo.Plugins.LightningMod
{
    public class WizardExplosiveBlastPlugin : AbstractSkillHandler, ISkillHandler
    {
        public WizardExplosiveBlastPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Wizard_ExplosiveBlast;

            CreateCastRule()
                .IfCanCastSkill(100, 150, 500)
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfPrimaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => {
                    return ctx.Skill.Player.Powers.BuffIsActive(hud.Sno.SnoPowers.WandOfWoh.Sno) || //沃尔
                    ctx.Skill.Player.Stats.ResourcePctPri >= 70 || //法力高于70%
                    (ctx.Skill.Player.GetSetItemCount(84014) >= 6 && ctx.Skill.Player.Powers.BuffIsActive(hud.Sno.SnoPowers.Deathwish.Sno))//火鸟6+绝命
                    ;
                }).ThenContinueElseNoCast()//沃尔魔杖或法力高于70%时
                .IfEnoughMonstersNearby(GetBlastRange, ctx => 1).ThenCastElseContinue()
                .IfEliteOrBossIsNearby(GetBlastRange, true).ThenCastElseContinue();

            CreateCastRule()
                //保持无尽深渊BUFF
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.WandOfWoh).ThenNoCastElseContinue()
                .IfCanCastSkill(100, 150, 500)
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.OrbOfInfiniteDepth).ThenContinueElseNoCast()//无尽深渊法珠
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.Wizard_ExplosiveBlast, 1, 1700 ,2000).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(GetBlastRange, ctx => 1).ThenCastElseContinue()
                .IfEliteOrBossIsNearby(GetBlastRange, true).ThenCastElseContinue();
        }

        private int GetBlastRange(TestContext ctx)
        {
            switch (ctx.Skill.Rune)
            {
                case 0: return 12; // short fuse
                case 1: return 18 + (int)((20 * (1 + Hud.Game.Me.Stats.MoveSpeed / 100)) * 1.5); // obliterate
                default: return 12 + (int)((20 * (1 + Hud.Game.Me.Stats.MoveSpeed / 100)) * 1.5); // everything else
            }
        }
    }
}