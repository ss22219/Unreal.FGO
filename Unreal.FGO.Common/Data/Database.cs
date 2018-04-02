using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class Database
{
    //public List<mstAi> mstAi { get; set; }

    //public List<mstAiAct> mstAiAct { get; set; }

    //public List<mstApRecover> mstApRecover { get; set; }

    //public List<mstAttriRelation> mstAttriRelation { get; set; }

    //public List<mstBankShop> mstBankShop { get; set; }

    //public List<mstBanner> mstBanner { get; set; }

    //public List<mstBattleBg> mstBattleBg { get; set; }

    //public List<mstBgm> mstBgm { get; set; }

    //public List<mstBingo> mstBingo { get; set; }

    //public List<mstBingoline> mstBingoline { get; set; }

    //public List<mstBingoPanel> mstBingoPanel { get; set; }

    public List<mstBoxGacha> mstBoxGacha { get; set; }

    public List<mstBoxGachaBase> mstBoxGachaBase { get; set; }

    //public List<mstBoxGachaExtraBase> mstBoxGachaExtraBase { get; set; }

    //public List<mstBoxGachaPickup> mstBoxGachaPickup { get; set; }

    //public List<mstBoxGachaTalk> mstBoxGachaTalk { get; set; }

    //public List<mstBuff> mstBuff { get; set; }

    //public List<mstCard> mstCard { get; set; }

    //public List<mstClass> mstClass { get; set; }

    //public List<mstClassRelation> mstClassRelation { get; set; }

    //public List<mstClosedMessage> mstClosedMessage { get; set; }

    //public List<mstCombineLimit> mstCombineLimit { get; set; }

    //public List<mstCombineMaterial> mstCombineMaterial { get; set; }

    //public List<mstCombineQp> mstCombineQp { get; set; }

    //public List<mstCombineQpSvtEquip> mstCombineQpSvtEquip { get; set; }

    //public List<mstCombineSkill> mstCombineSkill { get; set; }

    //public List<mstCommandSpell> mstCommandSpell { get; set; }

    //public List<mstConstant> mstConstant { get; set; }

    //public List<mstConstantStr> mstConstantStr { get; set; }

    //public List<mstCv> mstCv { get; set; }

    //public List<mstDrop> mstDrop { get; set; }

    //public List<mstEffect> mstEffect { get; set; }

    //public List<mstEquip> mstEquip { get; set; }

    //public List<mstEquipExp> mstEquipExp { get; set; }

    //public List<mstEquipImage> mstEquipImage { get; set; }

    //public List<mstEquipSkill> mstEquipSkill { get; set; }

    public List<mstEvent> mstEvent { get; set; }

    //public List<mstEventCampaign> mstEventCampaign { get; set; }

    //public List<mstEventDetail> mstEventDetail { get; set; }

    //public List<mstEventLoginBonus> mstEventLoginBonus { get; set; }

    //public List<mstEventLoginCampaign> mstEventLoginCampaign { get; set; }

    //public List<mstEventMission> mstEventMission { get; set; }

    //public List<mstEventMissionAction> mstEventMissionAction { get; set; }

    //public List<mstEventMissionCondition> mstEventMissionCondition { get; set; }

    //public List<mstEventMissionConditionDetail> mstEventMissionConditionDetail { get; set; }

    public List<mstEventQuest> mstEventQuest { get; set; }

    //public List<mstEventReward> mstEventReward { get; set; }

    //public List<mstEventRewardSet> mstEventRewardSet { get; set; }

    //public List<mstEventSvt> mstEventSvt { get; set; }

    //public List<mstFriendship> mstFriendship { get; set; }

    //public List<mstFunc> mstFunc { get; set; }

    //public List<mstGacha> mstGacha { get; set; }

    //public List<mstGachaAdjust> mstGachaAdjust { get; set; }

    //public List<mstGachaBase> mstGachaBase { get; set; }

    //public List<mstGachaBonus> mstGachaBonus { get; set; }

    //public List<mstGachaPickup> mstGachaPickup { get; set; }

    //public List<mstGachaRarity> mstGachaRarity { get; set; }

    //public List<mstGameIllustration> mstGameIllustration { get; set; }

    //public List<mstGift> mstGift { get; set; }

    //public List<mstHeroine> mstHeroine { get; set; }

    //public List<mstIllustrator> mstIllustrator { get; set; }

    public List<mstItem> mstItem { get; set; }

    //public List<mstMapGimmick> mstMapGimmick { get; set; }

    //public List<mstMission> mstMission { get; set; }

    //public List<mstPresentMessage> mstPresentMessage { get; set; }

    public List<mstQuest> mstQuest { get; set; }

    public List<mstQuestGroup> mstQuestGroup { get; set; }

    public List<mstQuestPhase> mstQuestPhase { get; set; }

    public List<mstQuestRelease> mstQuestRelease { get; set; }

    //public List<mstSetItem> mstSetItem { get; set; }

    //public List<mstShop> mstShop { get; set; }

    //public List<mstShopScript> mstShopScript { get; set; }

    //public List<mstSkill> mstSkill { get; set; }

    //public List<mstSkillDetail> mstSkillDetail { get; set; }

    //public List<mstSkillLv> mstSkillLv { get; set; }

    public List<mstSpot> mstSpot { get; set; }

    public List<mstSpotRoad> mstSpotRoad { get; set; }

    //public List<mstStage> mstStage { get; set; }

    //public List<mstStoneShop> mstStoneShop { get; set; }

    public List<mstSvt> mstSvt { get; set; }

    //public List<mstSvtCard> mstSvtCard { get; set; }

    //public List<mstSvtComment> mstSvtComment { get; set; }

    //public List<mstSvtExp> mstSvtExp { get; set; }

    public List<mstSvtGroup> mstSvtGroup { get; set; }

    public List<mstSvtLimit> mstSvtLimit { get; set; }

    //public List<mstSvtLimitAdd> mstSvtLimitAdd { get; set; }

    //public List<mstSvtRarity> mstSvtRarity { get; set; }

    //public List<mstSvtScript> mstSvtScript { get; set; }

    //public List<mstSvtSkill> mstSvtSkill { get; set; }

    //public List<mstSvtTreasureDevice> mstSvtTreasureDevice { get; set; }

    //public List<mstSvtVoice> mstSvtVoice { get; set; }

    //public List<mstTips> mstTips { get; set; }

    //public List<mstTotalLogin> mstTotalLogin { get; set; }

    //public List<mstTreasureDevice> mstTreasureDevice { get; set; }

    //public List<mstTreasureDeviceDetail> mstTreasureDeviceDetail { get; set; }

    //public List<mstTreasureDeviceLv> mstTreasureDeviceLv { get; set; }

    //public List<mstUserExp> mstUserExp { get; set; }

    //public List<mstUserTrem> mstUserTrem { get; set; }

    //public List<mstVoice> mstVoice { get; set; }

    public List<mstWar> mstWar { get; set; }

    //public List<npcDeck> npcDeck { get; set; }

    public List<npcFollower> npcFollower { get; set; }

    //public List<npcSvt> npcSvt { get; set; }

    //public List<npcSvtFollower> npcSvtFollower { get; set; }
}

