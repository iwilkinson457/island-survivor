using UnityEngine;
using ExtractionDeadIsles.Interaction;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;
using ExtractionDeadIsles.UI;
using ExtractionDeadIsles.World;

namespace ExtractionDeadIsles.Building
{
    [RequireComponent(typeof(SphereCollider))]
    public class CampfireStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Use Campfire";
        [SerializeField] private float useRadius = 3f;
        [SerializeField] private ItemDefinition rawItem;
        [SerializeField] private ItemDefinition cookedItem;
        [SerializeField] private float cookDurationSeconds = 8f;

        [SerializeField] private InventorySlot inputSlot = new InventorySlot();
        [SerializeField] private InventorySlot fuelSlot = new InventorySlot();
        [SerializeField] private InventorySlot outputSlot = new InventorySlot();

        [SerializeField] private bool isLit;
        [SerializeField] private float currentFuelSecondsRemaining;
        [SerializeField] private float currentCookProgress;

        public string InteractionPrompt => prompt;
        public bool IsBurning => isLit && currentFuelSecondsRemaining > 0f;
        public bool CanLight => !IsBurning && fuelSlot.HasItem && fuelSlot.Item != null && fuelSlot.Item.IsFuel && fuelSlot.Quantity > 0;
        public float CurrentFuelSecondsRemaining => currentFuelSecondsRemaining;
        public float CurrentCookProgress => currentCookProgress;
        public float CookDurationSeconds => cookDurationSeconds;
        public ItemDefinition InputItem => inputSlot.Item;
        public int InputQuantity => inputSlot.Quantity;
        public ItemDefinition FuelItem => fuelSlot.Item;
        public int FuelQuantity => fuelSlot.Quantity;
        public ItemDefinition OutputItem => outputSlot.Item;
        public int OutputQuantity => outputSlot.Quantity;

        private void Reset()
        {
            ConfigureTriggerCollider();
        }

        private void Awake()
        {
            ConfigureTriggerCollider();
        }

        private void ConfigureTriggerCollider()
        {
            var sphere = GetComponent<SphereCollider>();
            if (sphere == null) return;
            sphere.isTrigger = true;
            sphere.radius = useRadius;
        }

        private void Update()
        {
            TickFuel(Time.deltaTime);
            TickCooking(Time.deltaTime);
        }

        public void Interact(GameObject interactor)
        {
            var panel = interactor.GetComponent<CampfirePanel>();
            if (panel == null)
                panel = interactor.AddComponent<CampfirePanel>();

            panel.Open(this);
        }

        public bool TryPlaceInputItem(ItemDefinition item, int amount)
        {
            if (!CanAcceptInput(item) || amount <= 0) return false;
            return inputSlot.TryAdd(item, amount) == 0;
        }

        public bool TryPlaceFuelItem(ItemDefinition item, int amount)
        {
            if (!CanAcceptFuel(item) || amount <= 0) return false;
            return fuelSlot.TryAdd(item, amount) == 0;
        }

