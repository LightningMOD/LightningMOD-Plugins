using System.Linq;
using Turbo.Plugins.glq;
namespace Turbo.Plugins.LightningMod
{
    public class BarbarianWrathOfTheBerserkerPlugin : AbstractSkillHandler, ISkillHandler
    {
        public BarbarianWrathOfTheBerserkerPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.UseTpStart, CastPhase.UseWpStart, CastPhase.Move, CastPhase.PreAttack)
        {
            Enabled = false;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Barbarian_WrathOfTheBerserker;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                /*.IfTrue(ctx => {
                    var buff = ctx.Skill.Player.Powers.GetBuff(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting.Sno);//��ȡ������
                    return buff != null && buff.TimeLeftSeconds[0] > 0 && buff.TimeLeftSeconds[0] < 2;//��ȡ������ʣ��ʱ��С��2��
                }
                ).ThenCastElseContinue()//��������������ǰʩ��*/
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0, 500, 2000, true).ThenCastElseContinue()
                .IfBuffIsAboutToExpire(300, 500).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    int CoeIndex = Hud.GetPlugin<PublicClassPlugin>().CoeIndex;
                    bool isCOE = ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.ConventionOfElements.Sno);
                    double HighestElementLeft = PublicClassPlugin.GetHighestElementLeftSecond(hud, ctx.Skill.Player, CoeIndex);
                    var IKset = Hud.Game.Me.GetSetItemCount(671068) >= 4;//����4����
                    return (IKset) || (Hud.Game.Me.Powers.UsedPassives.Any(p => p.Sno == Hud.Sno.SnoPowers.Barbarian_Passive_BoonOfBulKathos.Sno) && ctx.Skill.Player.Stats.CooldownReduction >= 0.75) //���������Ķ��󱻶� ȥ CDR����
                    || Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ObsidianRingOfTheZodiac.Sno) //�Ƶ�
                    || Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.MesserschmidtsReaver.Sno) //÷��
                    ||(isCOE ? ((HighestElementLeft <= 16 && HighestElementLeft >= 15) || Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Ingeom.Sno)) && Hud.Game.ActorQuery.IsEliteOrBossCloserThan(40, false) : Hud.Game.ActorQuery.IsEliteOrBossCloserThan(40, false)) //������Ӣ��BOSS��װ��Ԫ�ؽ�ָʱֻ�ڱ���ǰ1��ʩ��
                    || (ctx.Skill.Player.Defense.HealthPct <= 30)//Ѫ������30%
                        ;
                }).ThenCastElseContinue()
                ;

        }
    }
}