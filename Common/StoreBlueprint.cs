using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenSuperCrane.Common
{
    [UpdateBefore(typeof(PickUpAndDropAppliance))]
    public class StoreCopiedBlueprint : ApplianceInteractionSystem, IModSystem
    {
        private CItemHolder Holder;

        private CApplianceBlueprint Blueprint;

        private CForSale Sale;

        private CAppliance Appliance;

        private CBlueprintStore Store;

        protected override InteractionType RequiredType => InteractionType.Grab;

        protected override bool IsPossible(ref InteractionData data)
        {
            data.Context.Require(data.Interactor, out Holder);

            if (!data.Context.Require(data.Interactor, out Holder) ||
                !data.Context.Require((Entity)Holder, out Blueprint) ||
                !Blueprint.IsCopy ||
                !data.Context.Require((Entity)Holder, out Sale))
            {
                return false;
            }
            if (!data.Context.Require(data.Target, out Store) ||
                !Store.InUse ||
                Store.HasBeenCopied ||
                Store.Price != Sale.Price ||
                Store.ApplianceID != Blueprint.Appliance)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            data.Context.Destroy(Holder.HeldItem);
            data.Context.Set(data.Interactor, default(CItemHolder));
            Store.HasBeenCopied = true;
            data.Context.Set(data.Target, Store);
        }
    }
}