        public bool TryPlaceOutputItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            if (outputSlot.HasItem && outputSlot.Item != item) return false;
            return outputSlot.TryAdd(item, amount) == 0;
        }

        public bool TryTakeInputStack(out ItemDefinition item, out int quantity)
        {
            item = inputSlot.Item;
            quantity = inputSlot.Quantity;
            if (!inputSlot.HasItem) return false;
            inputSlot.Clear();
            currentCookProgress = 0f;
            return true;
        }

        public bool TryTakeFuelStack(out ItemDefinition item, out int quantity)
        {
            item = fuelSlot.Item;
            quantity = fuelSlot.Quantity;
            if (!fuelSlot.HasItem) return false;
            fuelSlot.Clear();
            return true;
        }

        public bool TryTakeOutputStack(out ItemDefinition item, out int quantity)
        {
            item = outputSlot.Item;
            quantity = outputSlot.Quantity;
            if (!outputSlot.HasItem) return false;
            outputSlot.Clear();
            return true;
        }

        public void ConfigureCooking(ItemDefinition raw, ItemDefinition cooked)
        {
            rawItem = raw;
            cookedItem = cooked;
        }

        public bool CanAcceptInput(ItemDefinition item)
        {
            if (item == null || rawItem == null) return false;
            if (item != rawItem) return false;
            return !inputSlot.HasItem || inputSlot.Item == item;
        }

        public bool CanAcceptFuel(ItemDefinition item)
        {
            if (item == null || !item.IsFuel || item.FuelBurnSeconds <= 0f) return false;
            return !fuelSlot.HasItem || fuelSlot.Item == item;
        }

        public bool TryMoveInputFromPlayer(PlayerInventory inventory, ItemDefinition item, int amount)
        {
            if (inventory == null || item == null || amount <= 0) return false;
            if (!CanAcceptInput(item)) return false;
            if (!inventory.RemoveItems(item, amount)) return false;

            int leftover = inputSlot.TryAdd(item, amount);
            if (leftover > 0)
            {
                inventory.TryAddItem(item, leftover);
                return false;
            }

            return true;
        }

        public bool TryMoveFuelFromPlayer(PlayerInventory inventory, ItemDefinition item, int amount)
        {
            if (inventory == null || item == null || amount <= 0) return false;
            if (!CanAcceptFuel(item)) return false;
            if (!inventory.RemoveItems(item, amount)) return false;

            int leftover = fuelSlot.TryAdd(item, amount);
            if (leftover > 0)
            {
                inventory.TryAddItem(item, leftover);
                return false;
            }

            return true;
        }

        public bool TryReturnInputToPlayer(PlayerInventory inventory)
        {
            if (inventory == null || !inputSlot.HasItem) return false;
            var item = inputSlot.Item;
            int qty = inputSlot.Quantity;
            if (!inventory.TryAddItem(item, qty)) return false;
            inputSlot.Clear();
            currentCookProgress = 0f;
            return true;
        }

        public bool TryReturnFuelToPlayer(PlayerInventory inventory)
        {
            if (inventory == null || !fuelSlot.HasItem) return false;
            var item = fuelSlot.Item;
            int qty = fuelSlot.Quantity;
            if (!inventory.TryAddItem(item, qty)) return false;
            fuelSlot.Clear();
            return true;
        }

        public bool TryTakeOutputToPlayer(PlayerInventory inventory)
        {
            if (inventory == null || !outputSlot.HasItem) return false;
            var item = outputSlot.Item;
            int qty = outputSlot.Quantity;
            if (!inventory.TryAddItem(item, qty)) return false;
            outputSlot.Clear();
            return true;
        }

        public bool TryLight()
        {
            if (!CanLight) return false;
            isLit = true;
            return ConsumeNextFuelUnit();
        }

        private void TickFuel(float deltaTime)
        {
            if (!isLit) return;

            if (currentFuelSecondsRemaining > 0f)
            {
                currentFuelSecondsRemaining = Mathf.Max(0f, currentFuelSecondsRemaining - deltaTime);
                if (currentFuelSecondsRemaining > 0f)
                    return;
            }

            if (!ConsumeNextFuelUnit())
                isLit = false;
        }

        private bool ConsumeNextFuelUnit()
        {
            if (!fuelSlot.HasItem || fuelSlot.Item == null || !fuelSlot.Item.IsFuel || fuelSlot.Item.FuelBurnSeconds <= 0f)
            {
                currentFuelSecondsRemaining = 0f;
                return false;
            }

            currentFuelSecondsRemaining = fuelSlot.Item.FuelBurnSeconds;
            fuelSlot.Remove(1);
            if (!fuelSlot.HasItem)
                fuelSlot.Clear();
            return true;
        }

        private void TickCooking(float deltaTime)
        {
            if (!IsBurning) return;
            if (!inputSlot.HasItem || inputSlot.Item != rawItem || cookedItem == null) return;
            if (outputSlot.HasItem && outputSlot.Item != cookedItem) return;
            if (outputSlot.HasItem && outputSlot.Quantity >= cookedItem.MaxStack) return;

            currentCookProgress += deltaTime;
            if (currentCookProgress < cookDurationSeconds)
                return;

            currentCookProgress = 0f;
            inputSlot.Remove(1);
            outputSlot.TryAdd(cookedItem, 1);
            if (!inputSlot.HasItem)
                inputSlot.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var tracker = other.GetComponent<CampfireProximityTracker>();
            if (tracker != null)
                tracker.NotifyEnteredCampfireRange();
        }

        private void OnTriggerExit(Collider other)
        {
            var tracker = other.GetComponent<CampfireProximityTracker>();
            if (tracker != null)
                tracker.NotifyExitedCampfireRange();
        }
    }
}
