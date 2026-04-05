using System.Collections.Generic;
using UnityEngine;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    public class BackpackGrid
    {
        private int _width, _height;
        private PlacedItem[,] _occupancy;
        private List<PlacedItem> _items;

        public int Width  => _width;
        public int Height => _height;

        public BackpackGrid(int width, int height)
        {
            Resize(width, height);
        }

        public void Resize(int width, int height)
        {
            _width  = Mathf.Max(1, width);
            _height = Mathf.Max(1, height);
            _occupancy = new PlacedItem[_width, _height];
            _items = new List<PlacedItem>();
        }

        public List<PlacedItem> SimulateResize(int newW, int newH)
        {
            var temp = new BackpackGrid(newW, newH);
            var overflow = new List<PlacedItem>();
            foreach (var pi in _items)
            {
                if (!temp.TryPlaceFirstFit(pi.item, pi.quantity, pi.rotated))
                    overflow.Add(pi);
            }
            return overflow;
        }

        public List<PlacedItem> ResizeWithItems(int newW, int newH)
        {
            var oldItems = new List<PlacedItem>(_items);
            Resize(newW, newH);
            var overflow = new List<PlacedItem>();
            foreach (var pi in oldItems)
            {
                if (!TryPlaceFirstFit(pi.item, pi.quantity, pi.rotated))
                    overflow.Add(pi);
            }
            return overflow;
        }

        public bool CanPlace(ItemDefinition item, int x, int y, bool rotated)
        {
            if (item == null) return false;
            int w = rotated ? item.GridHeight : item.GridWidth;
            int h = rotated ? item.GridWidth  : item.GridHeight;
            if (x < 0 || y < 0) return false;
            if (x + w > _width || y + h > _height) return false;
            for (int cx = x; cx < x + w; cx++)
                for (int cy = y; cy < y + h; cy++)
                    if (_occupancy[cx, cy] != null) return false;
            return true;
        }

        private void DoPlace(ItemDefinition item, int x, int y, bool isRotated, int quantity)
        {
            var pi = new PlacedItem(item, x, y, isRotated, quantity);
            _items.Add(pi);
            int w = pi.EffectiveWidth;
            int h = pi.EffectiveHeight;
            for (int cx = x; cx < x + w; cx++)
                for (int cy = y; cy < y + h; cy++)
                    _occupancy[cx, cy] = pi;
        }

        public bool TryPlace(ItemDefinition item, int x, int y, bool isRotated, int quantity)
        {
            if (!CanPlace(item, x, y, isRotated)) return false;
            DoPlace(item, x, y, isRotated, quantity);
            return true;
        }

        public int TryMergeAmount(ItemDefinition item, int amount)
        {
            if (item == null || !item.Stackable || amount <= 0) return 0;
            int merged = 0;
            foreach (var pi in _items)
            {
                if (merged >= amount) break;
                if (pi.item != item) continue;
                int space = item.MaxStack - pi.quantity;
                if (space <= 0) continue;
                int take = Mathf.Min(space, amount - merged);
                pi.quantity += take;
                merged += take;
            }
            return merged;
        }

        public bool TryPlaceFirstFit(ItemDefinition item, int quantity, bool preferRotated = false)
        {
            if (item == null || quantity <= 0) return false;
            int remaining = quantity;
            int stackSize = item.Stackable ? item.MaxStack : 1;

            while (remaining > 0)
            {
                int placedQty = Mathf.Min(remaining, stackSize);
                bool placed = false;

                bool[] orientations = preferRotated && item.Rotatable
                    ? new[] { true, false }
                    : new[] { false, true };

                foreach (bool rot in orientations)
                {
                    if (rot && !item.Rotatable) continue;
                    int w = rot ? item.GridHeight : item.GridWidth;
                    int h = rot ? item.GridWidth  : item.GridHeight;
                    bool found = false;
                    int foundX = 0, foundY = 0;
                    for (int row = 0; row <= _height - h && !found; row++)
                    {
                        for (int col = 0; col <= _width - w && !found; col++)
                        {
                            if (CanPlace(item, col, row, rot))
                            {
                                foundX = col; foundY = row;
                                found = true;
                            }
                        }
                    }
                    if (found)
                    {
                        DoPlace(item, foundX, foundY, rot, placedQty);
                        remaining -= placedQty;
                        placed = true;
                        break;
                    }
                }

                if (!placed) return false;
            }
            return true;
        }

        public PlacedItem GetItemAt(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height) return null;
            return _occupancy[x, y];
        }

        public bool RemovePlaced(PlacedItem pi)
        {
            if (pi == null) return false;
            if (!_items.Remove(pi)) return false;
            for (int cx = 0; cx < _width; cx++)
                for (int cy = 0; cy < _height; cy++)
                    if (_occupancy[cx, cy] == pi) _occupancy[cx, cy] = null;
            return true;
        }

        public PlacedItem RemoveItemAt(int x, int y)
        {
            var pi = GetItemAt(x, y);
            if (pi == null) return null;
            RemovePlaced(pi);
            return pi;
        }

        public bool RemoveAmount(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            if (CountItem(item) < amount) return false;
            int remaining = amount;
            var toRemove = new List<PlacedItem>();
            foreach (var pi in _items)
            {
                if (remaining <= 0) break;
                if (pi.item != item) continue;
                int take = Mathf.Min(remaining, pi.quantity);
                pi.quantity -= take;
                remaining -= take;
                if (pi.quantity <= 0) toRemove.Add(pi);
            }
            foreach (var pi in toRemove)
                RemovePlaced(pi);
            return remaining <= 0;
        }

        public int CountItem(ItemDefinition item)
        {
            if (item == null) return 0;
            int total = 0;
            foreach (var pi in _items)
                if (pi.item == item) total += pi.quantity;
            return total;
        }

        public bool CanFitAmount(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return true;
            int remaining = amount;

            if (item.Stackable)
            {
                foreach (var pi in _items)
                {
                    if (pi.item != item) continue;
                    remaining -= (item.MaxStack - pi.quantity);
                    if (remaining <= 0) return true;
                }
            }

            // Simulate placing new stacks
            var tempOccupied = new bool[_width, _height];
            foreach (var pi in _items)
            {
                int w = pi.EffectiveWidth;
                int h = pi.EffectiveHeight;
                for (int cx = pi.x; cx < pi.x + w; cx++)
                    for (int cy = pi.y; cy < pi.y + h; cy++)
                        tempOccupied[cx, cy] = true;
            }

            int stackSize = item.Stackable ? item.MaxStack : 1;
            while (remaining > 0)
            {
                bool found = false;
                int fw = item.GridWidth;
                int fh = item.GridHeight;
                for (int row = 0; row <= _height - fh && !found; row++)
                {
                    for (int col = 0; col <= _width - fw && !found; col++)
                    {
                        bool fits = true;
                        for (int cx = col; cx < col + fw && fits; cx++)
                            for (int cy = row; cy < row + fh && fits; cy++)
                                if (tempOccupied[cx, cy]) fits = false;
                        if (fits)
                        {
                            for (int cx = col; cx < col + fw; cx++)
                                for (int cy = row; cy < row + fh; cy++)
                                    tempOccupied[cx, cy] = true;
                            remaining -= stackSize;
                            found = true;
                        }
                    }
                }
                if (!found && item.Rotatable)
                {
                    int rw = item.GridHeight;
                    int rh = item.GridWidth;
                    if (rw != fw || rh != fh)
                    {
                        for (int row = 0; row <= _height - rh && !found; row++)
                        {
                            for (int col = 0; col <= _width - rw && !found; col++)
                            {
                                bool fits = true;
                                for (int cx = col; cx < col + rw && fits; cx++)
                                    for (int cy = row; cy < row + rh && fits; cy++)
                                        if (tempOccupied[cx, cy]) fits = false;
                                if (fits)
                                {
                                    for (int cx = col; cx < col + rw; cx++)
                                        for (int cy = row; cy < row + rh; cy++)
                                            tempOccupied[cx, cy] = true;
                                    remaining -= stackSize;
                                    found = true;
                                }
                            }
                        }
                    }
                }
                if (!found) return false;
            }
            return true;
        }

        public IReadOnlyList<PlacedItem> GetAllPlaced() => _items;

        public void Clear()
        {
            _items.Clear();
            _occupancy = new PlacedItem[_width, _height];
        }

        // Legacy API compatibility
        public PlacedItem GetAt(int col, int row) => GetItemAt(col, row);
        public bool TryFindFirstFit(ItemDefinition item, bool rot, out int col, out int row)
        {
            col = 0; row = 0;
            if (item == null) return false;
            int w = rot ? item.GridHeight : item.GridWidth;
            int h = rot ? item.GridWidth  : item.GridHeight;
            for (int r = 0; r <= _height - h; r++)
            {
                for (int c = 0; c <= _width - w; c++)
                {
                    if (CanPlace(item, c, r, rot))
                    {
                        col = c; row = r;
                        return true;
                    }
                }
            }
            return false;
        }

        public IReadOnlyList<PlacedItem> PlacedItems => _items;
    }
}
