using System.Linq;

namespace Turbo.Plugins.LightningMod
{
    public class BarbarianWarCryPlugin : AbstractSkillHandler, ISkillHandler
	{
        public int CastBelowFuryPercentage { get; set; }

        public BarbarianWarCryPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.UseTpStart, CastPhase.UseWpStart, CastPhase.Move, CastPhase.PreAttack, CastPhase.Attack)
        {
            Enabled = false;
            CastBelowFuryPercentage = 50;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Barbarian_WarCry;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(500, 1000).ThenCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.ChilaniksChain).ThenContinueElseNoCast()//��������
                .IfPrimaryResourcePercentageIsBelow(CastBelowFuryPercentage).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    return ctx.Skill.Player.GetSetItemCount(786990) < 6 && ctx.Hud.Game.Players.All(p => !p.IsDead && p.HasValidActor && p.CentralXyDistanceToMe <= 100 && !p.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_ActorGhostedBuff.Sno)) && hud.Game.ActorQuery.NearestBoss == null;//���Ѷ���100�����Ҳ������״̬,BOSS���ڸ���ʱ����Ч
                }
                ).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.Ingeom.Sno) && (ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ObsidianRingOfTheZodiac.Sno) || ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.MesserschmidtsReaver.Sno))).ThenContinueElseNoCast()//����+�Ƶ���÷��
                .IfSpecificBuffIsAboutToExpireOnParty(Hud.Sno.SnoPowers.ChilaniksChain, 1, 8000, 8000, HeroClass.All, 100).ThenCastElseContinue()//���������������BUFF����8��ʱʩ��
                ;

            CreateCastRule()//������ʱ�������ּ���BUFF
                .IfInTown().ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.GetSetItemCount(786990) >= 6).ThenContinueElseNoCast()//������
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.ChilaniksChain).ThenContinueElseNoCast()//��������
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.ChilaniksChain, 1, 300, 500).ThenCastElseContinue()
                ;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Rune == 0).ThenContinueElseNoCast()//ŭ��������ļ��������ж�
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpireOnParty(Hud.Sno.SnoPowers.Barbarian_WarCry, 1, 200, 500, HeroClass.All, 100).ThenCastElseContinue()
                ;
        }
    }
}