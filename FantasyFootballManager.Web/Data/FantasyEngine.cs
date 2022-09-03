namespace FantasyFootballManager.Web.Data;

public class FantasyEngine
{
    private const int NUMOFLEAUGEPLAYERS = 12;
    public FantasyEngine() { }

    public static async Task<List<RuleResult>> RunEngine(List<FootballPlayer> players)
    {
        var workflows = new List<RulesEngine.Models.WorkflowRules>();

        var rbPlayers = players.Where(p => (p.Position == "RB") && (p.ADP / NUMOFLEAUGEPLAYERS) <= 3).Count();

        RulesEngine.Models.WorkflowRules playerNeedWorkflow = new RulesEngine.Models.WorkflowRules();
        playerNeedWorkflow.WorkflowName = "Determine Player Need";
        
        List<RulesEngine.Models.Rule> rules = new List<RulesEngine.Models.Rule>();

        RulesEngine.Models.Rule ruleRB = new RulesEngine.Models.Rule();
        ruleRB.RuleName = "Need a Round 1 or Round 2 RB";
        ruleRB.SuccessEvent = "Round 1 or Round 2 RB Need Achieved.";
        ruleRB.ErrorMessage = System.Text.Json.JsonSerializer.Serialize<RuleResult>(new RuleResult() { Importance = 10, Position = "RB", Message = "Need a Round 1 or Round 2 RB."});
        ruleRB.Expression = $"input1.Where(p => p.Position == \"RB\" && (p.ADP / {NUMOFLEAUGEPLAYERS}) <= 3).Count() > 0";
        ruleRB.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleRB);

        RulesEngine.Models.Rule ruleWR = new RulesEngine.Models.Rule();
        ruleWR.RuleName = "Need a Round 1 or Round 2 WR";
        ruleWR.SuccessEvent = "Round 1 or Round 2 WR Need Achieved.";
        ruleWR.ErrorMessage = System.Text.Json.JsonSerializer.Serialize<RuleResult>(new RuleResult() { Importance = 15, Position = "WR", Message = "Need a Round 1 or Round 2 WR."});
        ruleWR.Expression = $"input1.Where(p => p.Position == \"WR\" && (p.ADP / {NUMOFLEAUGEPLAYERS}) <= 3).Count() > 0";
        ruleWR.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleWR);

        RulesEngine.Models.Rule ruleBackupQB = new RulesEngine.Models.Rule();
        ruleBackupQB.RuleName = "Need a backup QB";
        ruleBackupQB.SuccessEvent = "Backup QB Achieved.";
        ruleBackupQB.ErrorMessage = System.Text.Json.JsonSerializer.Serialize<RuleResult>(new RuleResult() { Importance = 100, Position = "QB", Message = "Need a backup QB."});
        ruleBackupQB.Expression = $"input1.Where(p => p.Position == \"QB\").Count() >= 2";
        ruleBackupQB.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleBackupQB);

        RulesEngine.Models.Rule ruleKicker = new RulesEngine.Models.Rule();
        ruleKicker.RuleName = "Need a Kicker";
        ruleKicker.SuccessEvent = "Have a Kicker, so we are good..";
        ruleKicker.ErrorMessage = System.Text.Json.JsonSerializer.Serialize<RuleResult>(new RuleResult() { Importance = 150, Position = "K", Message = "Need a Kicker."});
        ruleKicker.Expression = $"input1.Where(p => p.Position == \"K\").Count() == 1";
        ruleKicker.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleKicker);

        RulesEngine.Models.Rule ruleDefense = new RulesEngine.Models.Rule();
        ruleDefense.RuleName = "Need a Defense";
        ruleDefense.SuccessEvent = "Have a Defense, so we are good..";
        ruleDefense.ErrorMessage = System.Text.Json.JsonSerializer.Serialize<RuleResult>(new RuleResult() { Importance = 125, Position = "DEF", Message = "Need a Defense."});
        ruleDefense.Expression = $"input1.Where(p => p.Position == \"DEF\").Count() == 1";
        ruleDefense.RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression;
        rules.Add(ruleDefense);

        
        playerNeedWorkflow.Rules = rules;
        workflows.Add(playerNeedWorkflow);

        var bre = new RulesEngine.RulesEngine(workflows.ToArray(), null);

        List<RulesEngine.Models.RuleResultTree> resultList = await bre.ExecuteAllRulesAsync("Determine Player Need", players.Where(p=> p.IsOnMyTeam).ToList());
        
        List<RuleResult> ruleResults = new List<RuleResult>();
        foreach (var item in resultList)
        {               
            //Console.WriteLine(" Verification succeeded: {0} , message: {1}",item.IsSuccess,item.ExceptionMessage);
            if (item.IsSuccess == false)
            {
                RuleResult r = System.Text.Json.JsonSerializer.Deserialize<RuleResult>(item.ExceptionMessage);
                ruleResults.Add(r);
            }
        }
        return ruleResults;
    }
}