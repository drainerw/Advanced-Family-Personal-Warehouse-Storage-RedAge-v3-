<script>
    import { executeClient } from 'api/rage'

    export let viewData;

    let data = typeof viewData === 'string' ? JSON.parse(viewData) : viewData;
    let selectedSlot = null;


    const cdnUrl = window.cloud ? window.cloud + 'images/warehouses/' : 'images/warehouses/';
    const defaultIcon = `data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 96 72"><rect fill="%23333" width="96" height="72"/><rect fill="%23444" x="8" y="18" width="80" height="46" rx="2"/><rect fill="%23555" x="14" y="24" width="18" height="12"/><rect fill="%23555" x="38" y="24" width="18" height="12"/><rect fill="%23555" x="62" y="24" width="18" height="12"/><path d="M4 18L48 4L92 18" stroke="%23555" stroke-width="2" fill="none"/></svg>`;

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

<style src="./style.css"></style>

<div class="overlay" on:click|self={close}>
    <div class="panel">
        <div class="header">
            <div class="photo" style="background-image: url('{cdnUrl}{data.id}.jpg'), url('{defaultIcon}')"></div>
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
