using System;
using Steamworks;
using Steamworks.Data;

public enum SteamAchievement
{
    ACH_FINISH_GAME, //finale moment class
    ACH_FIND_ALL_NAMES, //flag, checks with finalemoment
    ACH_CATNIP_UP_DOWN, //catnipevent
    ACH_FIND_ALL_GOLDEN_BALLS, //AchivEvent
    ACH_FIND_TURBO_BABA, //AchivEvent
    ACH_LISTEN_TO_ALL_TURBO_BABA_JOKES, //AchivEvent
    ACH_TALK_TO_SLEEPY_CAT //AchivEvent
}

public static class SteamIntegration
{
    public const int GameId = 4154720;
    public static bool Initialized = false;

    public static void Initialize()
    {
        if (!Initialized)
        {            
            try
            {
                SteamClient.Init(GameId);
                Initialized = true;
                CLogger.Log($"Finished initializing SteamClient");

                CLogger.Log("Finding game achievements!");
                foreach (var a in SteamUserStats.Achievements)
                {
                    CLogger.Log( $"{a.Name} ({a.State})" );
                }	
            }
            catch (Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?

                CLogger.LogError($"Unable to initialize SteamClient!");
                CLogger.LogError(e);
            }
        }
        else
        {
            CLogger.Log($"SteamClient already initialized.");
        }
    }

    public static void OnUpdate()
    {
        SteamClient.RunCallbacks();
    }

    public static void Shutdown()
    {
        CLogger.Log($"Shutting down SteamClient.");
        SteamClient.Shutdown();
    }

    public static void UnlockAchievement(SteamAchievement achievement)
    {
        try
        {
            CLogger.Log($"Unlocking steam achievement: {achievement}");
            var ach = new Achievement(achievement.ToString());
            if (ach.Trigger())
            {
                CLogger.Log($"Successfully unlocked steam achievement: {achievement}");
            }
            else
            {
                CLogger.LogError($"Unable to unlock steam achievement: {achievement}");
            }
        }
        catch (Exception e)
        {
            CLogger.LogError($"Error occurred while unlocking steam achievement: {achievement}");
            CLogger.LogError(e);
        }
    }

    public static bool IsAchievementUnlocked(SteamAchievement achievement)
    {
        try
        {
            var ach = new Achievement(achievement.ToString());
            return ach.State;
        }
        catch(Exception e)
        {
            CLogger.LogError($"Unable to check state of steam achievement: {achievement}");
            CLogger.LogError(e);
            return false;
        }
    }
}