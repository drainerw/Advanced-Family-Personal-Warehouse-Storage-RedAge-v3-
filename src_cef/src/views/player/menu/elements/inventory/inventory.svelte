В : src_cef/src/views/player/menu/elements/inventory/inventory.svelte

<!-- 1. Ищем условие отрисовки прогресс-бара инвентаря ("Вес", "Вместимость") -->
<!-- Обычно это около 2164 строки. Ищем: -->
<div class="inventory__text"><span class="{otherName[OtherInfo.Id].icon} inventory__icon"></span>{otherName[OtherInfo.Id].descr}</div>

<!-- ДОБАВЛЯЕМ ПОСЛЕ </div>: -->
                                </div>
                                {#if OtherInfo.Id === otherType.Organization || OtherInfo.Id === otherType.Fraction || OtherInfo.Id === otherType.Vehicle || OtherInfo.Id === otherType.Safe || OtherInfo.Id === otherType.Storage || OtherInfo.Id === otherType.PublicWarehouse}
                                    <div class="box-column box-control" style="margin-left: 10px; min-width: 120px;">
                                        <div class="box-flex inventory__text" style="justify-content: flex-end; margin-bottom: 5px;">
                                            <span class="inventoryicons-open-box inventory__icon" style="margin-right: 5px;"></span>
                                            {ItemsData["other"].filter(item => item.ItemId !== 0).length} / {ItemsData["other"].length}
                                        </div>
                                        <div class="weapon__progress" style="width: 100%; height: 4px; background: rgba(255,255,255,0.1); border-radius: 2px; overflow: hidden;">
                                            <div class="weapon__progress-line" style="width: {(ItemsData["other"].filter(item => item.ItemId !== 0).length / ItemsData["other"].length) * 100}%; height: 100%; background: #CDF15C; transition: width 0.3s ease;"></div>
                                        </div>


<!-- ГОТОВЫЙ ТАК : -->
                        </div> -->
                        <div class="box-other" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            
                            <div class="box-between height-box">
                                <div class="box-column height-box">
                                    <div class="inventory__title">{otherName[OtherInfo.Id].name}</div>
                                    <div class="inventory__text"><span class="{otherName[OtherInfo.Id].icon} inventory__icon"></span>{otherName[OtherInfo.Id].descr}</div>
                                </div>
                                {#if OtherInfo.Id === otherType.Organization || OtherInfo.Id === otherType.Fraction || OtherInfo.Id === otherType.Vehicle || OtherInfo.Id === otherType.Safe || OtherInfo.Id === otherType.Storage || OtherInfo.Id === otherType.PublicWarehouse}
                                    <div class="box-column box-control" style="margin-left: 10px; min-width: 120px;">
                                        <div class="box-flex inventory__text" style="justify-content: flex-end; margin-bottom: 5px;">
                                            <span class="inventoryicons-open-box inventory__icon" style="margin-right: 5px;"></span>
                                            {ItemsData["other"].filter(item => item.ItemId !== 0).length} / {ItemsData["other"].length}
                                        </div>
                                        <div class="weapon__progress" style="width: 100%; height: 4px; background: rgba(255,255,255,0.1); border-radius: 2px; overflow: hidden;">
                                            <div class="weapon__progress-line" style="width: {(ItemsData["other"].filter(item => item.ItemId !== 0).length / ItemsData["other"].length) * 100}%; height: 100%; background: #CDF15C; transition: width 0.3s ease;"></div>
                                        </div>
                                    </div>
                                {/if}
                            </div>
                            <div class="box-list">
                                {#each ItemsData["other"] as item, index}
                                    <Slot
                                        key={index}
                                        item={item}
                                        iconInfo={window.getItem (item.ItemId)}
                                        on:mousedown={(event) => handleMouseDown(event, index, "other")}
                                        on:mouseup={handleSlotMouseUp}
                                        on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                                        on:mouseleave={handleSlotMouseLeave} />
                                {/each}
