using System;

namespace NeptuneEvo.Warehouses.Models
{
    public class WarehouseUnit
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int SlotNumber { get; set; }
        public int? OwnerUuid { get; set; } = null;
        public int? FamilyId { get; set; } = null;
        public bool Locked { get; set; } = true;

        public bool IsFree => !OwnerUuid.HasValue && !FamilyId.HasValue;
        
        public string InventoryId => $"warehouse_{Id}";
    }
}
