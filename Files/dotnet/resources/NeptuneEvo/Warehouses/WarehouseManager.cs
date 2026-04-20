using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Players;
using NeptuneEvo.Warehouses.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NeptuneEvo.Warehouses
{
    class WarehouseManager : Script
    {
        private static readonly nLog Log = new nLog("Warehouses.Manager");

        [Command("reloadwarehouses")]
        public static void CMD_ReloadWarehouses(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null || characterData.AdminLVL < 8) return;
            LoadWarehouses();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Склады успешно перезагружены", 3000);
        }
        public static List<WarehouseBuilding> Warehouses = new List<WarehouseBuilding>();
        public static Dictionary<int, WarehouseUnit> Units = new Dictionary<int, WarehouseUnit>();

        private static List<Marker> Markers = new List<Marker>();
        private static List<TextLabel> Labels = new List<TextLabel>();
        private static List<ExtColShape> ColShapes = new List<ExtColShape>();

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            try
            {
                LoadWarehouses();
            }
            catch (Exception e)
            {
                Log.Write($"OnResourceStart Exception: {e.ToString()}");
            }
        }

        private static readonly Vector3 ExitPosition = new Vector3(1048.0, -3097.0, -40.0);
        private static readonly Vector3 StoragePosition = new Vector3(1060.0, -3100.0, -40.0);

        public static void LoadWarehouses()
        {
            foreach (var marker in Markers) if (marker.Exists) marker.Delete();
            foreach (var label in Labels) if (label.Exists) label.Delete();
            foreach (var shape in ColShapes) CustomColShape.DeleteColShape(shape);

            Markers.Clear();
            Labels.Clear();
            ColShapes.Clear();
            Warehouses.Clear();
            Units.Clear();

            DataTable result = MySQL.QueryRead("SELECT * FROM `public_warehouses`") ?? new DataTable();
            foreach (DataRow row in result.Rows)
            {
                var building = new WarehouseBuilding
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = Convert.ToString(row["name"]),
                    Address = Convert.ToString(row["address"]),
                    Price = Convert.ToInt32(row["price"]),
                    Capacity = Convert.ToInt32(row["capacity"]),
                    TotalUnits = Convert.ToInt32(row["total_units"]),
                    Position = JsonConvert.DeserializeObject<Vector3>(Convert.ToString(row["pos"])),
                    InteriorPos = JsonConvert.DeserializeObject<Vector3>(Convert.ToString(row["interior_pos"]))
                };

                
                ColShapes.Add(CustomColShape.CreateCylinderColShape(building.Position, 1.5f, 2f, 0, ColShapeEnums.PublicWarehouse, building.Id, 0));
                Markers.Add(NAPI.Marker.CreateMarker(1, building.Position - new Vector3(0, 0, 0.9), new Vector3(), new Vector3(), 1.5f, new Color(255, 165, 0, 150), false, 0));
                Labels.Add(NAPI.TextLabel.CreateTextLabel($"~o~{building.Name}", building.Position + new Vector3(0, 0, 0.5), 5f, 0.5f, 0, new Color(255, 255, 255), true, 0));

                Warehouses.Add(building);
            }

            result = MySQL.QueryRead("SELECT * FROM `public_warehouse_units`") ?? new DataTable();
            foreach (DataRow row in result.Rows)
            {
                var unit = new WarehouseUnit
                {
                    Id = Convert.ToInt32(row["id"]),
                    WarehouseId = Convert.ToInt32(row["warehouse_id"]),
                    SlotNumber = Convert.ToInt32(row["slot_number"]),
                    OwnerUuid = row["owner_uuid"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["owner_uuid"]),
                    FamilyId = row["family_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["family_id"]),
                    Locked = Convert.ToBoolean(row["locked"])
                };

                var building = Warehouses.FirstOrDefault(w => w.Id == unit.WarehouseId);
                if (building != null)
                {
                    building.Units[unit.SlotNumber] = unit;
                    Units[unit.Id] = unit;
                }
            }

            // Выход
            ColShapes.Add(CustomColShape.CreateCylinderColShape(ExitPosition, 1.5f, 2f, uint.MaxValue, ColShapeEnums.PublicWarehouse, 0, 1));
            Markers.Add(NAPI.Marker.CreateMarker(1, ExitPosition, new Vector3(), new Vector3(), 1.0f, new Color(255, 165, 0, 150), false, uint.MaxValue));
            Labels.Add(NAPI.TextLabel.CreateTextLabel("~o~Выход", ExitPosition + new Vector3(0, 0, 0.5), 5f, 0.5f, 0, new Color(255, 255, 255), true, uint.MaxValue));

            // Склад
            ColShapes.Add(CustomColShape.CreateCylinderColShape(StoragePosition, 1.5f, 2f, uint.MaxValue, ColShapeEnums.PublicWarehouse, 0, 2));
            Markers.Add(NAPI.Marker.CreateMarker(1, StoragePosition, new Vector3(), new Vector3(), 1.0f, new Color(255, 165, 0, 150), false, uint.MaxValue));
            Labels.Add(NAPI.TextLabel.CreateTextLabel("~o~Склад", StoragePosition + new Vector3(0, 0, 0.5), 5f, 0.5f, 0, new Color(255, 255, 255), true, uint.MaxValue));
            
            Log.Write($"Loaded {Warehouses.Count} warehouses and {Units.Count} units.");
        }

        [Interaction(ColShapeEnums.PublicWarehouse)]
        public static void Interact(ExtPlayer player, int buildingId, int state)
        {
            try 
            {
                if (state == 1) 
                {
                    ExitUnit(player);
                    return;
                }
                if (state == 2) 
                {
                    OpenStorage(player);
                    return;
                }

                var building = Warehouses.FirstOrDefault(w => w.Id == buildingId);
                if (building == null) return;

                var unitsData = new List<object>();
                for (int i = 1; i <= building.TotalUnits; i++)
                {
                    if (building.Units.ContainsKey(i))
                    {
                        var unit = building.Units[i];
                        unitsData.Add(new
                        {
                            id = unit.Id,
                            slot = unit.SlotNumber,
                            owner = unit.OwnerUuid,
                            family = unit.FamilyId,
                            isMine = (unit.OwnerUuid == player.GetUUID()),
                            isFree = unit.IsFree
                        });
                    }
                    else
                    {
                        unitsData.Add(new { slot = i, isFree = true });
                    }
                }

                Trigger.ClientEvent(player, "client.warehouse.open", JsonConvert.SerializeObject(new
                {
                    id = building.Id,
                    name = building.Name,
                    address = building.Address,
                    price = building.Price,
                    capacity = building.Capacity,
                    units = unitsData
                }));
            }
            catch (Exception e)
            {
                Log.Write($"Interact Exception: {e.ToString()}");
            }
        }

        public static void ExitUnit(ExtPlayer player)
        {
            if (player.Dimension <= baseDimension) return;
            int unitId = (int)(player.Dimension - baseDimension);
            
            if (!Units.ContainsKey(unitId)) 
            {
                player.Dimension = 0;
                player.Position = new Vector3(899.12f, -2985.34f, 5.9f); 
                return;
            }

            var unit = Units[unitId];
            var building = Warehouses.FirstOrDefault(w => w.Id == unit.WarehouseId);
            if (building == null) return;

            player.Position = building.Position;
            player.Dimension = 0;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы вышли на улицу", 3000);
        }

        public static void OpenStorage(ExtPlayer player)
        {
            if (player.Dimension <= baseDimension) return;
            int unitId = (int)(player.Dimension - baseDimension);

            if (!Units.ContainsKey(unitId)) return;
            var unit = Units[unitId];
            
            if (unit.OwnerUuid != player.GetUUID()) 
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет доступа к этому складу", 3000);
                return;
            }

            // ЯЧЕЙКИ(СЛОТЫ) 800 МОЖНО МЕНЯТЬ
            Chars.Repository.LoadOtherItemsData(player, "publicwarehouse", $"Склад #{unitId}", 13, 800);
        }

        [RemoteEvent("server.warehouse.buy")]
        public static void BuyUnit(ExtPlayer player, int buildingId, int slot)
        {
            try
            {
                var building = Warehouses.FirstOrDefault(w => w.Id == buildingId);
                if (building == null) return;

                if (building.Units.ContainsKey(slot))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта ячейка уже занята", 3000);
                    return;
                }

                // Лимит  (1 игрок = 1 склад)
                if (Units.Values.Any(u => u.OwnerUuid == player.GetUUID()))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже владеете одной ячейкой!", 3000);
                    return;
                }

                if (UpdateData.CanIChange(player, building.Price, true) == 255)
                {
                    Wallet.Change(player, -building.Price);
                    
                    int unitId = Convert.ToInt32(MySQL.QueryRead($"INSERT INTO `public_warehouse_units` (`warehouse_id`, `slot_number`, `owner_uuid`, `locked`) VALUES ({buildingId}, {slot}, {player.GetUUID()}, 1); SELECT LAST_INSERT_ID();").Rows[0][0]);

                    var unit = new WarehouseUnit
                    {
                        Id = unitId,
                        WarehouseId = buildingId,
                        SlotNumber = slot,
                        OwnerUuid = player.GetUUID(),
                        Locked = true
                    };

                    building.Units[slot] = unit;
                    Units[unitId] = unit;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно купили ячейку №{slot}", 3000);
                    Trigger.ClientEvent(player, "client.warehouse.close"); 
                }
            }
            catch (Exception e)
            {
                Log.Write($"BuyUnit Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.warehouse.enter")]
        public static void EnterUnit(ExtPlayer player, int unitId)
        {
            if (!Units.ContainsKey(unitId)) return;
            var unit = Units[unitId];
            var building = Warehouses.FirstOrDefault(w => w.Id == unit.WarehouseId);
            if (building == null) return;

            if (unit.Locked && unit.OwnerUuid != player.GetUUID())
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ячейка закрыта", 3000);
                return;
            }

            player.Position = building.InteriorPos;
            player.Dimension = (uint)(baseDimension + unit.Id);
            
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы вошли в ячейку №{unit.SlotNumber}", 3000);
        }

        private const uint baseDimension = 10000;

        [RemoteEvent("server.warehouse.sell")]
        public static void SellUnit(ExtPlayer player, int unitId)
        {
            try
            {
                if (!Units.ContainsKey(unitId)) return;
                var unit = Units[unitId];

                if (unit.OwnerUuid != player.GetUUID())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это не ваша ячейка", 3000);
                    return;
                }

                var building = Warehouses.FirstOrDefault(w => w.Id == unit.WarehouseId);
                if (building == null) return;

                int sellPrice = building.Price / 2;
                Wallet.Change(player, sellPrice);

                MySQL.Query($"DELETE FROM `public_warehouse_units` WHERE `id` = {unitId}");

                building.Units.Remove(unit.SlotNumber);
                Units.Remove(unitId);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали ячейку №{unit.SlotNumber} за ${sellPrice}", 3000);
                Interact(player, building.Id, 0);
            }
            catch (Exception e)
            {
                Log.Write($"SellUnit Exception: {e.ToString()}");
            }
        }
    }
}
