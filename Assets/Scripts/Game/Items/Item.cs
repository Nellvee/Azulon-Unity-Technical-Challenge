using System;
using System.Collections;
namespace Project.Items
{
    /// <summary>
    /// Runtime object for Items
    /// </summary>
    [Serializable]
    public class Item : IItem
    {

        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        public event Action<int> OnCountChanged;
        /// <summary>
        /// for now useless. I don't want to give access to change data of a runtime item. (maybe in future)
        /// </summary>
        public event Action<IItemData> OnDataChanged; 
        /// <summary>
        /// Will fire on any changes in Item.
        /// </summary>
        public event Action<IItem> OnItemChanged;

        /// <summary>
        /// Invokes <see cref="OnItemChanged"/>
        /// </summary>
        protected virtual void NotifyItemChanged()
        {
            OnItemChanged?.Invoke(this);
        }

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        private IItemData _data;
        private int _count;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        public IItemData Data => _data;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (_count == value) return;
                _count = value;
                OnCountChanged?.Invoke(_count);
                //any changed in item will call this.
                NotifyItemChanged();
            }
        }

        // ──────────────────────────────
        // Constructors
        // ──────────────────────────────

        public Item(IItemData data, int count)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _count = count;
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        public virtual IItem Clone()
        {
            return new Item(Data, Count);
        }
        public bool Equals(IItem other)
        {
            return Data != null
                && other != null && other.Data != null
                && Data.Id == other.Data.Id
                ;
        }

        public override string ToString()
        {
            return $"Item(Data({Data}), Count({Count}))";
        }
    }
}