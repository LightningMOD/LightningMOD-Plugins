using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class NecArmyoftheDeadPlugin : AbstractSkillHandler, ISkillHandler
    {
        public NecArmyoftheDeadPlugin()
            : base(CastType.SimpleSkill, CastPhase.AutoCast, CastPhase.UseWpStart, CastPhase.Move, CastPhase.Attack, CastPhase.AttackIdle)
        {
            Enabled = false;
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Necromancer_ArmyOfTheDead;

            CreateCastRule()//��װ��Ԫ�ؽ�ָ�Ĺ���
                .IfCanCastSkill(100, 150, 1000).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx =>
                {
                    int range = 14;
                    bool IncludeMinion = false;
                    IWorldCoordinate cursor = ctx.Hud.Window.CreateScreenCoordinate(ctx.Hud.Window.CursorX, ctx.Hud.Window.CursorY).ToWorldCoordinate();
                    bool Result = ctx.Hud.Game.AliveMonsters.Any(m => ((m.IsElite && (IncludeMinion ? true : m.Rarity != ActorRarity.RareMinion)) || m.SnoMonster.Priority == MonsterPriority.goblin) && m.FloorCoordinate.XYDistanceTo(cursor) <= range && !m.Invisible && !m.Illusion);
                    return Result;
                }).ThenCastElseContinue()
                .IfTrue(ctx =>
                {
                    var MonsterRiftProgression = 0d;
                    IWorldCoordinate cur = Hud.Window.CreateScreenCoordinate(Hud.Window.CursorX, Hud.Window.CursorY).ToWorldCoordinate();
                    var monsters = hud.Game.AliveMonsters.Where( m => m.FloorCoordinate.XYDistanceTo(cur) <= ((ctx.Skill.Rune == 3 && glq.PublicClassPlugin.IsElementReady(hud, 1, ctx.Skill.Player)) ? 30 : 20)) ;
                    foreach (var monster in monsters)
                    {
                        MonsterRiftProgression += monster.SnoMonster.RiftProgression * 100.0d / Hud.Game.MaxQuestProgress;
                    }
                    return MonsterRiftProgression >= 1.3d;
                }
                ).ThenCastElseContinue()
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
                    return sets >= 2 && Revives.Count() < 10;
                }).ThenContinueElseNoCast()
                .IfEnoughMonstersNearbyCursor(ctx => 15, ctx => 4).ThenCastElseContinue()
                .IfEliteOrBossNearbyCursor(ctx => 15, false).ThenCastElseContinue()
                ;
        }

    }
}