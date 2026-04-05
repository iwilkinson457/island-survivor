namespace ExtractionDeadIsles.Inventory
{
    /// <summary>
    /// Typed equipment slots available in the player's equipment panel.
    /// Each slot accepts only items whose ItemDefinition.CompatibleEquipmentSlots includes this type.
    /// </summary>
    public enum EquipmentSlotType
    {
        Weapon1,
        Weapon2,
        Head,
        Torso,
        Legs,
        Backpack
    }
}
