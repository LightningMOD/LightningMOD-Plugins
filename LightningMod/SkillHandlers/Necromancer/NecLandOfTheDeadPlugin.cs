namespace Turbo.Plugins.LightningMod
{
    public class NecLandOfTheDeadPlugin : AbstractSkillHandler, ISkillHandler
    {
        public float secMin { get; set; }
        public float secMax { get; set; }
        public NecLandOfTheDeadPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
            secMin = 0;
            secMax = 1;
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_LandOfTheDead;
            CreateCastRule()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.Necromancer_LandOfTheDead).ThenNoCastElseContinue()
                .IfSpecificBuffIsActive(hud.Sno.SnoPowers.HauntedVisions).ThenNoCastElseContinue()//��������
                .IfTrue(ctx =>
                {
                    var buff = hud.Game.Me.Powers.GetBuff(hud.Sno.SnoPowers.Necromancer_Simulacrum.Sno);

                    return buff?.TimeElapsedSeconds[1] > secMin && buff?.TimeElapsedSeconds[1] < secMax;
                }).ThenCastElseContinue()
                ;

            CreateCastRule()//�޵������淨
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()//100�����޹ֲ���
                .IfTrue(ctx =>
                {
                    bool isSetPestilence = hud.Game.Me.GetSetItemCount(740282) >= 6;//����6����
                    bool isSetCaptainCrimson = hud.Game.Me.GetSetItemCount(707760) >= 3; //����3����
                    return isSetPestilence && isSetCaptainCrimson && (hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.Necromancer_Passive_BloodIsPower.Sno) || hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.MesserschmidtsReaver.Sno));//����+����+��Ѫ֮����÷��
                }).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100, 500).ThenCastElseContinue()
                ;
            CreateCastRule()//��������
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfEnoughMonstersNearby(ctx => 100, ctx => 1).ThenContinueElseNoCast()//100�����޹ֲ���
                .IfTrue(ctx =>
                {
                    return hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.MesserschmidtsReaver.Sno);//÷��
                }).ThenContinueElseNoCast()
                .IfBuffIsAboutToExpire(100, 500).ThenCastElseContinue()
                ;
        }
    }
}