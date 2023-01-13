using System.Linq;
using Turbo.Plugins.glq;
namespace Turbo.Plugins.LightningMod
{
    public class NecBoneArmorPlugin : AbstractSkillHandler, ISkillHandler
    {
        public int SpareResource { get; set; }
        public NecBoneArmorPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_BoneArmor;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Rune == 1 || (ctx.Skill.Rune == 2 && hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.ConventionOfElements.Sno) && hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.KrysbinsSentence.Sno))).ThenNoCastElseContinue()//�������߷��Ļ��ߣ��׹��Ѿ�+Ԫ�ؽ�+����˹����ʱ��ʹ�øù���
                .IfTrue(ctx =>
                {
                    var monsters = ctx.Hud.Game.AliveMonsters.Where(m => !m.Invulnerable && !m.Invisible && (m.CentralXyDistanceToMe < 30 + m.RadiusBottom));
                    return monsters.Count() >= (ctx.Hud.Game.Me.Powers.BuffIsActive(476686, 0) ? 2 : 1) || hud.Game.ActorQuery.IsEliteOrBossCloserThan(30, false);//������Χ�������Ҫ�������
                })
                .ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Defense.HealthPct < 50 && ctx.Skill.Rune == 3).ThenNoCastElseContinue()//��������������50%Ѫ���²�ʹ��
                .IfTrue(ctx => ctx.Skill.Player.GetSetItemCount(740281) >= 2 && ctx.Skill.Rune != 3).ThenCastElseContinue()//ʥ��ʱֱ��ʩ�ţ���Ϊ������������
                .IfTrue(ctx => (ctx.Skill.Player.Powers.UsedNecromancerPowers.SkeletalMage?.Rune == 1 || ctx.Hud.Game.Me.Powers.BuffIsActive(484311)) && ctx.Skill.Player.Powers.BuffIsActive(476586) && ctx.Skill.Rune != 3).ThenCastElseContinue()//�귨+�ֻ�����ʱֱ��ʩ�ţ��Ҳ�Ϊ������������
                .IfTrue(ctx =>
                {
                    var buff = Hud.Game.Me.Powers.GetBuff(Hud.Sno.SnoPowers.Necromancer_BoneArmor.Sno);
                    return buff?.IconCounts[0] < (ctx.Hud.Game.Me.Powers.BuffIsActive(476686, 0) ? 15 : 10);
                }).ThenCastElseContinue()//��������ʱʩ��
                .IfBuffIsAboutToExpire(10000,15000).ThenCastElseContinue()//BUFFʣ��10~15������ʱʩ��
                ;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx =>  ctx.Skill.Rune == 2).ThenContinueElseNoCast()//�ù�����ް׹��Ѿʷ���
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.ConventionOfElements).ThenContinueElseNoCast()//��Ԫ�ؽ�ָ
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.KrysbinsSentence).ThenContinueElseNoCast()//������˹��
                .IfTrue(ctx =>//��α�֤�Ǽ׼��˲���
                {
                    var monsters = ctx.Hud.Game.AliveMonsters.Where(m => !m.Invulnerable && !m.Invisible && m.CentralXyDistanceToMe < 30);
                    return monsters.Count() >= (ctx.Hud.Game.Me.Powers.BuffIsActive(476686, 0) ? 2 : 1) || hud.Game.ActorQuery.IsEliteOrBossCloserThan(30, false);//������Χ�������Ҫ�������
                })
                .ThenContinueElseNoCast()
                .IfTrue(ctx =>//�����ڱ���Ԫ��ʹ��
                {
                    int CoeIndex = Hud.GetPlugin<PublicClassPlugin>().CoeIndex;
                    var CoeBuffLeftTime = PublicClassPlugin.GetBuffLeftTime(hud, hud.Sno.SnoPowers.ConventionOfElements.Sno, PublicClassPlugin.GetHighestElement(Hud, ctx.Skill.Player, CoeIndex));//��ȡ����Ԫ��ʣ��BUFFʱ��
                    return CoeBuffLeftTime > 0 && CoeBuffLeftTime < 3;
                }).ThenCastElseContinue()//Ԫ��ʱ�ͷ�
                .IfTrue(ctx =>
                {
                    var buff = Hud.Game.Me.Powers.GetBuff(Hud.Sno.SnoPowers.Necromancer_BoneArmor.Sno);
                    return buff?.IconCounts[0] < (ctx.Hud.Game.Me.Powers.BuffIsActive(476686, 0) ? 15 : 10);
                }).ThenCastElseContinue()//��������ʱʩ��
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(476586) && ctx.Skill.Rune != 3).ThenCastElseContinue()//�ֻ�����ʱֱ��ʩ�ţ���Ϊ������������
                .IfBuffIsAboutToExpire(1000, 1500).ThenCastElseContinue()//BUFFʣ�༴������ʱʩ��
                ;

            CreateCastRule()//���ӹ������װ������ʯ��û��װ���ܷ���ʯ��60%CDR���ϣ������30�����й־��ͷŹǼס�
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Player.Stats.CooldownReduction >= 0.6).ThenContinueElseNoCast()
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.GogokOfSwiftnessPrimary).ThenContinueElseNoCast()
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.BaneOfTheStrickenPrimary).ThenNoCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 30, ctx => 1).ThenCastElseContinue()
                ;
        }
    }
}