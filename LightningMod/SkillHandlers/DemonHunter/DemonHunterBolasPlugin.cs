namespace Turbo.Plugins.LightningMod
{
    public class DemonHunterBolasPlugin : AbstractSkillHandler, ISkillHandler
    {
        public DemonHunterBolasPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.Attack)
        {
            Enabled = false;
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);
            
            AssignedSnoPower = Hud.Sno.SnoPowers.DemonHunter_Bolas;
            //���ֶԽ�
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue() 
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => ctx.Skill.Player.Powers.BuffIsActive(359583, 0)).ThenContinueElseNoCast()//װ�����Ŀ˼�
                .IfEnoughMonstersNearbyCursor(ctx => 15, ctx => 1).ThenContinueElseNoCast()
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.Generic_ItemPassiveUniqueRing735x1, 1, 30, 100).ThenCastElseContinue()
                ;
            //������������
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx => Hud.Game.Me.Powers.UsedLegendaryPowers.WrapsOfClarity?.Active == true).ThenContinueElseNoCast()//װ����������
                .IfSpecificBuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInvulnerable, 0).ThenNoCastElseContinue()//����
                .IfSpecificBuffIsAboutToExpire(hud.Sno.SnoPowers.WrapsOfClarity, 1, 30, 300).ThenCastElseContinue()
                ;
        }
    }
}