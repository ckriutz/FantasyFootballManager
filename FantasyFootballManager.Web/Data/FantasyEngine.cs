namespace FantasyFootballManager.Web.Data;

public class FantasyEngine
{
    private const int NUMOFLEAUGEPLAYERS = 12;
    public FantasyEngine() { }

    public static async Task<List<RulesEngine.Models.RuleResultTree>> RunEngine(List<FootballPlayer> players)
    {
        var workflows = new List<RulesEngine.Models.WorkflowRules>();

        var rbPlayers = players.Where(p => (p.Position == "RB") && (p.ADP / NUMOFLEAUGEPLAYERS) <= 3).Count();

        RulesEngine.Models.WorkflowRules playerNeedWorkflow = new RulesEngine.Models.WorkflowRules();
        playerNeedWorkflow.WorkflowName = "Determine Player Need";
        
        List<RulesEngine.Models.Rule> rules = new List<RulesEngine.Models.Rule>();

        RulesEngine.Models.Rule ruleRB = new RulesEngine.Models.Rule();
        ruleRB.RuleName = "Need a top 24 RB";
        ruleRB.SuccessEvent = "Top 24 RB Need Achieved.";
        ruleRB.ErrorMessage = "Need top 24 RB";
        ruleRB.Expression = $"input1.Where(p => p.Position == \"RB\" && (p.ADP / {NUMOFLEAUGEPLAYERS}) <= 3).Count() > 0";
        ruleRB.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleRB);

        RulesEngine.Models.Rule ruleWR = new RulesEngine.Models.Rule();
        ruleWR.RuleName = "Need a top 24 WR";
        ruleWR.SuccessEvent = "Top 24 WR Need Achieved.";
        ruleWR.ErrorMessage = "Need top 24 WR";
        ruleWR.Expression = $"input1.Where(p => p.Position == \"WR\" && (p.ADP / {NUMOFLEAUGEPLAYERS}) <= 3).Count() > 0";
        ruleWR.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleWR);

        
        playerNeedWorkflow.Rules = rules;
        workflows.Add(playerNeedWorkflow);

        var bre = new RulesEngine.RulesEngine(workflows.ToArray(), null);

        List<RulesEngine.Models.RuleResultTree> resultList = await bre.ExecuteAllRulesAsync("Determine Player Need", players.Where(p=> p.IsOnMyTeam).ToList());
        return resultList;
        //foreach (var item in resultList)
        //{               
            //Console.WriteLine(" Verification succeeded: {0} , message: {1}",item.IsSuccess,item.ExceptionMessage);
        //}
    }
}