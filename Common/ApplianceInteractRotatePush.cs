using Kitchen;
using Unity.Entities;

namespace KitchenSuperCrane.Common
{
    [UpdateBefore(typeof(ShowPingedApplianceInfo))]
    internal class ApplianceInteractRotatePush : ApplianceInteractionSystem
    {
        private CConveyPushRotatable Rotatable;
        protected override InteractionType RequiredType => InteractionType.Notify;

        protected override bool RequirePress => true;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Target, out Rotatable))
            {
                return false;
            }

            if (Rotatable.Target == Orientation.Null)
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
            Set(data.Target, Rotatable);
        }
    }
}
