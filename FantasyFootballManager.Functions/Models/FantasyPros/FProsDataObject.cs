using System.Collections.Generic;

namespace FantasyFootballManager.Functions.Models.FantasyPros;

public class Root
{
    public string sport { get; set; }
    public string type { get; set; }
    public string ranking_type_name { get; set; }
    public string year { get; set; }
    public string week { get; set; }
    public string position_id { get; set; }
    public string scoring { get; set; }
    public string filters { get; set; }
    public int count { get; set; }
    public int total_experts { get; set; }
    public string last_updated { get; set; }
    public List<Player> players { get; set; }
}

public class Player
{
    public int player_id { get; set; }
    public string player_name { get; set; }
    public string sportsdata_id { get; set; }
    public string player_team_id { get; set; }
    public string player_position_id { get; set; }
    public string player_positions { get; set; }
    public string player_short_name { get; set; }
    public string player_eligibility { get; set; }
    public string player_yahoo_positions { get; set; }
    public string player_page_url { get; set; }
    public string player_filename { get; set; }
    public string player_square_image_url { get; set; }
    public string player_image_url { get; set; }
    public string player_yahoo_id { get; set; }
    public string cbs_player_id { get; set; }
    public string player_bye_week { get; set; }
    public double player_owned_avg { get; set; }
    public double player_owned_espn { get; set; }
    public int player_owned_yahoo { get; set; }
    public int? player_ecr_delta { get; set; }
    public int rank_ecr { get; set; }
    public string rank_min { get; set; }
    public string rank_max { get; set; }
    public string rank_ave { get; set; }
    public string rank_std { get; set; }
    public string pos_rank { get; set; }
    public int tier { get; set; }
}