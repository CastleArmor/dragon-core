namespace Dragon.Core
{
    public enum SlotOccupationMode
    {
        Override, //Whatever is on the slot gets removed, and new one gets added.
        Additive //Added on on top, but when override comes along, it will be removed.
    }
}