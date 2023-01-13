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
            //���ֶԽ�
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue() 
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && ctx.Skill.Player.Powers.BuffIsActive(359583, 0)).ThenContinueElseNoCast()//ǿ��Ӳ�׻�������� �� װ�����Ŀ˼�
                .IfEnoughMonstersNearbyCursor(ctx => 30, ctx => 1).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.Generic_ItemPassiveUniqueRing735x1, 1, 30, 100).ThenCastElseContinue()
                ;
            //������������
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && Hud.Game.Me.Powers.UsedLegendaryPowers.WrapsOfClarity?.Active == true).ThenContinueElseNoCast()//ǿ��Ӳ�׻�������� �� װ����������
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInvulnerable, 0).ThenNoCastElseContinue()//����
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.WrapsOfClarity, 1, 30, 300).ThenCastElseContinue()
                ;
            //��������
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4)).ThenContinueElseNoCast()//ǿ��Ӳ�׻��������
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0).ThenNoCastElseContinue()//����
                .IfPrimaryResourcePercentageIsBelow(20).ThenCastElseContinue()
                ;
            //���ֽ���
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 0 || ctx.Skill.Rune == 4) && Hud.Game.Me.GetSetItemCount(254164) >= 2).ThenContinueElseNoCast()//ǿ��Ӳ�׻�������� �Ҳ��������׼�����
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0).ThenNoCastElseContinue()//����
                .IfSecondaryResourcePercentageIsBelow(33).ThenCastElseContinue()
                ;
        }
    }
}