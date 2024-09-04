using Controllers;
using Kitchen;
using KitchenMods;
using KitchenSuperCrane.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace KitchenSuperCrane.Common
{
    [FilterModes(AllowedModes = GameConnectionMode.All)]
    public class MouseAssignmentHandler : GenericSystemBase, IModSystem
    {
        FieldInfo f_Players;

        protected override void Initialise()
        {
            base.Initialise();
            f_Players = typeof(BaseInputSource).GetField("Players", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        int LastMouseUser = 0;

        protected override void OnUpdate()
        {
            IEnumerable<InputDevice> mouseDevices = InputSystem.devices.Where(x => x.path == "/Mouse");

            if (!MouseUtils.TryGetPreferredMouseUserID(out int preferredPlayerID) ||
                !mouseDevices.Any())
                return;

            //if (LastMouseUser != 0 &&
            //    LastMouseUser == preferredPlayerID)
            //    return;

            Dictionary<int, PlayerData> players = (Dictionary<int, PlayerData>)f_Players?.GetValue(InputSourceIdentifier.DefaultInputSource);

            if (players == default || !players.TryGetValue(preferredPlayerID, out PlayerData playerData))
                return;

            bool isChanged = false;
            foreach (KeyValuePair<int, PlayerData> kvp in players)
            {
                InputUser user = kvp.Value.InputData.User;

                foreach (InputDevice device in mouseDevices)
                {
                    bool isPairedToUser = user.pairedDevices.Contains(device);
                    if (kvp.Key == preferredPlayerID)
                    {
                        if (!isPairedToUser)
                        {
                            isChanged = true;
                            InputUser.PerformPairingWithDevice(device, user);
                        }
                    }
                    else if (isPairedToUser)
                    {
                        isChanged = true;
                        user.UnpairDevice(device);
                    }
                }
            }

            if (isChanged)
            {
                if (preferredPlayerID == 0)
                    Main.LogInfo($"Un-paired mouse from users.");
                else
                    Main.LogInfo($"Re-paired mouse to user {preferredPlayerID}");
            }
            //LastMouseUser = preferredPlayerID;
        }
    }
}
