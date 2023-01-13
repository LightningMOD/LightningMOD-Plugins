namespace Turbo.Plugins.LightningMod
{
    using System.Linq;
    using System;
    public class DemonHunterSmokeScreenPlugin : AbstractSkillHandler, ISkillHandler
    {
        public DemonHunterSmokeScreenPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_SmokeScreen;

            CreateCastRule()//��ͨ����100�����й�Σ��ʱʩ��
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.BuffIsActive).ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Hud.Avoidance.CurrentValue).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.AvoidablesInRange.Any(x => x.AvoidableDefinition.InstantDeath)).ThenCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfHealthWarning(60, 80).ThenCastElseContinue();

            CreateCastRule()//�Զ����ֲ�Ӱ��ָBUFF
                .IfTrue(ctx => ctx.Skill.Player.Powers.UsedLegendaryPowers.ElusiveRing?.Active == true).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.ElusiveRing, 1, 100, 150).ThenCastElseContinue();//��Ӱ��ָBuff

            CreateCastRule()//��������+���ƴ���+80����10+�ֻ�Ӣ��BOSSʩ��
               .IfTrue(ctx => ctx.Skill.Rune == 3 && (ctx.Skill.Player.Stats.ResourceCostReduction > 0.8 ? true : ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null)).ThenContinueElseNoCast()//��������+���ƴ���
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfOnCooldown().ThenNoCastElseContinue()
               .IfCanCastSimple().ThenContinueElseNoCast()
               .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
               .IfEnoughMonstersNearby(ctx => 80, ctx => 10).ThenCastElseContinue()
               .IfEliteOrBossIsNearby(ctx => 80).ThenCastElseContinue()
               ;

            CreateCastRule()//��������+���ƴ���+80������ÿ2.8~3��ʩ��һ��
               .IfTrue(ctx => ctx.Skill.Rune == 3 && (ctx.Skill.Player.Stats.ResourceCostReduction > 0.8 ? true : ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null)).ThenContinueElseNoCast()//��������+���ƴ���
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfOnCooldown().ThenNoCastElseContinue()
               .IfCanCastSimple().ThenContinueElseNoCast()
               .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
               .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenContinueElseNoCast()
               .IfCanCastSkill(2800, 3000, 5000).ThenCastElseContinue()//ÿ2.8~3��ʩ��һ��
               ;

            CreateCastRule()//Ʈ������+���ƴ���+���ĸ���40%+80������+�ƶ�ʩ��
                 .IfTrue(ctx =>
                 {
                     float ResourceCostReduction = ctx.Skill.Player.Stats.ResourceCostReduction;
                     return ctx.Skill.Rune == 4 && ResourceCostReduction >= 0.4 && (ResourceCostReduction > 0.8 ? true : ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null);
                     }
                 ).ThenContinueElseNoCast()//Ʈ������+���ƴ���+���ĸ���40%
                 .IfInTown().ThenNoCastElseContinue()
                 .IfCastingIdentify().ThenNoCastElseContinue()
                 .IfCastingPortal().ThenNoCastElseContinue()
                 .IfOnCooldown().ThenNoCastElseContinue()
                 .IfCanCastSimple().ThenContinueElseNoCast()
                 .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                 .IfRunning(true).ThenCastElseContinue()//�ƶ�ʱʩ��
                 .IfEliteOrBossIsNearby(ctx => 80).ThenCastElseContinue()//80�����о�Ӣ��BOSSʱʩ��
                 ;

            CreateCastRule()//Ʈ������+���ƴ���+80������+�ƶ�ʩ��
               .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenContinueElseNoCast()//80������������ʹ�øù���
               .IfTrue(ctx => ctx.Skill.Rune == 4 && (ctx.Skill.Player.Stats.ResourceCostReduction > 0.8 ? true : ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null)).ThenContinueElseNoCast()//Ʈ������+���ƴ���
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfOnCooldown().ThenNoCastElseContinue()
               .IfCanCastSimple().ThenContinueElseNoCast()
               .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
               .IfRunning(true).ThenCastElseContinue()//�ƶ�ʱʩ��
               .IfEliteOrBossIsNearby(ctx => 80).ThenCastElseContinue()//80�����о�Ӣ��BOSSʱʩ��
               ;

            CreateCastRule()//Ʈ������+���ƴ���+�޵���+�ƶ�ʱÿ2.8~3��ʩ��һ��
               .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenNoCastElseContinue()//80�����������˲�ʹ�øù���
               .IfTrue(ctx => ctx.Skill.Rune == 4 && (ctx.Skill.Player.Stats.ResourceCostReduction > 0.8 ? true : ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null)).ThenContinueElseNoCast()//Ʈ������+���ƴ���
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfOnCooldown().ThenNoCastElseContinue()
               .IfCanCastSimple().ThenContinueElseNoCast()
               .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
               .IfRunning(true).ThenContinueElseNoCast()//���ƶ���ʩ��
               .IfCanCastSkill(2800, 3000, 5000).ThenCastElseContinue()//ÿ2.8~3��ʩ��һ��
               ;

            CreateCastRule()//Ʈ������+���ƴ���+80������+�ƶ�ʩ��
               .IfTrue(ctx => ctx.Skill.Rune == 0 && ctx.Skill.Player.Powers.UsedLegendaryPowers.Ingeom?.Active == true).ThenContinueElseNoCast()//��ʧ��ĩ+����
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfOnCooldown().ThenNoCastElseContinue()
               .IfCanCastSimple().ThenContinueElseNoCast()
               .IfTrue(ctx => ctx.Skill.BuffIsActive).ThenNoCastElseContinue()
               .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenCastElseContinue()//80�����й�ʱʩ��
               ;
        }
    }
}