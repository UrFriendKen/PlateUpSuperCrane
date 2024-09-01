using Controllers;
using Kitchen;

namespace KitchenSuperCrane.Helpers
{
    internal static class MouseUtils
    {
        public static bool IsMouseUser(int player_id)
        {
            return TryGetPreferredMouseUserID(out int preferredPlayerID) &&
                preferredPlayerID == player_id;
        }

        public static bool TryGetPreferredMouseUserID(out int preferredPlayerID)
        {
            string preferredCraneOwnerName = Main.PrefManager.Get<string>(Main.PREFERRED_CRANE_OWNER_ID);
            preferredPlayerID = 0;

            int firstNonKeyboardPlayerID = 0;
            foreach (PlayerInfo playerInfo in Players.Main.All())
            {
                if (!playerInfo.IsLocalUser)
                    continue;

                ControllerType controllerType = InputSourceIdentifier.DefaultInputSource.GetCurrentController(playerInfo.ID);

                if (controllerType == ControllerType.Keyboard)
                {
                    preferredPlayerID = playerInfo.ID;
                    return true;
                }

                if (controllerType != ControllerType.None &&
                    firstNonKeyboardPlayerID == 0)
                {
                    firstNonKeyboardPlayerID = playerInfo.ID;
                }

                if (preferredPlayerID == 0 &&
                    !string.IsNullOrEmpty(preferredCraneOwnerName) &&
                    playerInfo.IsLocalUser &&
                    playerInfo.HasProfile &&
                    preferredCraneOwnerName == playerInfo.Profile.Name)
                {
                    preferredPlayerID = playerInfo.ID;
                }
            }

            if (preferredPlayerID == 0)
                preferredPlayerID = firstNonKeyboardPlayerID;

            return preferredPlayerID != 0;
        }
    }
}
