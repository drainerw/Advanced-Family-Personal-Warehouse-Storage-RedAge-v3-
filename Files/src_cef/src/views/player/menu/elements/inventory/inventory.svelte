В : src_cef/src/views/player/menu/elements/inventory/inventory.svelte

<!-- 1. Ищем условие отрисовки прогресс-бара инвентаря ("Вес", "Вместимость") -->
<!-- Обычно это около 2166 строки. Ищем: -->
OtherInfo.Id === otherType.Storage

<!-- ЗАМЕНЯЕМ НА: -->
(OtherInfo.Id === otherType.Storage || OtherInfo.Id === otherType.PublicWarehouse)
