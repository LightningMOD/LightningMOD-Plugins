using System.Linq;
namespace Turbo.Plugins.LightningMod
{
    public class MonkMysticAllyPlugin : AbstractSkillHandler, ISkillHandler
	{
        public MonkMysticAllyPlugin()
            : base(CastType.BuffSkill, CastPhase.AutoCast, CastPhase.Attack)
        {
            Enabled = false;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            AssignedSnoPower = Hud.Sno.SnoPowers.Monk_MysticAlly;
               
            CreateCastRule()
                .IfCanCastSkill(150, 200, 500).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfTrue(ctx => (ctx.Skill.Rune == 3 || (ctx.Skill.Rune == 1 && ctx.Skill.Player.GetSetItemCount(742942) >= 6)) && ctx.Skill.Player.Stats.ResourceCurPri < 50).ThenCastElseContinue()//�������3��ˮ����ʱ����6��������������50
                .IfTrue(ctx => ctx.Skill.Rune == 4 && ctx.Skill.Player.Defense.HealthPct < 30).ThenCastElseContinue()//�������4
                ;
            CreateCastRule()
                .IfCanCastSkill(100, 200, 500).ThenContinueElseNoCast()
                .IfInTown().ThenNoCastElseContinue()
                .IfCastingIdentify().ThenNoCastElseContinue()
                .IfCastingPortal().ThenNoCastElseContinue()
                .IfOnCooldown().ThenNoCastElseContinue()
                .IfRunning().ThenNoCastElseContinue()
                .IfIdle().ThenNoCastElseContinue()
                .IfTrue(ctx => {//ˮ�����1���������
                    bool isBossOrEliteNearby = ctx.Hud.Game.AliveMonsters.Any(x => (x.Rarity == ActorRarity.Boss || x.Rarity == ActorRarity.Champion || x.Rarity == ActorRarity.Rare || x.Rarity == ActorRarity.Unique) && x.NormalizedXyDistanceToMe < 20 && !x.Illusion && !x.Invulnerable && !x.Invisible);
                    return (ctx.Skill.Rune == 1 || ctx.Skill.Rune == 2) && ctx.Skill.Player.GetSetItemCount(742942) >= 6 && ((ctx.Skill.Player.Density.GetDensity(20) > 1 || isBossOrEliteNearby) && (getCurrentMysticAlly() - getCurrentStone()) >= getMaxMysticAlly());//����6��ʱʩ��
                }).ThenCastElseContinue()
                .IfTrue(ctx => {//�������0
                    bool isCOE = ctx.Hud.Game.Me.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.ConventionOfElements.Sno);
                    bool isLesserGods = ctx.Hud.Game.Me.Powers.BuffIsActive(485725); //�ɳ��߰���
                    bool isBossOrEliteNearby = ctx.Hud.Game.ActorQuery.IsEliteOrBossCloserThan(20, false) || ctx.Hud.Game.ActorQuery.NearestGoblin?.NormalizedXyDistanceToMe < 20 || ctx.Hud.Game.ActorQuery.NearestKeywarden?.NormalizedXyDistanceToMe < 20;
                    bool isLesserGodsDebuff = ctx.Hud.Game.AliveMonsters.Any(x => (isBossOrEliteNearby ? (x.Rarity == ActorRarity.Boss || x.Rarity == ActorRarity.Champion || x.Rarity == ActorRarity.Rare || x.Rarity == ActorRarity.Unique) : true) && x.NormalizedXyDistanceToMe < 20 && !x.Illusion && !x.Invulnerable && !x.Invisible && x.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_None, 485725) == 1);
                    return ctx.Skill.Rune == 0 && isLesserGods && isLesserGodsDebuff && (isCOE ? ctx.Skill.Player.Powers.BuffIsActive(ctx.Hud.Sno.SnoPowers.ConventionOfElements.Sno, 3) : true) && getCurrentMysticAlly() >= getMaxMysticAlly();
                }).ThenCastElseContinue()
                ;
        }
        private int getCurrentStone()
        {
            return Hud.Game.Actors.Where(x => x.SummonerAcdDynamicId == Hud.Game.Me.SummonerId && (x.SnoActor.Sno == ActorSnoEnum._monk_female_mystically_crimson || x.SnoActor.Sno == ActorSnoEnum._x1_projectile_mystically_runec_boulder //������ʯͷ
           )
            ).Count();
        }
        private int getCurrentMysticAlly()
        {
            /*bool isanymysticallymini = Hud.Game.Actors.Any(x => x.SummonerAcdDynamicId == Hud.Game.Me.SummonerId && (x.SnoActor.Sno == ActorSnoEnum._x1_monk_female_mysticallymini_crimson) //�����С��
           );
            if (isanymysticallymini && Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting.Sno,0) == true)//����������С����ʱ
            {
                return 0;
            }*/

            int mysticallymini = Hud.Game.Actors.Count(x => x.SummonerAcdDynamicId == Hud.Game.Me.SummonerId && (x.SnoActor.Sno == ActorSnoEnum._x1_monk_female_mysticallymini_crimson) //�����С��
           );
            int threshold = 5;//��ը���ٸ�С���˺��ٴδ���
            if (mysticallymini > 0 && mysticallymini < ((getMaxMysticAlly() * 2 - threshold) < 1 ? 1: (getMaxMysticAlly() * 2 - threshold)) && Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting.Sno, 0) == true)//����������С����ʱ
            {
                return (getMaxMysticAlly() * 2);
            }
            if (mysticallymini >= ((getMaxMysticAlly() * 2 - threshold) < 1 ? 1 : (getMaxMysticAlly() * 2 - threshold)) && Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.Generic_PagesBuffInfiniteCasting.Sno, 0) == true) return 0;

            return  Hud.Game.Actors.Where(x => x.SummonerAcdDynamicId == Hud.Game.Me.SummonerId && (x.SnoActor.Sno == ActorSnoEnum._monk_female_mystically_crimson || x.SnoActor.Sno == ActorSnoEnum._monk_male_mystically_crimson || //�����
           x.SnoActor.Sno == ActorSnoEnum._monk_female_mystically_indigo || x.SnoActor.Sno == ActorSnoEnum._monk_male_mystically_indigo ||//ˮ����
           x.SnoActor.Sno == ActorSnoEnum._monk_female_mystically_obsidian || x.SnoActor.Sno == ActorSnoEnum._monk_male_mystically_obsidian//������
           )
           ).Count();
        }
        private int getMaxMysticAlly()
        {
            if (Hud.Game.Me.GetSetItemCount(742942) >= 6) return 10;
            if (Hud.Game.Me.Powers.BuffIsActive(409811) || Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.TheCrudestBoots.Sno)) return 2;//���ϴֲ�Ь
            return 1;
        }
    }
}