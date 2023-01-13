namespace Turbo.Plugins.LightningMod
{
    using System.Linq;

    public class BarbarianIgnorePainPlugin : AbstractSkillHandler, ISkillHandler
	{
        private IPlayer DPS = null;
        private IPlayer shawang = null;
        public BarbarianIgnorePainPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Barbarian_IgnorePain;

            CreateCastRule()//ͬ�����
                .IfTrue(ctx => ctx.Skill.Rune == 2).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfHealthWarning(30, 60).ThenCastElseContinue()
                .IfNearbyPartyMemberIsInDanger(48, 30, 60, 40, true).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    var players = ctx.Hud.Game.Players.Where(p => !p.IsDead && p.SnoArea.Sno == hud.Game.Me.SnoArea.Sno && p.CentralXyDistanceToMe <= 200);
                    return ctx.Skill.Rune == 2 && players.All(p => p.CentralXyDistanceToMe <= 50 && !p.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_ActorGhostedBuff.Sno));//���Ѷ���50�����Ҳ������״̬
                }
                ).ThenCastElseContinue()
                .IfSpecificBuffIsActiveOnParty(Hud.Sno.SnoPowers.StoneGauntlets, 2, HeroClass.All, 50).ThenCastElseContinue()//��ʯ����debuff
                .IfTrue(ctx =>
                {
                    if (ctx.Skill.Player.InGreaterRiftRank == 0) return false;//���ڴ��ؾ�ʱ��ִ��
                    var players = ctx.Hud.Game.Players.Where(p => !p.IsMe && !p.IsDead && !p.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_ActorGhostedBuff.Sno) &&//�����Լ�������ŵĶ���
                    p.SnoArea.Sno == hud.Game.Me.SnoArea.Sno && p.CoordinateKnown && //������Чλ��
                    p.Powers.UsedLegendaryGems.EsotericAlterationPrimary?.Active == false && //û�д�תɷ
                    p.Powers.UsedLegendaryGems.GemOfEfficaciousToxinPrimary?.Active == false && //û�д��綾
                    p.Powers.UsedLegendaryPowers.OculusRing?.Active == false);//û�д���Ŀ
                    if (players == null) return false;
                    shawang = players.FirstOrDefault(p => p.Powers.UsedLegendaryGems.BaneOfTheStrickenPrimary?.Active == true);//���ܷ�������ɱ��λ
                    if (shawang == null)
                    {
                        shawang = players.OrderByDescending(p => p.Offense.SheetDps).FirstOrDefault();//û�д��ܷ�����ʱȡ�����ߵ�����ɱ��λ
                    }
                    DPS = players.Where(p => p.Powers.UsedLegendaryGems.BaneOfTheStrickenPrimary?.Active == false).OrderByDescending(p => p.Offense.SheetDps).FirstOrDefault();//�����ܷ���DPS��ߵ����ǽ���λ
                    if (shawang == null)
                    {
                        DPS = players.OrderByDescending(p => p.Offense.SheetDps).FirstOrDefault();//�����ܷ�ʱȡ�����ߵ���Ϊ����λ
                    }
                    bool cast = false;
                    if (Hud.Game.RiftPercentage < 100)
                    {
                        if(DPS != null && DPS.CoordinateKnown && DPS.CentralXyDistanceToMe < 50)//���Ƚ׶ν���λ��50����
                        {
                            cast = true;
                        }
                    }else
                    {
                        if(shawang != null && shawang.CoordinateKnown && shawang.CentralXyDistanceToMe < 50)//ɱ���׶�ɱ��λ��50����
                        {
                            cast = true;
                        }
                    }
                        return ctx.Skill.Rune == 2 && cast == true;//ɱ�������λ��50����ʱʩ��
                }
                ).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.Ingeom.Sno) && ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.PrideOfCassius.Sno) && (ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ObsidianRingOfTheZodiac.Sno) || ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.MesserschmidtsReaver.Sno))).ThenContinueElseNoCast()//����+����˹֮��+�Ƶ���÷��
                .IfSpecificBuffIsAboutToExpireOnParty(Hud.Sno.SnoPowers.Barbarian_IgnorePain, 0, 8000, 8000, HeroClass.All, 50).ThenCastElseContinue()//����������ӿ�ʹBUFF����8��ʱʩ��
                ;

            CreateCastRule()//����������
                .IfTrue(ctx => ctx.Skill.Rune == 3).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100, 500).ThenCastElseContinue()
                ;

            CreateCastRule()//��������
                .IfTrue(ctx => ctx.Skill.Rune != 2).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfHealthWarning(40, 80).ThenCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Rune != 3 && ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInvulnerable.Sno, 0)).ThenNoCastElseContinue()//������������ʱ������������ʹ��
                .IfTrue(ctx =>
                ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ObsidianRingOfTheZodiac.Sno) || ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.MesserschmidtsReaver.Sno) || ctx.Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.PrideOfCassius.Sno)//����˹֮����Ƶ���÷��
                ).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100,500).ThenCastElseContinue()
                ;
        }
    }
}