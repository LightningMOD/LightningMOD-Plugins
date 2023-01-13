namespace Turbo.Plugins.LightningMod
{
    using System.Linq;
    public class WitchDoctorSoulHarvestJadeHarvesterPlugin : AbstractSkillHandler, ISkillHandler
    {
        public int ActivationRange { get; set; }

        public WitchDoctorSoulHarvestJadeHarvesterPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.PreAttack, CastPhase.Attack, CastPhase.Move)
        {
            Enabled = false;
            ActivationRange = 18;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.WitchDoctor_SoulHarvest;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 18, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    return (Hud.Game.Me.GetSetItemCount(842970) >= 6);//���6��
                }
                ).ThenContinueElseNoCast()
                .IfTrue(ctx =>//���н��ֶ�û����BUFFʱ���ȿ��ǲ���
                {
                    int Stacks = ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.SacredHarvester.Sno) == true ? 10 : 5;//�ո
                    return (
                    !ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.WitchDoctor_SoulHarvest.Sno, 5) &&//û��6���׼���BUFF
                    ctx.Skill.Player.Density.GetDensity(18) >= Stacks//����������
                    )
                    ;
                }).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    var monsters = Hud.Game.AliveMonsters.Where(m => ((m.SummonerAcdDynamicId == 0 && m.IsElite) || !m.IsElite) && m.FloorCoordinate.XYDistanceTo(Hud.Game.Me.FloorCoordinate) <= 18);//18���ڳ����������
                    int Count = 0;
                    bool RingOfEmptiness = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.RingOfEmptiness.Sno, 0);//����֮��
                    foreach (var monster in monsters)
                    {
                        if(RingOfEmptiness)
                        {
                            if (monster.Haunted || monster.Locust) Count++;//��Ⱥ��ʴ��������
                        }
                       else
                        {
                            if (monster.Haunted) Count++;//��Ⱥ��ʴ��������
                        }
                    }
                    return (Count >= 1 && ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.WitchDoctor_SoulHarvest.Sno, 5));
                }
                ).ThenCastElseContinue()
                ;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 18, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    return (Hud.Game.Me.GetSetItemCount(842970) >= 6 && ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.WitchDoctor_SoulHarvest.Sno, 0));//���6�������ո�BUFF
                }
                ).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.WitchDoctor_SoulHarvest, 5, 300, 500).ThenCastElseContinue()//���ȿ��Ǽ���buff
                ;

        }
    }
}