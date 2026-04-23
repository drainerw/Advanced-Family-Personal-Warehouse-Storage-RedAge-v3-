В : src_cef/src/views/player/menu/elements/inventory/inventory.svelte

<!-- 1. Ищем условие отрисовки прогресс-бара инвентаря ("Вес", "Вместимость") -->
<!-- Обычно это около 2166 строки. Ищем: -->
{#if OtherInfo.Id === otherType.Organization || OtherInfo.Id === otherType.Fraction || OtherInfo.Id === otherType.Vehicle

<!-- ДОБАВЛЯЕМ: -->
(OtherInfo.Id === otherType.Storage || OtherInfo.Id === otherType.PublicWarehouse)


<!-- ПОЛУЧАЕТСЯ ТАК : -->
{#if OtherInfo.Id === otherType.Organization || OtherInfo.Id === otherType.Fraction || OtherInfo.Id === otherType.Vehicle || OtherInfo.Id === otherType.Safe || OtherInfo.Id === otherType.Storage || OtherInfo.Id === otherType.PublicWarehouse}
