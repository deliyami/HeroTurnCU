class ParentStat : IStatInterface {
    // 성격은 ATK, DEF... ParentStat까지는 index로 받고
    // AllStat에서 case로 할 것
    public int STAT {get; set;}
    public int tribe {get; set;}
    public int effort {get; set;}
    public int level {get; set;}
    public int personality {get; set;}

    public ParentStat(int constructStat, int constructTribe, int constructEffort, int constructLevel, int constructPersonality)
    {
        // another event
        ChangeStat(constructStat, constructTribe, constructEffort, constructLevel, constructPersonality);
    }
    public void ChangeStat(int newStat, int newTribe, int newEffort, int newLevel, int newPersonality) {
        STAT = newStat;
        tribe = newTribe;
        effort = newEffort;
        level = newLevel;
        personality = newPersonality;
    }
    public void LevelChange(int newLevel){
        ChangeStat(STAT, tribe, effort, newLevel, personality);
    }
    // [ { (종족값 x 2) + 개체값 + (노력치/4) } x 레벨/100 + 5] x 성격보정
    virtual public int SettingRealStat() {
        return (((STAT * 2) + tribe + (effort / 4)) * level / 100 + 5) * personality;
    }
}