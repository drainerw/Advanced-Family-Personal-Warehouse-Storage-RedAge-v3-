using GTANetworkAPI;
using System.Collections.Generic;

namespace NeptuneEvo.Warehouses.Models
{
    public class WarehouseBuilding
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Price { get; set; }
        public int Capacity { get; set; }
        public int TotalUnits { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 InteriorPos { get; set; }
        
        public Dictionary<int, WarehouseUnit> Units { get; set; } = new Dictionary<int, WarehouseUnit>();
    }
}
