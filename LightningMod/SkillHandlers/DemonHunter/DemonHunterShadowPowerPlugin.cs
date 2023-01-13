using System;
namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterShadowPowerPlugin : AbstractSkillHandler, ISkillHandler
    {
        public bool ElusiveRingEnabled { get; set; } = true;
        public bool LifeOnHitEnabled { get; set; } = true;
        public DemonHunterShadowPowerPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.PreAttack)
        {
            Enabled = false;
            ElusiveRingEnabled = false;
            LifeOnHitEnabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_ShadowPower;

            // normal һ�����Σ��ʱ�Զ�ʩ��
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.DemonHunter_ShadowPower.Sno)).ThenNoCastElseContinue()
                .IfTrue(ctx => Hud.Game.Me.GetSetItemCount(916931) >= 4).ThenCastElseContinue()//��Ӱ4����
                .IfFalse(ctx =>
                {
                    var lotsOfDiscipline = (ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null) &&
                                            (ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation.Rune != 0) &&
                                            !ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation.IsOnCooldown; // not for Punishment

                    return (lotsOfDiscipline && (ctx.Skill.Player.Defense.HealthPct <= 65)) || LifeInWarning(ctx.Skill.Player);
                }).ThenNoCastElseContinue()
                .IfBuffIsAboutToExpire(30, 100).ThenCastElseContinue();

            //��Ӱ����+�־�6����+���ƴ���+�ƶ�ʱʩ��
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.DemonHunter_ShadowPower.Sno)).ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Rune == 1 && (ctx.Skill.Player.GetSetItemCount(791249) >= 6 || ctx.Skill.Player.GetSetItemCount(254164) >= 6) && ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null).ThenContinueElseNoCast()//��Ӱ����+�־�6���׻�а��֮��6����+���ƴ���
                .IfRunning(true).ThenCastElseContinue()
                ;

            //���밵Ӱ+�־�6����+���ƴ���+80����1����ʱʩ��
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(Hud.Sno.SnoPowers.DemonHunter_ShadowPower.Sno)).ThenNoCastElseContinue()
                .IfTrue(ctx => ctx.Skill.Rune == 2 && (ctx.Skill.Player.GetSetItemCount(791249) >= 6 || ctx.Skill.Player.GetSetItemCount(254164) >= 6) && ctx.Skill.Player.Powers.UsedDemonHunterPowers.Preparation != null).ThenContinueElseNoCast()//���밵Ӱ+�־�6���׻�а��֮��6����+���ƴ���
                .IfEnoughMonstersNearby(ctx => 80, ctx => 1).ThenCastElseContinue()//80����1����
                ;

            // elusive ring ��Ӱ��ָ
            CreateCastRule()
                .IfTrue(ctx => ElusiveRingEnabled && ctx.Skill.Player.Powers.UsedLegendaryPowers.ElusiveRing?.Active == true).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenContinueElseNoCast()
                .IfEnoughMonstersNearby(ctx => 40, ctx => 1).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.ElusiveRing, 1, 30, 100).ThenCastElseContinue();

            //Bottomless Potion of Mutilation ��ɱ��ҩˮ
            CreateCastRule()
                .IfTrue(ctx =>
                {
                    var buff = Hud.Game.Me.Powers.GetBuff(hud.Sno.SnoPowers.Generic_X1LegendaryGenericPotionPowerup.Sno);
                    return (LifeOnHitEnabled && buff != null && buff.TimeLeftSeconds[4]>=4.6);
                }
                ).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfSecondaryResourceIsEnough(0, ctx => 0).ThenCastElseContinue()
            ;
        }
        private bool LifeInWarning(IPlayer player)
        {
            return player.Defense.HealthPct <= 30 || (player.Powers.HealthPotionSkill.IsOnCooldown && (player.Defense.HealthPct <= 85))
                || (Hud.Game.IsEliteOnScreen && (player.Defense.HealthPct <= 75 || player.AvoidablesInRange.Count > 1));
        }
    }
}