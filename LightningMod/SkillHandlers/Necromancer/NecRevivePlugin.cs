using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class NecRevivePlugin : AbstractSkillHandler, ISkillHandler
    {
        public NecRevivePlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_Revive;
            //Rune = 4//������
            CreateCastRule()
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfTrue(ctx =>
                {
                    var Revives = hud.Game.Actors.Where(m =>
                    (m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_None, 462239) == 1 ||
                    m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_A, 462239) == 1 ||
                    m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_B, 462239) == 1 ||
                    m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_C, 462239) == 1 ||
                    m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_D, 462239) == 1 ||
                    m.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_E, 462239) == 1) && 
                    m.SummonerAcdDynamicId == ctx.Skill.Player.SummonerId);//��������
                    int sets = ctx.Skill.Player.GetSetItemCount(740279);//��˹����װ
                    return sets >= 2 && Revives.Count() < 10 && isCorpse();
                }).ThenCastElseContinue()
                ;
        }
        private bool isCorpse()
        {
            IWorldCoordinate cur = Hud.Window.CreateScreenCoordinate(Hud.Window.CursorX, Hud.Window.CursorY).ToWorldCoordinate();
            bool isCorpse = Hud.Game.Actors.Any(a => a.SnoActor.Sno == ActorSnoEnum._p6_necro_corpse_flesh && a.FloorCoordinate.XYDistanceTo(cur) <= 20);//���20���ڴ���ʬ��
            return isCorpse;
        }
    }
}