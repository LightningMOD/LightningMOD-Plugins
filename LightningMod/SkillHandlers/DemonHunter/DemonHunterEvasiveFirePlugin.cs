namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterEvasiveFirePlugin : AbstractSkillHandler, ISkillHandler
	{
        public DemonHunterEvasiveFirePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_EvasiveFire;
            //保持对戒
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue() 
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && ctx.Skill.Player.Powers.BuffIsActive(359583, 0)).ThenContinueElseNoCast()//强化硬甲或凝神射击 且 装备守心克己
                .IfEnoughMonstersNearbyCursor(ctx => 30, ctx => 1).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.Generic_ItemPassiveUniqueRing735x1, 1, 30, 100).ThenCastElseContinue()
                ;
            //保持明彻裹腕
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && Hud.Game.Me.Powers.UsedLegendaryPowers.WrapsOfClarity?.Active == true).ThenContinueElseNoCast()//强化硬甲或凝神射击 且 装备明彻裹腕
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInvulnerable, 0).ThenNoCastElseContinue()//护盾
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.WrapsOfClarity, 1, 30, 300).ThenCastElseContinue()
                ;
            //保持憎恨
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4)).ThenContinueElseNoCast()//强化硬甲或凝神射击
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0).ThenNoCastElseContinue()//减耗
                .IfPrimaryResourcePercentageIsBelow(20).ThenCastElseContinue()
                ;
            //保持戒律
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && Hud.Game.Me.GetSetItemCount(254164) >= 2).ThenContinueElseNoCast()//强化硬甲或凝神射击 且不洁两件套及以上
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0).ThenNoCastElseContinue()//减耗
                .IfSecondaryResourcePercentageIsBelow(33).ThenCastElseContinue()
                ;
        }
    }
}