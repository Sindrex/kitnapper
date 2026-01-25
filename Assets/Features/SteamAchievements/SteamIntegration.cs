using System;
using Steamworks;
using Steamworks.Data;

public enum SteamAchievement
{
    ACH_FINISH_GAME,
    ACH_FIND_ALL_NAMES,
    ACH_CATNIP_UP_DOWN,
    ACH_TEST
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
        CLogger.Log($"Unlocking steam achievement: {achievement}");
        var ach = new Achievement(achievement.ToString());
        if (ach.Trigger())
        {
            CLogger.Log($"Successfully unlocked steam achievement: {achievement}");
        }
        else
        {
            CLogger.Log($"Unable to unlock steam achievement: {achievement}");
        }
    }
}