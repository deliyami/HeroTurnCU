interface IStatInterface
{
    // 종족값
    int STAT {get; set;}
    // 개체치
    int tribe {get; set;}
    // 노력치
    int effort {get; set;} 
    // 레벨
    int level {get; set;}
    // 성격
    int personality {get; set;}

    void ChangeStat(int newStat, int newTribe, int newEffort, int newLevel, int newPersonality);
    int SettingRealStat();
}