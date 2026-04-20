
gm.events.add('client.warehouse.open', (data) => {
    if (global.menuCheck()) return;
    mp.gui.emmit(`window.router.setView("PlayerWarehouse", '${data}')`);
    global.menuOpen();
});

gm.events.add('client.warehouse.close', () => {
    closeWarehouse();
});

gm.events.add('warehouse', (act, jsonData) => {
    let data = JSON.parse(jsonData);
    switch (act) {
        case "buy":
            mp.events.callRemote("server.warehouse.buy", data.buildingId, data.slot);
            break;
        case "enter":
            mp.events.callRemote("server.warehouse.enter", data.unitId);
            closeWarehouse();
            break;
        case "sell":
            mp.events.callRemote("server.warehouse.sell", data.unitId);
            break;
        case "close":
            closeWarehouse();
            break;
    }
});

function closeWarehouse() {
    global.menuClose();
    mp.gui.emmit(`window.router.close();`);
    setTimeout(() => {
        mp.gui.emmit(`window.router.setHud();`);
    }, 50);
}
