namespace Turbo.Plugins.LightningMod
{
    using Turbo.Plugins.glq;
    public class CrusaderLawsOfValorPlugin : AbstractSkillHandler, ISkillHandler
	{
        public CrusaderLawsOfValorPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.PreAttack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Crusader_LawsOfValor;

            CreateCastRule()//һ�����
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//����ʱ
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_X1CrusaderLawsOfValorPassive2, 6).ThenNoCastElseContinue()//�Ѽ���
                .IfTrue(ctx =>
                (ctx.Hud.Game.Me.Powers.BuffIsActive(310678) ||//�ɷ��ޱ�
                ctx.Skill.Player.Stats.CooldownReduction >= 0.5 || ctx.Hud.Game.Me.Powers.BuffIsActive(402459)) && //CDR����50����˻Ƶ�
                (!isSanGuang() ||
                (isSanGuang() && (ctx.Hud.Game.RiftPercentage < 100 || !ctx.Skill.Player.InGreaterRift)))//���ؾ����Ƚ׶�
                ).ThenCastElseContinue()
                ;
            CreateCastRule()//����BOSSս����
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Crusader_SteedCharge).ThenNoCastElseContinue()//����ʱ
                .IfTrue(ctx =>
                (isSanGuang() && ctx.Hud.Game.RiftPercentage == 100 && ctx.Hud.Game.ActorQuery.IsEliteOrBossCloserThan(30) && (PublicClassPlugin.IsElementReady (Hud, 0.5, ctx.Skill.Player, 4) || //���ؾ�BOSS�׶Σ���ʥǰ0.5�뼤��
                (ctx.Hud.Game.Me.Powers.BuffIsActive(310678) &&//�ɷ��ޱ�
                ctx.Skill.Player.Stats.CooldownReduction >= 0.6518)))//�޷��ͷŵĻ�������
                ).ThenCastElseContinue()
                ;
        }

        private bool isSanGuang()
        {
            bool isHeavensFury = Hud.Game.Me.Powers.UsedCrusaderPowers.HeavensFury?.Rune == 4; //����֮��
            bool isAegisofValor = Hud.Game.Me.GetSetItemCount(192736) >= 6;//����6����
            bool isFateoftheFell = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.FateOfTheFell.Sno, 0);//��а�ذ�
            bool isConventionOfElements = Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.ConventionOfElements.Sno, 0);//Ԫ�ؽ�ָ
            return isAegisofValor && isHeavensFury && isFateoftheFell && isConventionOfElements;
        }
    }
}