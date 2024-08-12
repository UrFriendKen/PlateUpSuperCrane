using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenSuperCrane.Common
{
    [UpdateBefore(typeof(ShowPingedApplianceInfo))]
    internal class ApplianceInteractRotatePush : ApplianceInteractionSystem, IModSystem
    {
        private CConveyPushRotatable Rotatable;
        protected override InteractionType RequiredType => InteractionType.Notify;

        protected override bool RequirePress => true;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Has<CIsCraneMode>(data.Interactor))
                return false;
            if (!data.Context.Require(data.Target, out Rotatable) ||
                Rotatable.Target == Orientation.Null)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Rotatable.Target = Rotatable.Target.RotateCW();
            if (Rotatable.Target == Orientation.Down)
            {
                Rotatable.Target = Orientation.Left;
            }
            data.Context.Set(data.Target, Rotatable);
        }
    }
}
