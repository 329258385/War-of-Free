using System;
using UnityEngine;
using GameCore.Loader;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Solarmax {
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskType {
        Level = 1,      //关卡任务
        Daily,          //日常任务
        Achieve,        //成就任务
    }

    /// <summary>
    /// 奖励类型
    /// </summary>
    public enum RewardType {
        Gold,          //金币
    }

    public enum TaskStatus {
        Unfinished,     //未完成，未领取
        Completed,      //已完成，未领取
        Received,       //已完成，已领取
    }

    /// <summary>
    /// 任务配置
    /// </summary>
    public class TaskConfig {
        public string       id;                 //id
        public string       descId;             //描述id
        public string       levelId;            //关卡id
         
        public TaskType     taskType;           //任务类型
        public RewardType   rewardType;         //奖励类型

        public int          taskParameter;      //完成条件 
        public int          rewardValue;        //奖励数量

        public TaskStatus   status;             //任务状态

        public bool Load(XElement element) {
            id = element.GetAttribute("id", "");
            taskType = (TaskType)Convert.ToInt32(element.GetAttribute("TaskType", ""));
            if ((int)taskType == 1) {
                levelId = element.GetAttribute("LevelID", "");
            } else {
                descId = element.GetAttribute("DictionaryID", "");
            }

            rewardValue = Convert.ToInt32(element.GetAttribute("RewardValue", ""));
            taskParameter = Convert.ToInt32(element.GetAttribute("TaskParameter", ""));
            rewardType = (RewardType)Convert.ToInt32(element.GetAttribute("RewardType", ""));

            return true;
        }
    }

    public class TaskConfigProvider : Singleton<TaskConfigProvider>, IDataProvider {
        public List<TaskConfig> dataList = new List<TaskConfig>();
        public List<TaskConfig> levelList = new List<TaskConfig>();
        public List<TaskConfig> dailyList = new List<TaskConfig>();
        public List<TaskConfig> achieveList = new List<TaskConfig>();

        public string Path() {
            return "/data/Task.Xml";
        }

        public bool IsXML() {
            return false;
        }

        public void Load() {
            try {
                string url = UtilTools.GetStreamAssetsByPlatform(Path());
                if (string.IsNullOrEmpty(url)) {
                    return;
                }

                WWW www = new WWW(url);
                while (!www.isDone) ;

                if (!string.IsNullOrEmpty(www.text)) {
                    XDocument xmlDoc = XDocument.Parse(www.text);
                    var xElement = xmlDoc.Elements("configs");
                    if (xElement == null) {
                        return;
                    }

                    var elements = xElement.Elements("config");
                    foreach (var em in elements) {
                        TaskConfig item = new TaskConfig();
                        if (item.Load(em)) {
                            switch (item.taskType) {
                                case TaskType.Level:
                                    levelList.Add(item);
                                    break;

                                case TaskType.Daily:
                                    dailyList.Add(item);
                                    break;

                                case TaskType.Achieve:
                                    achieveList.Add(item);
                                    break;
                            }

                            dataList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e) {
                LoggerSystem.Instance.Error("data/Task.Xml resource failed " + e.ToString());
            }
        }

        public bool Verify() {
            return true;
        }

        public void LoadExtraData() {
        }

        public List<TaskConfig> GetAllData() {
            return dataList;
        }

        public List<TaskConfig> GetLevelData() {
            return levelList;
        }

        public List<TaskConfig> GetDailyData() {
            return dailyList;
        }

        public List<TaskConfig> GetAchieveData() {
            return achieveList;
        }
    }
}