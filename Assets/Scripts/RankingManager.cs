using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager
{
    [System.Serializable]
    public struct Result
    {
        public Result(string nick, int points)
        {
            this.nick = nick;
            this.points = points;
            this.jerseyNumber = "";
        }
        public string nick;
        public int points;
        public string jerseyNumber;
    }
    
    public static List<Result> GetLocalRanking()
    {
        List<Result> results = new List<Result>();

        for (int i = 0; i < 100; i++)
        {
            int points = PlayerPrefs.GetInt("ranking_local_points_" + i);
            string nick = PlayerPrefs.GetString("ranking_local_nick_" + i);
            if (points != 0)
            {
                results.Add(new Result(nick, points));
            }
        }

        return results;
    }

    public static void ClearLocalRanking()
    {
        for (int i = 0; i < 100; i++)
        {
            PlayerPrefs.SetInt("ranking_local_points_" + i, 0);
            PlayerPrefs.SetString("ranking_local_nick_" + i, "");
        }
        
        PlayerPrefs.Save();
    }

    public static void AddResultToLocalRanking(string nick, int points)
    {
        List<Result> ranking = GetLocalRanking();
        ranking.Add(new Result(nick, points));
        ranking.Sort((x, y) => y.points.CompareTo(x.points));

        List<Result> rankingTop100 = new List<Result>();

        for (int i = 0; i < Math.Min(100, ranking.Count); i++)
        {
            rankingTop100.Add(ranking[i]);
        }
        
        ClearLocalRanking();
        
        for (int i = 0; i < rankingTop100.Count; i++)
        {
            PlayerPrefs.SetInt("ranking_local_points_" + i, rankingTop100[i].points);
            PlayerPrefs.SetString("ranking_local_nick_" + i, rankingTop100[i].nick);
        }
    }
}