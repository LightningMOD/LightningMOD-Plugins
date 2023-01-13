using Turbo.Plugins.glq;
namespace Turbo.Plugins.LightningMod
{
    public class WizardBlackHolePlugin : AbstractSkillHandler, ISkillHandler
    {
        public int SpareResource { get; set; }

        public WizardBlackHolePlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Wizard_BlackHole;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfTrue(ctx =>
                ctx.Skill.Player.GetSetItemCount(84014) >= 6 && ctx.Skill.Player.Powers.UsedLegendaryPowers.Deathwish?.Active == true).ThenNoCastElseContinue()//火鸟6件时无效
                .IfTrue(ctx =>{
                    double HighestElementLeftTime = glq.PublicClassPlugin.GetBuffLeftTime(Hud, Hud.Sno.SnoPowers.ConventionOfElements.Sno, glq.PublicClassPlugin.GetHighestElement(Hud, Hud.Game.Me));
                    if (HighestElementLeftTime == 0) HighestElementLeftTime = 4;
                    bool result = ((ctx.Skill.Rune == 3 && PublicClassPlugin.GetBuffLeftTime(hud, 243141, 5) <= HighestElementLeftTime - 1) || //法术窃取
            (ctx.Skill.Rune == 4 && PublicClassPlugin.GetBuffLeftTime(hud, 243141, 8) <= HighestElementLeftTime)//绝对零度
                ) && hud.Game.Me.Powers.BuffIsActive(hud.Sno.SnoPowers.ConventionOfElements.Sno);

                return result;
            }).ThenContinueElseNoCast()//法术窃取或绝对零度+元素戒指
                .IfTrue(ctx =>
                {
                    int CoeIndex = Hud.GetPlugin<PublicClassPlugin>().CoeIndex;
                    bool IsElementReady = PublicClassPlugin.IsElementReady(hud, ctx.Skill.Rune == 4 ? 2 : 2, ctx.Skill.Player, CoeIndex);//法术窃取和绝对零度爆发前和爆发期间都可以使用
                    return IsElementReady;
                }).ThenContinueElseNoCast()
                .IfEnoughMonstersNearbyCursor(ctx => 15, ctx => 1).ThenCastElseContinue()
                ;
        }
    }
}