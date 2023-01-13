namespace Turbo.Plugins.LightningMod
{
    using System.Linq;
    public class BarbarianSprintPlugin : AbstractSkillHandler, ISkillHandler
    {
        public BarbarianSprintPlugin()
            : base(CastType.BuffSkill,  CastPhase.AutoCast, CastPhase.Move, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Barbarian_Sprint;

            CreateCastRule()//��������
                .IfTrue(ctx => ctx.Skill.Rune != 3).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfCanCastSkill(300, 400, 500).ThenContinueElseNoCast()
                .IfPrimaryResourceIsEnough(40, ctx => 20).ThenContinueElseNoCast()
                .IfRunning(true).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100,300).ThenCastElseContinue()//BUFF������ʧʱ�Զ�����BUFF
                ;

            CreateCastRule()//���о�����
                .IfTrue(ctx => ctx.Skill.Rune == 3).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfCanCastSkill(300, 400, 500).ThenContinueElseNoCast()
                .IfPrimaryResourceIsEnough(40, ctx => 0).ThenContinueElseNoCast()
                .IfBossIsNearby(ctx => 50).ThenNoCastElseContinue()//BOSS�ڸ���ʱ��ʩ��
                .IfTrue(ctx =>
                {
                    var players = ctx.Hud.Game.Players.Where(p => !p.IsDead && p.SnoArea.Sno == hud.Game.Me.SnoArea.Sno && p.CentralXyDistanceToMe <= 200);
                    return players.All(p => p.CentralXyDistanceToMe <= 50 && !p.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_ActorGhostedBuff.Sno));//���Ѷ���50����
                }).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(500, 1000).ThenCastElseContinue()//BUFF������ʧʱ�Զ�����BUFF
                .IfPrimaryResourceIsEnough(70, ctx => 0).ThenContinueElseNoCast()//70%ŭ������
                .IfSpecificBuffIsAboutToExpireOnParty(Hud.Sno.SnoPowers.Barbarian_Sprint, 0, 2000, 2000, HeroClass.All, 50).ThenCastElseContinue()//������Ѽ���BUFF����2��ʱʩ��
                ;
        }
    }
}