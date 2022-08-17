class HP : ParentStat {
    public HP(int constructStat, int constructTribe, int constructEffort, int constructLevel, int constructPersonality)
        : base(constructStat, constructTribe, constructEffort, constructLevel, constructPersonality)
    {
        // another event
    }
    public override int SettingRealStat() {
        // [ { (종족값 x 2) + 개체값 + (노력치/4) + 100 } x 레벨/100 ] + 10
        return (((STAT * 2) + tribe + (effort / 4)) * level / 100 + 5) * personality;
    }
}