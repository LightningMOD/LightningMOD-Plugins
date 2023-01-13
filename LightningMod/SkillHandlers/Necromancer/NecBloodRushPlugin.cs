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
                    return ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.RequiemCereplate.Sno) && //�����ؼ�
                    ctx.Skill.Player.Powers.CurrentSkills.Any(s => s.SnoPower.Sno == hud.Sno.SnoPowers.Necromancer_Devour.Sno && s.Rune == 3) &&//���� - ��Ѫ����
                    ctx.Skill.Player.Powers.CurrentSkills.Any(s => s.SnoPower.Sno == hud.Sno.SnoPowers.Necromancer_BoneSpear.Sno)//��ì
                    ;
                }).ThenContinueElseNoCast()
                .IfPrimaryResourcePercentageIsBelow(SesourcesBelow).ThenContinueElseNoCast()//���������ض��ٷֱ�ʱ��Ч
                .IfBossIsNearby(ctx => 50).ThenContinueElseNoCast()//BOSS��50�뷶Χ��
                .IfTrue(ctx=> 
                {
                    IWorldCoordinate cursor = Hud.Window.CreateScreenCoordinate(Hud.Window.CursorX, Hud.Window.CursorY).ToWorldCoordinate();
                    return cursor.XYZDistanceTo(hud.Game.Me.FloorCoordinate) < 15;//�������Ѫ������Χ��
                }
                ).ThenCastElseNoCast()
                ;
            //(��ǰ����ֵ + (11 * (1 + ������Ч����)) - 200(��Ѫ�����̶�200��������)) / 6 = ÿ0.5���õľ��꣬����3�빲��6��
        }
    }
}