using System;
using System.Collections.Generic;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    /// <summary>
    /// 2D spatial occupancy grid for backpack storage.
    ///
    /// Supports multi-cell items, fit checks, rotation, and atomic place/remove.
    /// All mutating operations are atomic: if a placement cannot succeed the grid is
    /// left unchanged (safe-reject guarantee).
    /// </summary>
    public class BackpackGrid
    {
        // ----------------------------------------------------------------
        // Dimensions
        // ----------------------------------------------------------------

        /// <summary>Current grid width in cells.</summary>
        public int Width  { get; private set; }

        /// <summary>Current grid height in cells.</summary>
        public int Height { get; private set; }

        // ----------------------------------------------------------------
        // Internal state
        // ----------------------------------------------------------------

        // _cells[col, row]: null = empty, non-null = PlacedItem that occupies this cell
        private PlacedItem[,] _cells;

        // Canonical list of all items currently placed in this grid
        private readonly List<PlacedItem> _placedItems = new List<PlacedItem>();

        /// <summary>All items currently placed in the grid (read-only view).</summary>
        public IReadOnlyList<PlacedItem> PlacedItems => _placedItems;

        // ----------------------------------------------------------------
        // Construction
        // ----------------------------------------------------------------

        public BackpackGrid(int width, int height)
        {
            if (width  < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));
            Width  = width;
            Height = height;
            _cells = new PlacedItem[width, height];
        }

        // ----------------------------------------------------------------
        // Fit check
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns true if the item's footprint (possibly rotated) fits at (x, y) without
        /// going out of bounds or overlapping an occupied cell.
        ///
        /// Pass <paramref name="exclude"/> to treat that item's current cells as empty —
        /// useful when checking whether an already-placed item can move to a new position.
        /// </summary>
        public bool CanPlace(ItemDefinition item, int x, int y, bool rotated = false, PlacedItem exclude = null)
        {
            if (item == null) return false;
            int w = rotated ? item.GridHeight : item.GridWidth;
            int h = rotated ? item.GridWidth  : item.GridHeight;
            return FitsAt(x, y, w, h, exclude);
        }

        private bool FitsAt(int x, int y, int w, int h, PlacedItem exclude)
        {
            if (x < 0 || y < 0)              return false;
            if (x + w > Width)               return false;
            if (y + h > Height)              return false;

            for (int cx = x; cx < x + w; cx++)
            {
                for (int cy = y; cy < y + h; cy++)
                {
                    var occupant = _cells[cx, cy];
                    if (occupant != null && occupant != exclude)
                        return false;
                }
            }
            return true;
        }

        // ----------------------------------------------------------------
        // Place
        // ----------------------------------------------------------------

        /// <summary>
        /// Atomically places an item at (x, y).
        /// Returns the new <see cref="PlacedItem"/> on success, or null if the fit check fails.
        /// The grid is never partially modified on failure.
        /// </summary>
        public PlacedItem TryPlace(ItemDefinition item, int x, int y, bool rotated = false)
        {
            if (!CanPlace(item, x, y, rotated)) return null;

            int w = rotated ? item.GridHeight : item.GridWidth;
            int h = rotated ? item.GridWidth  : item.GridHeight;

            var placed = new PlacedItem(item, x, y, rotated);
            _placedItems.Add(placed);
            WriteCells(placed, x, y, w, h);
            return placed;
        }

        private void WriteCells(PlacedItem placed, int x, int y, int w, int h)
        {
            for (int cx = x; cx < x + w; cx++)
                for (int cy = y; cy < y + h; cy++)
                    _cells[cx, cy] = placed;
        }

        // ----------------------------------------------------------------
        // Remove
        // ----------------------------------------------------------------

        /// <summary>
        /// Removes a placed item from the grid and clears all cells it occupied.
        /// Returns false if <paramref name="placed"/> is not found in this grid.
        /// </summary>
        public bool TryRemove(PlacedItem placed)
        {
            if (placed == null) return false;
            if (!_placedItems.Remove(placed)) return false;
            EraseCells(placed);
            return true;
        }

        private void EraseCells(PlacedItem placed)
        {
            int w = placed.EffectiveWidth;
            int h = placed.EffectiveHeight;
            for (int cx = placed.X; cx < placed.X + w; cx++)
                for (int cy = placed.Y; cy < placed.Y + h; cy++)
                    if (_cells[cx, cy] == placed)
                        _cells[cx, cy] = null;
        }

        // ----------------------------------------------------------------
        // Move (same grid, new position/rotation)
        // ----------------------------------------------------------------

        /// <summary>
        /// Moves an already-placed item to a new (x, y) position with a potentially
        /// different rotation.  The move is atomic: on failure the item stays where it was.
        /// Returns false if the new position fails the fit check.
        /// </summary>
        public bool TryMove(PlacedItem placed, int newX, int newY, bool newRotated)
        {
            if (placed == null || !_placedItems.Contains(placed)) return false;

            // Fit check ignoring the item's own current cells
            if (!CanPlace(placed.Item, newX, newY, newRotated, exclude: placed)) return false;

            // Erase old cells
            EraseCells(placed);

            // Update position
            placed.X       = newX;
            placed.Y       = newY;
            placed.Rotated = newRotated;

            // Write new cells
            WriteCells(placed, newX, newY, placed.EffectiveWidth, placed.EffectiveHeight);
            return true;
        }

        // ----------------------------------------------------------------
        // Query helpers
        // ----------------------------------------------------------------

        /// <summary>Returns the <see cref="PlacedItem"/> occupying cell (col, row), or null if empty/OOB.</summary>
        public PlacedItem GetAt(int col, int row)
        {
            if (col < 0 || col >= Width || row < 0 || row >= Height) return null;
            return _cells[col, row];
        }

        /// <summary>Returns the first placed item matching the given definition, or null.</summary>
        public PlacedItem Find(ItemDefinition item)
        {
            foreach (var p in _placedItems)
                if (p.Item == item) return p;
            return null;
        }

        /// <summary>
        /// Scans left-to-right, top-to-bottom for the first (col, row) where the item's
        /// footprint fits.  Returns false if no valid position exists.
        /// </summary>
        public bool TryFindFirstFit(ItemDefinition item, bool rotated, out int col, out int row)
        {
            col = 0; row = 0;
            if (item == null) return false;
            int w = rotated ? item.GridHeight : item.GridWidth;
            int h = rotated ? item.GridWidth  : item.GridHeight;

            for (int r = 0; r <= Height - h; r++)
            {
                for (int c = 0; c <= Width - w; c++)
                {
                    if (FitsAt(c, r, w, h, null))
                    {
                        col = c;
                        row = r;
                        return true;
                    }
                }
            }
            return false;
        }

        // ----------------------------------------------------------------
        // Resize (backpack swap)
        // ----------------------------------------------------------------

        /// <summary>
        /// Attempts to resize the grid to new dimensions.
        /// Returns false without modifying the grid if any currently placed item would
        /// fall outside the new bounds (overflow-safe rejection).
        /// </summary>
        public bool TryResize(int newWidth, int newHeight)
        {
            if (newWidth < 1 || newHeight < 1) return false;

            foreach (var p in _placedItems)
            {
                if (p.X + p.EffectiveWidth  > newWidth)  return false;
                if (p.Y + p.EffectiveHeight > newHeight) return false;
            }

            var newCells = new PlacedItem[newWidth, newHeight];
            foreach (var p in _placedItems)
            {
                int w = p.EffectiveWidth;
                int h = p.EffectiveHeight;
                for (int cx = p.X; cx < p.X + w; cx++)
                    for (int cy = p.Y; cy < p.Y + h; cy++)
                        newCells[cx, cy] = p;
            }

            Width  = newWidth;
            Height = newHeight;
            _cells = newCells;
            return true;
        }

        // ----------------------------------------------------------------
        // Clear
        // ----------------------------------------------------------------

        /// <summary>Removes all items from the grid.</summary>
        public void Clear()
        {
            _placedItems.Clear();
            _cells = new PlacedItem[Width, Height];
        }
    }
}
