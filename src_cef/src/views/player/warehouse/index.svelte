<script>
    import { executeClient } from 'api/rage'

    export let viewData;

    let data = typeof viewData === 'string' ? JSON.parse(viewData) : viewData;
    let selectedSlot = null;

    function selectSlot(unit) {
        selectedSlot = unit;
    }

    function enterSlot() {
        if (!selectedSlot || !selectedSlot.isMine) return;
        executeClient('warehouse', 'enter', JSON.stringify({ unitId: selectedSlot.id }));
    }

    function sellSlot() {
        if (!selectedSlot || !selectedSlot.isMine) return;
        executeClient('warehouse', 'sell', JSON.stringify({ unitId: selectedSlot.id }));
    }

    function buySlot() {
        if (!selectedSlot || !selectedSlot.isFree) return;
        executeClient('warehouse', 'buy', JSON.stringify({ buildingId: data.id, slot: selectedSlot.slot }));
    }

    function close() {
        window.router.close();
    }

    function fmt(n) {
        return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    }
</script>

<div class="overlay" on:click|self={close}>
    <div class="panel">
        <div class="header">
            <div class="photo"></div>
            <div class="info">
                <div class="name">{data.name}</div>
                <div class="addr">{data.address}</div>
                <div class="line"><i>Помещений:</i> <b>{data.units.length}</b> &nbsp; <i>Свободно:</i> <b>{data.units.filter(u => u.isFree).length}</b></div>
                <div class="line"><i>Вместимость:</i> <b class="green">{fmt(data.capacity)} <span class="kg">КГ</span></b> &nbsp; <i>Стоимость:</i> <b>${fmt(data.price)}</b></div>
            </div>
            <div class="x" on:click={close}>✕</div>
        </div>
        <div class="sep"></div>
        <div class="grid-area">
            {#each data.units as u (u.slot)}
                <div class="c"
                    class:mine={u.isMine}
                    class:free={u.isFree}
                    class:taken={!u.isFree && !u.isMine}
                    class:sel={selectedSlot && selectedSlot.slot === u.slot}
                    on:click={() => selectSlot(u)}
                >
                    <b>{u.slot}</b>
                    <span>{u.isMine ? 'Мой' : u.isFree ? 'На продаже' : 'Куплен'}</span>
                </div>
            {/each}
        </div>
        <div class="sep"></div>
        <div class="foot">
            {#if selectedSlot && selectedSlot.isMine}
                <div class="fbtn enter" on:click={enterSlot}>Войти</div>
                <div class="fbtn sell" on:click={sellSlot}>Продать</div>
                <div class="fbtn closebtn" on:click={close}>Закрыть</div>
            {:else if selectedSlot && selectedSlot.isFree}
                <div class="fbtn buy" on:click={buySlot}>Купить за ${fmt(data.price)}</div>
                <div class="fbtn closebtn" on:click={close}>Закрыть</div>
            {:else}
                <div class="fbtn enter dim">Войти</div>
                <div class="fbtn closebtn" on:click={close}>Закрыть</div>
            {/if}
        </div>
    </div>
</div>

<style>
    *{box-sizing:border-box;margin:0;padding:0}
    .overlay{position:fixed;inset:0;display:flex;align-items:center;justify-content:center;z-index:5000;font-family:'Inter','Segoe UI',sans-serif}
    .panel{background:#1e1e1e;border-radius:10px;width:560px;color:#fff;overflow:hidden}
    .header{display:flex;gap:14px;padding:18px 20px;position:relative}
    .photo{width:96px;height:72px;border-radius:6px;flex-shrink:0;background:#333;background-image:url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 96 72"><rect fill="%23333" width="96" height="72"/><rect fill="%23444" x="8" y="18" width="80" height="46" rx="2"/><rect fill="%23555" x="14" y="24" width="18" height="12"/><rect fill="%23555" x="38" y="24" width="18" height="12"/><rect fill="%23555" x="62" y="24" width="18" height="12"/><path d="M4 18L48 4L92 18" stroke="%23555" stroke-width="2" fill="none"/></svg>');background-size:cover}
    .info{flex:1;min-width:0}
    .name{font-size:17px;font-weight:700;margin-bottom:1px}
    .addr{font-size:12px;color:#888;margin-bottom:6px}
    .line{font-size:12px;color:#888;margin-bottom:3px}
    .line i{font-style:italic}
    .line b{color:#fff;font-weight:600}
    .line b.green{color:#4caf50}
    .kg{font-size:9px;background:rgba(76,175,80,0.2);color:#4caf50;padding:1px 3px;border-radius:2px;font-weight:600}
    .x{position:absolute;top:16px;right:18px;color:#555;font-size:16px;cursor:pointer;line-height:1}
    .x:hover{color:#aaa}
    .sep{height:1px;background:#2a2a2a}
    .grid-area{display:grid;grid-template-columns:repeat(5,1fr);gap:5px;padding:14px 18px}
    .c{display:flex;flex-direction:column;align-items:center;justify-content:center;aspect-ratio:1;border-radius:5px;cursor:pointer;border:2px solid transparent;user-select:none}
    .c b{font-size:26px;font-weight:800;line-height:1.1}
    .c span{font-size:10px;font-weight:500}

    .c.taken{background:#2a2a2a}
    .c.taken b{color:#ccc}
    .c.taken span{color:#666}

    .c.free{background:#2e7d32}
    .c.free b{color:#fff}
    .c.free span{color:rgba(255,255,255,0.7)}

    .c.mine{background:#c2185b}
    .c.mine b{color:#fff}
    .c.mine span{color:rgba(255,255,255,0.75)}

    .c.sel{border-color:#fff}

    .foot{display:flex;align-items:center;padding:14px 18px;gap:8px}
    .fbtn{padding:11px 24px;border-radius:6px;font-size:14px;font-weight:500;cursor:pointer;user-select:none;text-align:center}
    .fbtn.enter{background:none;color:#fff}
    .fbtn.enter.dim{color:#444;cursor:default}
    .fbtn.sell{background:#4a1a1a;color:#e57373;flex:0 0 auto}
    .fbtn.sell:hover{background:#5a2020}
    .fbtn.buy{background:#2e7d32;color:#fff;flex:1}
    .fbtn.buy:hover{background:#388e3c}
    .fbtn.closebtn{flex:1;background:#2a2a2a;color:#777}
    .fbtn.closebtn:hover{color:#bbb}
</style>
