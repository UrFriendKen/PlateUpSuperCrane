using Controllers;
using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenSuperCrane.FranchiseMode
{
    public class ActivateCraneMode : FranchiseSystem, IModSystem
    {
        EntityQuery CraneActivators;
        EntityQuery PlayersWithoutCraneMode;
        EntityQuery Upgrade;

        protected override void Initialise()
        {
            base.Initialise();
            CraneActivators = GetEntityQuery(typeof(CActivatingCraneMode), typeof(CBlockPing), typeof(CInputData));
            PlayersWithoutCraneMode = GetEntityQuery(new QueryHelper().All(typeof(CPlayer), typeof(CInputData)).None(typeof(CActivatingCraneMode)));
            Upgrade = GetEntityQuery(typeof(CUpgradeAdvancedBuildMode));
        }

        protected override void OnUpdate()
        {
            EntityManager.AddComponent<CActivatingCraneMode>(PlayersWithoutCraneMode);

            using NativeArray<Entity> entities = CraneActivators.ToEntityArray(Allocator.Temp);
            using NativeArray<CActivatingCraneMode> activatingCraneModes = CraneActivators.ToComponentDataArray<CActivatingCraneMode>(Allocator.Temp);
            using NativeArray<CBlockPing> blockPings = CraneActivators.ToComponentDataArray<CBlockPing>(Allocator.Temp);
            using NativeArray<CInputData> inputDatas = CraneActivators.ToComponentDataArray<CInputData>(Allocator.Temp);

            float dt = Time.DeltaTime;
            bool hasCraneUpgrade = !Upgrade.IsEmpty;

            for (int i = 0; i < activatingCraneModes.Length; i++)
            {
                Entity entity = entities[i];
                CActivatingCraneMode crane_mode = activatingCraneModes[i];
                CBlockPing block_ping = blockPings[i];
                CInputData inputs = inputDatas[i];

                if (crane_mode.IsDeactivated)
                {
                    if (inputs.State.SecondaryAction2 == ButtonState.Released)
                    {
                        crane_mode.Reactivate();
                    }
                }
                else if (inputs.State.SecondaryAction2 == ButtonState.Held)
                {
                    crane_mode.Progress += dt;
                }
                else
                {
                    crane_mode.Progress = 0f;
                }
                block_ping.IsEnablingCraneMode |= crane_mode.Progress > 0.3f;
                block_ping.IsEnablingCraneMode &= inputs.State.SecondaryAction2 != ButtonState.Up;
                if (crane_mode.IsComplete)
                {
                    crane_mode.Deactivate();
                    if (Has<CIsCraneMode>(entity))
                    {
                        Unset<CIsCraneMode>(entity);
                    }
                    else if (hasCraneUpgrade)
                    {
                        Set<CIsCraneMode>(entity);
                    }
                }
                Set(entity, crane_mode);
                Set(entity, block_ping);
            }
        }
    }
}
