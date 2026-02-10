public class UnlockAchievementEvent : EventBase
{
    public SteamAchievement Achievement;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("UnlockAchievementEvent activated!");
        SteamIntegration.UnlockAchievement(Achievement);
    }
}
