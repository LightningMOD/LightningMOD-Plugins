using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class NecCommandSkeletonsPlugin : AbstractSkillHandler, ISkillHandler
    {
        private bool isCast = false;
        public NecCommandSkeletonsPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = true;
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_CommandSkeletons;
            CreateCastRule()
                .IfTrue(ctx =>
                {//�����ò�����������
                    return ctx.Skill.Rune == 3;
                }
                ).ThenNoCastElseContinue()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfPrimaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {//ѡ��BOSSʱ
                    var monster = Hud.Game.SelectedMonster2;
                    if (monster == null) return false;
                    return monster?.Rarity == ActorRarity.Boss;
                }).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {//�������ʱ�򼤻�һ��
                    return !Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Necromancer_CommandSkeletons.Sno);
                }).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    bool cast = false;
                    if (Hud.Game.Me.Powers.UsedLegendaryPowers.BloodsongMail?.Active == true)//Ѫ������
                    {
                        if (Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Necromancer_LandOfTheDead.Sno))//��������
                        {
                            if (!isCast)
                            {
                                isCast = true;
                                cast = true;
                            }
                        }
                        else
                        {
                            isCast = false;
                        }
                    }
                    else
                    {
                        isCast = false;
                    }
                    return cast;
                }).ThenCastElseContinue()
                ;


            CreateCastRule()
                .IfTrue(ctx =>
                {//�����ò�����������
                    return ctx.Skill.Rune == 3;
                }
                ).ThenNoCastElseContinue()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfPrimaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Necromancer_CommandSkeletons.Sno)).ThenNoCastElseContinue()//����ʱ������
                .IfTrue(ctx =>
                {//ѡ�������ʱ
                    var monster = Hud.Game.SelectedMonster2;
                    if (monster == null)
                    {
                        return false;
                    }
                    else
                    {
                        bool JessethArms = ctx.Skill.Player.Powers.BuffIsActive(476047);//������
                        if (JessethArms)//���� = ����һ��
                        {
                            return true;
                        }
                    }
                    return false;
                }).ThenCastElseContinue()
                ;


            CreateCastRule()
                .IfTrue(ctx =>
                {//�����ò�����������
                    return ctx.Skill.Rune == 3;
                }
                ).ThenNoCastElseContinue()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfPrimaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {//ѡ�������ʱ
                    var monster = Hud.Game.SelectedMonster2;
                    if (monster == null)
                    {
                        return false;
                    }
                    else
                    {
                        bool ObsidianRingoftheZodiac = ctx.Skill.Player.Powers.BuffIsActive(402459);//�Ƶ�
                        if (ctx.Skill.GetResourceRequirement() <= 25 && ObsidianRingoftheZodiac)//���ĵ���25+����+�Ƶ� = ����ʩ��
                        {
                            return true;
                        }
                    }
                    return false;
                }).ThenCastElseContinue()
                ;

            CreateCastRule()
               .IfTrue(ctx =>
               {//�����ò�����������
                    return ctx.Skill.Rune == 3;
               }
               ).ThenNoCastElseContinue()
               .IfCanCastSkill(2000, 2000, 2000).ThenContinueElseNoCast()
               .IfInTown().ThenNoCastElseContinue()
               .IfCastingIdentify().ThenNoCastElseContinue()
               .IfCastingPortal().ThenNoCastElseContinue()
               .IfPrimaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
               //.IfSpecificBuffIsActive(hud.Sno.SnoPowers.BondsOfCLena).ThenContinueElseNoCast()//�����ɵ�����
               .IfTrue(ctx =>
               {//ѡ�о�Ӣ�ֻ�BOSSʱ
                    var monster = Hud.Game.SelectedMonster2;
                   if (monster == null)
                   {
                       return false;
                   }
                   var CommandSkeletonsTarget = Hud.Game.Actors.Where(x => x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_None, 453801) == 1 || x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_A, 453801) == 1 || x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_B, 453801) == 1 || x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_C, 453801) == 1 || x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_D, 453801) == 1 || x.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_4_Visual_Effect_E, 453801) == 1).FirstOrDefault();
                   bool isCommandSkeletonsTargetOnScreen = CommandSkeletonsTarget?.IsOnScreen == true;
                   var ArmyoftheDead = hud.Game.Me.Powers.GetUsedSkill(hud.Sno.SnoPowers.Necromancer_ArmyOfTheDead);
                   bool AttackAnyMonster = ArmyoftheDead?.IsOnCooldown == true && isCommandSkeletonsTargetOnScreen;
                   return (monster.Rarity == ActorRarity.Boss || monster.Rarity == ActorRarity.Champion || monster.Rarity == ActorRarity.Rare || AttackAnyMonster) && !monster.Illusion;
               }).ThenCastElseContinue()
               ;

        }
    }
}