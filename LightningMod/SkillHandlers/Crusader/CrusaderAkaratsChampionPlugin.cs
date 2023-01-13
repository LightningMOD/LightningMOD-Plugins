using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class CrusaderAkaratsChampionPlugin : AbstractSkillHandler, ISkillHandler
	{
        public CrusaderAkaratsChampionPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.Collect, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Crusader_AkaratsChampion;

            CreateCastRule()//һ�����
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => !isSanGuang()).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting, 0, 500, 2000, true).ThenCastElseContinue()//��������������ǰʩ��
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    var set = Hud.Game.Me.GetSetItemCount(580748);// ���˺���
                    return !ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno) && (ctx.Skill.Player.Stats.CooldownReduction >= 0.75 || (ctx.Skill.Player.Stats.CooldownReduction >= 0.5 && (set >= 4 || ctx.Hud.Game.Me.Powers.BuffIsActive(402459) || ctx.Hud.Game.Me.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.AkaratsAwakening.Sno))));//CDR����75��50�Ҵ��˻Ƶ��򰢿����ض���򰢿˺�4����
                }).ThenCastElseContinue()
                .IfBuffIsAboutToExpire(100, 200).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    var Crusader_SteedCharge = ctx.Skill.Player.Powers.GetUsedSkill(Hud.Sno.SnoPowers.Crusader_SteedCharge);
                    bool isNoFatal = (!ctx.Skill.Player.Powers.BuffIsActive(hud.Sno.SnoPowers.Crusader_Passive_Indestructible.Sno, 0) || ctx.Skill.Player.Powers.BuffIsActive(hud.Sno.SnoPowers.Crusader_Passive_Indestructible.Sno, 1)) &&//û���������Ļ�CD��
                    ctx.Skill.Rune == 3 &&//��֪����
                    (ctx.Skill.Player.HeroIsHardcore || ctx.Hud.Avoidance.CurrentValue || ctx.Hud.Game.ActorQuery.IsEliteOrBossCloserThan(20, false) || ctx.Skill.Player.AvoidablesInRange.Any(x => x.AvoidableDefinition.InstantDeath) || ctx.Skill.Player.Powers.CantMove || (Hud.Game.Me.Defense.HealthPct < (Hud.Game.Me.Powers.HealthPotionSkill.IsOnCooldown ? 60 : 30))) &&//ר��ģʽ��Σ��ʱ
                    (!ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno) || (ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno) && cando()))
                    ;
                    
                    return isNoFatal ? true : !ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno);
                }).ThenContinueElseNoCast()//���ȱ���
                .IfPrimaryResourcePercentageIsBelow(20).ThenCastElseContinue()
                .IfEliteOrBossIsNearby(ctx => 40).ThenCastElseContinue()
                ;

            CreateCastRule()//�������
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => isSanGuang()).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 30, ctx => 1).ThenContinueElseNoCast()
                .IfTrue(ctx =>
                {
                    return ctx.Skill.Player.Powers.BuffIsActive(hud.Sno.SnoPowers.ConventionOfElements.Sno, 4) && (!ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno) || (ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge.Sno) && cando()));
                }).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100, 200).ThenCastElseContinue()
                ;
        }
        private bool cando()
        {
            var Crusader_SteedCharge = Hud.Game.Me.Powers.GetUsedSkill(Hud.Sno.SnoPowers.Crusader_SteedCharge);
            return ((!Hud.Interaction.IsHotKeySet(ActionKey.Move) || (Hud.Interaction.IsHotKeySet(ActionKey.Move) && !Hud.Interaction.IsContinuousActionStarted(ActionKey.Move))) && (Crusader_SteedCharge != null && !Hud.Interaction.IsContinuousActionStarted(Crusader_SteedCharge.Key)));//δ����ǿ���ƶ���δ��ס����
        }

        private bool isSanGuang()
        {
            bool isAegisofValor = Hud.Game.Me.GetSetItemCount(192736) >= 6;//����6����
            bool isFateoftheFell = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.FateOfTheFell.Sno, 0);//��а�ذ�
            bool isConventionOfElements = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ConventionOfElements.Sno, 0);//Ԫ�ؽ�ָ
            return isAegisofValor && isFateoftheFell && isConventionOfElements;
        }
    }
}