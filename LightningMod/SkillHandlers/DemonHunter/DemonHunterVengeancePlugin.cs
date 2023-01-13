using Turbo.Plugins.glq;
namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterVengeancePlugin : AbstractSkillHandler, ISkillHandler
    {
        public DemonHunterVengeancePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack, CastPhase.PreAttack)
        {
            Enabled = false;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_Vengeance;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx => (ctx.Skill.Rune == 1 || ctx.Skill.Rune == 3) && ctx.Skill.Player.Defense.HealthPct < 60).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(50, 100).ThenCastElseContinue()
                ;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEliteOrBossIsNearby(ctx => 100, false).ThenContinueElseNoCast()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.ConventionOfElements).ThenContinueElseNoCast()//Ԫ�ؽ�ָ
                .IfTrue(ctx => {
                    int CoeIndex = Hud.GetPlugin<PublicClassPlugin>().CoeIndex;
                    double HighestElementLeft = PublicClassPlugin.GetHighestElementLeftSecond(hud, ctx.Skill.Player, CoeIndex);
                    return HighestElementLeft <= 16 && HighestElementLeft >= 15;//����1����ʩ��
                }).ThenCastElseContinue()
                ;

            CreateCastRule()//�Ƶ���������Χ�о�Ӣ��10��С��ʩ�ţ�CDR29%����ʱ��Χ����1������ʩ��
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(402459) || ctx.Skill.Player.Powers.BuffIsActive(446146, 0)).ThenContinueElseNoCast()//�Ƶ�����ʱ����������ʱ
                .IfEliteOrBossIsNearby(ctx => 60).ThenCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 60, ctx => 10).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.Stats.CooldownReduction * 100 > 29).ThenContinueElseNoCast()//����29%CDR
                .IfBuffIsAboutToExpire(50, 100).ThenCastElseContinue()
                ;

            CreateCastRule()// �Ƶ�����ʱ����������ʱ��36 % CDRʱ����ʩ��
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(402459) || ctx.Skill.Player.Powers.BuffIsActive(446146, 0)).ThenContinueElseNoCast()//�Ƶ�����ʱ����������ʱ
                .IfTrue(ctx => ctx.Skill.Player.Stats.CooldownReduction * 100 > 36).ThenContinueElseNoCast()//����36%CDR
                .IfBuffIsAboutToExpire(50, 100).ThenCastElseContinue()
                ;

            CreateCastRule()//����ŷ������һƵ�������������30������ʱʩ��
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Rune == 4).ThenContinueElseNoCast()//����ŷ�
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(402459) || ctx.Skill.Player.Powers.BuffIsActive(446146, 0)).ThenContinueElseNoCast()//�Ƶ�����ʱ����������ʱ
                .IfPrimaryResourceAmountIsBelow(ctx => 30).ThenContinueElseNoCast()//���޵���30��ʱ
                .IfBuffIsAboutToExpire(50, 100).ThenCastElseContinue()
                ;
        }
    }
}