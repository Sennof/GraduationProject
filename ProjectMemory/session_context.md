# Контекст сессии

**Дата:** 2026-04-20

**Текущая задача:** Реализация системы физических ценников (Price Tags) — инструмент создания/размещения, ценники на полках, влияние на AI-покупателей.

**Последний изменённый файл:** `Assets/Scripts/Infrastructure/Global/EntryPoint.cs`

## Ключевые решения

### Архитектура системы ценников
- `PriceTag` (MonoBehaviour) — физический ценник: `Configure(product, markup)` → `AttachToShelf(shelf, point)` / `Detach()`.
- `Shelf` расширен: `Dictionary<ProductData, PriceTag> _priceTags` + массив `Transform[] _priceTagAttachPoints`. Метод `GetProductMarkup(data)` возвращает локальную наценку (ценник), если есть, иначе — глобальную из `GlobalStatsBridge`.
- `AICustomer.FollowShelvesRoutine()` теперь использует `targetShelf.GetProductMarkup(data)` вместо прямого обращения к `GlobalStatsBridge` — покупатели реагируют на ценники.
- `ShelfSlot.TryGetItem()` проверяет `data.IsSellable` — предметы с `false` не берутся покупателями.

### Инструмент PriceTagMaker
- Паттерн идентичен `PenKnife`/`BuildingWrench`: `[RequireComponent(typeof(InteractingObject))]`, инициализация через `PriceTagMakerInitializer` + EventBus.
- `InteractingObject._triggerKey` должен быть `KeyCode.None` в prefab-е (инструмент управляет вводом сам).
- **F** — открыть/закрыть `UIPriceTagCreator`, **E** — прикрепить ценник к полке (raycast), **Q** — листать сконфигурированные теги.
- `CreatePriceTagsRequestEvent` содержит `TargetMaker` — сообщение адресное, не глобальное.

### PriceTagRoll (расходник)
- Прост: держишь в руках + **F** → `GetOtherSlotItemManager()` из `Inventory` → если там `PriceTagMaker` → `RefillCapacity()` + `DestroySlot()`.
- `Inventory` расширен одним методом `GetOtherSlotItemManager()`.

### Новые геттеры
- `InteractingObject.GetIsInHands()` — используется в `PriceTagMaker.Update()`.
- `InteractingObject.GetRaycastStartPoint()` — для будущего использования.

### Файловая структура новых классов
- `Infrastructure/PriceTags/` — `PriceTag.cs`, `PriceTagHanger.cs`
- `Infrastructure/Initializers/` — `PriceTagMakerInitializer.cs`
- `Gameplay/Items/` — `PriceTagMaker.cs`, `PriceTagRoll.cs`
- `Gameplay/UI/PriceTags/` — `UIPriceTagCreator.cs`, `UIPriceTagHangerPanel.cs`

## Следующие шаги

1. **Unity Editor — обязательная настройка:**
   - Создать `PriceTag` prefab (mesh + TextMeshPro для названия и цены).
   - Создать `PriceTagMaker` prefab (ItemObject + InteractingObject с `_triggerKey = None` + PriceTagMaker компонент).
   - Создать `PriceTagRoll` prefab (ItemObject + PriceTagRoll компонент).
   - Добавить на сцену объект с `PriceTagMakerInitializer`, назначить Camera и Inventory.
   - На существующих prefab-ах Shelf добавить дочерние Transform-ы и назначить их в `_priceTagAttachPoints[]`.

2. **Добавить `PriceTagRoll` в систему доставки** (`UIShopSideMenu` / `DeliveryManager`) как заказываемый товар.

3. **Добавить `PriceTagHanger` prefab** — объект-держатель для хранения ценников на стене (Interactable → `OpenPanel()`).

4. **Тестирование:** проверить что AICustomer реагирует на ценник (Too expensive / Great price) при наценке выше/ниже допустимой для класса.

5. **Опционально:** отображать в UI PriceTagMaker HUD информацию о текущем выбранном теге (`GetSelectedTagInfo()`).
