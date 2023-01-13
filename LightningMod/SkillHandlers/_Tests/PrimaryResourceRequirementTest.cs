namespace Turbo.Plugins.LightningMod
{
    using System;

    public class PrimaryResourceRequirementTest : AbstractSkillTest
    {
        public int SpareResource { get; set; }
        public Func<TestContext, int> BaseResourceCalculatorFunc { get; set; }

        internal override SkillTestResult Test(TestContext context)
        {
            if (BaseResourceCalculatorFunc == null) return SkillTestResult.Continue;
            var resourceRequirement = this.BaseResourceCalculatorFunc(context) == 0 ? context.Skill.GetResourceRequirement() : context.Skill.GetResourceRequirement(this.BaseResourceCalculatorFunc(context));
            resourceRequirement += this.SpareResource;

            if (context.Skill.Player.Stats.ResourceCurPri < resourceRequirement)
            {
                return ResultOnFail;
            }
            else return ResultOnSuccess;
        }
    }

    public static class PrimaryResourceRequirementTestFluent
    {
        public static PrimaryResourceRequirementTest IfPrimaryResourceIsEnough(this AbstractSkillTest parent, int spareResource, Func<TestContext, int> baseResourceCalculatorFunc)
        {
            var test = new PrimaryResourceRequirementTest()
            {
                SpareResource = spareResource,
                BaseResourceCalculatorFunc = baseResourceCalculatorFunc,
            };

            parent.NextTest = test;
            return test;
        }
    }
}