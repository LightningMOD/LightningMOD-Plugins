using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class CrusaderSteedChargePlugin : AbstractSkillHandler, ISkillHandler
	{
        public bool NotWorkForDrawandQuarter { get; set; }
        public CrusaderSteedChargePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Collect, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Crusader_SteedCharge;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//����ʱ
                .IfTrue(ctx => NotWorkForDrawandQuarter && ctx.Skill.Rune == 4).ThenNoCastElseContinue()//ս�����з���ʱ����Ч
                .IfTrue(ctx => ctx.Skill.Rune == 0 && ctx.Hud.Interaction.IsHotKeySet(ActionKey.Move) && ctx.Hud.Interaction.IsContinuousActionStarted(ActionKey.Move)).ThenCastElseContinue()//��������Ұ�סǿ���ƶ�ʱ����ʩ��
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(447290)).ThenContinueElseNoCast()//ս����
                .IfTrue(ctx => ctx.Hud.Game.AliveMonsters.Any(m =>(m.CurHealth / m.MaxHealth) < 0.99 && m.CentralXyDistanceToMe < (ctx.Skill.Player.Powers.BuffIsActive(403468, 0) ? 60 : 15))).ThenContinueElseNoCast()//��Χ15��������1����Ѫ������99%,������ʱΪ50��
                .IfFalse(ctx => ctx.Skill.Player.Powers.BuffIsActive(447291,1)).ThenCastElseContinue()
                ;
        }
    }
}