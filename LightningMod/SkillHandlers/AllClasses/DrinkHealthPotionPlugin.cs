namespace Turbo.Plugins.LightningMod
{
    using System.Linq;

    public class DrinkHealthPotionPlugin : AbstractSkillHandler, ISkillHandler
    {
        public int SDT { get; set; }
        public int HC { get; set; }
        public int InDanger { get; set; }
        public DrinkHealthPotionPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.Collect)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Generic_DrinkHealthPotion;

            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                //.IfOnCooldown().ThenNoCastElseContinue() //It is ineffective for Potion
                .IfTrue(IsPotionOnCoolDown).ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                //.IfCanCastSkill(50, 50, 50).ThenContinueElseNoCast()
                .IfHealthPercentageIsBelow(ctx =>
                {
                    var limit = SDT;
                    if (ctx.Skill.Player.HeroIsHardcore) limit = HC;
                    if (ctx.Skill.Player.AvoidablesInRange.Any(x => x.AvoidableDefinition.Type == AvoidableType.IceBalls)) limit = InDanger;
                    return limit;
                }).ThenCastElseContinue();
        }

        private bool IsPotionOnCoolDown(TestContext ctx)
        {
            bool IsOnCooldown;
            double Cooldown;
            Cooldown = (Hud.Game.Me.Powers.HealthPotionSkill.CooldownFinishTick - Hud.Game.CurrentGameTick) / 60d;
            IsOnCooldown = Cooldown <= 30 && Cooldown >= 0 ? true : false;
            return IsOnCooldown;
        }
    }
}