namespace Turbo.Plugins.LightningMod
{
    using System.Linq;
    public class BarbarianThreateningShoutPlugin : AbstractSkillHandler, ISkillHandler
	{
        public int ActivationRange { get; set; }
        public int DensityLimit { get; set; }

        public BarbarianThreateningShoutPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Move, CastPhase.PreAttack)
        {
            Enabled = false;
            ActivationRange = 15;
            DensityLimit = 10;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Barbarian_ThreateningShout;

            CreateCastRule()
                .IfTrue(ctx => ctx.Skill.Rune == 2).ThenNoCastElseContinue()//非恐怖收割符文
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfEliteOrBossIsNearby(ctx => ActivationRange).ThenCastElseContinue()
                .IfEnoughMonstersNearby(ctx => ActivationRange, ctx => ctx.Skill.Player.Powers.BuffIsActive(402458, 1) ? 1 : DensityLimit).ThenCastElseContinue();

            CreateCastRule()
                .IfTrue(ctx => ctx.Skill.Rune == 2).ThenContinueElseNoCast()//恐怖收割符文
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfCanCastSimple().ThenContinueElseNoCast()
                .IfBossIsNearby(ctx => ActivationRange).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    int playerConut = hud.Game.Players.Count();
                    bool playIsNearby = playerConut == 1 ? true : hud.Game.Players.Where(p => p.HasValidActor && p.CentralXyDistanceToMe < ActivationRange && !p.IsMe).Count() >= (playerConut - (playerConut > 2 ? 2 : 1));//靠近任意（最大玩家数-1）的玩家25码
                    var monsterWithHighestMonsterDensity = Hud.Game.AliveMonsters.Where(x => x.IsOnScreen).OrderByDescending(x => x.GetMonsterDensity(ActivationRange)).FirstOrDefault();
                    return (playIsNearby && monsterWithHighestMonsterDensity != null && monsterWithHighestMonsterDensity.CentralXyDistanceToMe <= ActivationRange && (monsterWithHighestMonsterDensity.GetMonsterDensity(25) + 1 >= DensityLimit || hud.Game.ActorQuery.IsEliteOrBossCloserThan(25)));//靠近密度最高怪物25码
                }).ThenCastElseContinue();
        }
    }
}