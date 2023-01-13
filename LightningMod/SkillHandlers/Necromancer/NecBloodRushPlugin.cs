using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class NecBloodRushPlugin : AbstractSkillHandler, ISkillHandler
    {
        public int SesourcesBelow { get; set; }
        public NecBloodRushPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
            SesourcesBelow = 80;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_BloodRush;
            Rune = 4;
            CreateCastRule()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx =>
                {
                    return ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.RequiemCereplate.Sno) && //安魂胸甲
                    ctx.Skill.Player.Powers.CurrentSkills.Any(s => s.SnoPower.Sno == hud.Sno.SnoPowers.Necromancer_Devour.Sno && s.Rune == 3) &&//吞噬 - 噬血灵气
                    ctx.Skill.Player.Powers.CurrentSkills.Any(s => s.SnoPower.Sno == hud.Sno.SnoPowers.Necromancer_BoneSpear.Sno)//骨矛
                    ;
                }).ThenContinueElseNoCast()
                .IfPrimaryResourcePercentageIsBelow(SesourcesBelow).ThenContinueElseNoCast()//能量低于特定百分比时有效
                .IfBossIsNearby(ctx => 50).ThenContinueElseNoCast()//BOSS在50码范围内
                .IfTrue(ctx=> 
                {
                    IWorldCoordinate cursor = Hud.Window.CreateScreenCoordinate(Hud.Window.CursorX, Hud.Window.CursorY).ToWorldCoordinate();
                    return cursor.XYZDistanceTo(hud.Game.Me.FloorCoordinate) < 15;//鼠标在噬血灵气范围内
                }
                ).ThenCastElseNoCast()
                ;
            //(当前精魂值 + (11 * (1 + 安魂特效增幅)) - 200(噬血灵气固定200精魂上限)) / 6 = 每0.5秒获得的精魂，持续3秒共计6次
        }
    }
}