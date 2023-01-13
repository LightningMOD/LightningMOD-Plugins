using Turbo.Plugins.glq;
namespace Turbo.Plugins.LightningMod
{
    public class WizardBlackHolePlugin : AbstractSkillHandler, ISkillHandler
    {
        public int SpareResource { get; set; }

        public WizardBlackHolePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Wizard_BlackHole;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx =>
                ctx.Skill.Player.GetSetItemCount(84014) >= 6 && ctx.Skill.Player.Powers.UsedLegendaryPowers.Deathwish?.Active == true).ThenNoCastElseContinue()//����6��ʱ��Ч
                .IfTrue(ctx =>{
                    double HighestElementLeftTime = glq.PublicClassPlugin.GetBuffLeftTime(Hud, Hud.Sno.SnoPowers.ConventionOfElements.Sno, glq.PublicClassPlugin.GetHighestElement(Hud, Hud.Game.Me));
                    if (HighestElementLeftTime == 0) HighestElementLeftTime = 4;
                    bool result = ((ctx.Skill.Rune == 3 && PublicClassPlugin.GetBuffLeftTime(hud, 243141, 5) <= HighestElementLeftTime - 1) || //������ȡ
            (ctx.Skill.Rune == 4 && PublicClassPlugin.GetBuffLeftTime(hud, 243141, 8) <= HighestElementLeftTime)//�������
                ) && hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.ConventionOfElements.Sno);

                return result;
            }).ThenContinueElseNoCast()//������ȡ��������+Ԫ�ؽ�ָ
                .IfTrue(ctx =>
                {
                    int CoeIndex = Hud.GetPlugin<PublicClassPlugin>().CoeIndex;
                    bool IsElementReady = PublicClassPlugin.IsElementReady(hud, ctx.Skill.Rune == 4 ? 2 : 2, ctx.Skill.Player, CoeIndex);//������ȡ�;�����ȱ���ǰ�ͱ����ڼ䶼����ʹ��
                    return IsElementReady;
                }).ThenContinueElseNoCast()
                .IfEnoughMonstersNearbyCursor(ctx => 15, ctx => 1).ThenCastElseContinue()
                ;
        }
    }
}