﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Idea
{
    public class KpiManager:KPI
    {
        private static KpiManager instance = new KpiManager();

        public static KpiManager GetInstance()
        {
            return instance;
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select top 1 ID  from KPI WHERE ID LIKE @id + '%' order by ID desc";

            KPI idea = DBManager<KPI>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (idea == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = idea.ID.Trim().Substring(8);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }

        public int InsertKPI(string IDEA_ID, string PRJ_MONTH, int TARGET_VALUE)
        {
            var ID = GenerateId();
            var sql = "INSERT INTO KPI(ID,IDEA_ID,PRJ_MONTH,TARGET_VALUE) VALUES(@ID,@IDEA_ID,@PRJ_MONTH,@TARGET_VALUE)";
            return DBManager<KPI>.Execute(sql, new { ID=ID, IDEA_ID = IDEA_ID, PRJ_MONTH = PRJ_MONTH, TARGET_VALUE = TARGET_VALUE });
        }
    }
}