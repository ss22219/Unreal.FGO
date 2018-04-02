using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core
{
    public class QuestChecker
    {
        public long TimetampSecond { get; set; }
        public List<HomeUserquest> userQuest { get; set; }

        public bool CheckQuestClear(string questId)
        {
            if (userQuest != null && userQuest.Count > 0)
            {
                return userQuest.Any(q => q.questId == questId && q.clearNum > 0);
            }
            return false;
        }

        public int GetUserItemNum(string itemId)
        {
            return 0;
        }
        public int GetSvtLevel(string svtId)
        {
            return 0;
        }

        public bool IsGetSvt(string svtId)
        {
            return false;
        }

        public int GetSvtLimit(string svtId)
        {
            return 0;
        }
        public int GetSvtFriendShip(string svtId)
        {
            return 0;
        }

        public bool IsGetSvyGroup(string groupId)
        {
            return false;
        }

        public bool CheckEventOpen(string eventId)
        {
            var mstEvent = AssetManage.Database.mstEvent.FirstOrDefault(e => e.id == eventId);
            var time = TimetampSecond;
            if (mstEvent != null)
            {
                if ((time < mstEvent.startedAt) || (time > mstEvent.endedAt))
                {
                    return false;
                }
                if (mstEvent.openType == 1)
                {
                    return (mstEvent.endedAt - time) > 0;
                }
                if (mstEvent.openHours == 0)
                {
                    return false;
                }
                if (mstEvent.intervalHours == 0)
                {
                    return false;
                }
                long num2 = mstEvent.openHours * 0xe10;
                long num3 = mstEvent.intervalHours * 0xe10;
                long num4 = num2 + num3;
                long num5 = (time - mstEvent.startedAt) % num4;
                if (num5 > num2)
                {
                    return false;
                }
                return (num2 - num5) > 0;
            }
            return false;
        }
        public static List<string> ClosedQuest = new List<string>();
        //开始结束时间检查
        public bool CheckQuestStartEndTime(string questId)
        {
            if (ClosedQuest.Contains(questId))
                return false;
            var quest = AssetManage.Database.mstQuest.FirstOrDefault(q => q.id == questId);
            if (quest != null)
            {
                long num2 = TimetampSecond;
                if ((quest.openedAt <= num2) && (quest.closedAt > num2))
                {
                    return true;
                }
                else if(quest.closedAt < num2)
                {
                    ClosedQuest.Add(questId);
                }
            }
            return false;
        }

        //开始结束时间 + 开始时间段检查
        public bool CheckQuestOpenTime(string questId)
        {
            var quest = AssetManage.Database.mstQuest.FirstOrDefault(q => q.id == questId);
            if (quest == null)
                return false;
            long num = TimetampSecond;
            if ((num < quest.openedAt) || (num > quest.closedAt))
            {
                return false;
            }
            if (quest.displayHours == 0)
            {
                return false;
            }
            if (quest.intervalHours == 0)
            {
                return false;
            }
            long num2 = quest.displayHours * 0xe10;
            long num3 = quest.intervalHours * 0xe10;
            long num4 = num2 + num3;
            long num5 = (num - quest.openedAt) % num4;
            if (num5 > num2)
            {
                return false;
            }
            return (num2 - num5) > 0;
        }

        public bool CheckQuestEventOpen(string questEventId)
        {
            return false;
        }

        public bool IsMissionAchive(string eventMissionId)
        {
            return false;
        }

        public bool CheckQuestGroupClear(string questId, string groupId, int clearNum)
        {
            var groups = AssetManage.Database.mstQuestGroup.Where(g => g.groupId == groupId);
            var userQuest = this.userQuest != null ? this.userQuest.FirstOrDefault(q => q.questId == questId) : null;
            string[] questIdListByGroupId = groups.Select(g => g.questId).ToArray();
            int num = 0;
            foreach (string groupQuestId in questIdListByGroupId)
            {
                if (questId != groupQuestId)
                {
                    if (CheckQuestClear(groupQuestId))
                    {
                        num++;
                    }
                    else
                    {

                        var entity = this.userQuest != null ? this.userQuest.FirstOrDefault(q => q.questId == groupQuestId) : null;
                        if ((entity != null) && (entity.questPhase >= 1))
                        {
                            num++;
                        }
                    }
                }
            }
            return (num < clearNum);
        }


        public bool CheckQuestRelease(mstQuestRelease release)
        {
            var val = release.value;
            var type = release.type;
            switch (type)
            {
                case 1:
                    return CheckQuestClear(release.targetId);
                case 2:
                    return GetUserItemNum(release.targetId) >= val;
                case 6:
                    return CheckQuestClear(release.questId) || GetSvtLevel(release.targetId) >= val;
                case 7:
                    return CheckQuestClear(release.questId) || GetSvtLimit(release.targetId) >= val;
                case 8:
                    return CheckQuestClear(release.questId) || IsGetSvt(release.targetId);
                case 9:
                    return CheckQuestClear(release.questId) || GetSvtFriendShip(release.targetId) >= val;
                case 10:
                    return CheckQuestClear(release.questId) || IsGetSvyGroup(release.targetId);
                case 11:
                    return CheckEventOpen(release.targetId);
                case 12:
                    return CheckQuestStartEndTime(release.questId);
                case 13:
                    return CheckQuestOpenTime(release.questId);
                case 0x18:
                    return IsMissionAchive(release.targetId);
                case 0x19:
                    return CheckQuestGroupClear(release.questId, release.targetId, val);
                default:
                    break;
            }
            return false;
        }

        public bool IsQuestRelease(mstQuest quest)
        {
            var relaseList = AssetManage.Database.mstQuestRelease.Where(r => r.questId == quest.id).ToList();
            foreach (var release in relaseList)
            {
                if (!CheckQuestRelease(release))
                    return false;
            }
            return true;
        }

        public List<mstQuest> GetUserQuestList()
        {
            var list = new List<mstQuest>();
            foreach (var quest in AssetManage.Database.mstQuest)
            {
                if (IsQuestRelease(quest))
                    list.Add(quest);
            }
            list.RemoveAll(q => q.afterClear == "1" && CheckQuestClear(q.id));
            return list;
        }
    }
}
