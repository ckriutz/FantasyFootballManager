using System;
using System.Text.Json.Serialization;

namespace FantasyFootballManager.Functions.Models.FantasyFootballCalculator;

    public class FootballCalculatorDataObject
    {
        public int player_id { get; set; }          // ✔  2876
        public string name { get; set; }            // "D.J. Moore"   
        public string position { get; set; }        // "WR"
        public string team { get; set; }            // "CAR"        
        public double adp { get; set; }             // ✔  67.5
        public string adp_formatted { get; set; }   // 6.08
        public int times_drafted { get; set; }      // 207
        public int high { get; set; }               // 52
        public int low { get; set; }                // 82
        public double stdev { get; set; }           // 5.7
        public int bye { get; set; }                // 13
    }