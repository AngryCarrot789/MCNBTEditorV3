using MCNBTEditor.Drop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace MCNBTEditor.Editor.FileTree {
    /// <summary>
    /// The base class for an entry in a tree system. Supports both inheritance and composition (via the Set/Get/TryGet data functions).
    /// <para>
    /// This class is meant to be a view model acting as both a model and view model. Because of how WPF binding works, you're effectively
    /// forced to use sub-types of this class (e.g. physical files, zip files, etc.) in order to implement bindable properties. Otherwise,
    /// you could simply just set specific data keys via the Get/Set/TryGet data functions in order to determine what type of file
    /// an instance of <see cref="TreeEntry"/> is
    /// </para>
    /// </summary>
    public class TreeEntry : BaseViewModel {
        private readonly ObservableCollection<TreeEntry> items;
        private TreeEntry parent;
        private bool isExpanded;
        private bool isExpanding;

        /// <summary>
        /// The parent file
        /// </summary>
        public TreeEntry Parent => this.parent;

        /// <summary>
        /// This tree items' child items
        /// </summary>
        public ReadOnlyObservableCollection<TreeEntry> Items { get; }

        /// <summary>
        /// Whether this item is empty, as in, has no children. This will not throw even if <see cref="CanHoldItems"/> is false
        /// </summary>
        public bool IsEmpty => this.items.Count < 1;

        /// <summary>
        /// The number of children in this item
        /// </summary>
        public int ItemCount => this.items.Count;

        /// <summary>
        /// Whether or not this item has been expanded at least once by the user
        /// </summary>
        public bool HasExpandedOnce { get; private set; }

        /// <summary>
        /// Whether or not this item is currently expanded and therefore the contents are loaded
        /// </summary>
        public bool IsExpanded {
            get => this.isExpanded;
            set {
                if (this.isExpanded == value) {
                    return;
                }

                if (this.isExpanding) {
                    this.RaisePropertyChanged(ref this.isExpanded, false);
                    return;
                }

                if (value) {
                    this.OnExpandInternal();
                }
                else {
                    this.RaisePropertyChanged(ref this.isExpanded, false);
                }
            }
        }

        /// <summary>
        /// Whether or not this file is currently being "expanded" (as in, browsed to in a tree)
        /// </summary>
        public bool IsExpanding {
            get => this.isExpanding;
            private set => this.RaisePropertyChanged(ref this.isExpanding, value);
        }

        /// <summary>
        /// Whether or not this tree item can hold child tree items (stored in <see cref="Items"/>). Adding files when this is false will throw
        /// </summary>
        public virtual bool CanHoldItems => false;

        /// <summary>
        /// The file system associated with this entry. Will be null for the absolute tree root (which just acts as a container for entries)
        /// </summary>
        public TreeFileSystem FileSystem { get; set; }

        /// <summary>
        /// The explorer associated with this tree entry. This is used to process things such as navigation, deletion, etc.
        /// </summary>
        public FileTree Explorer { get; set; }

        private readonly Dictionary<string, object> dataKeys;
        private bool isLazilyLoaded;

        public bool IsRoot => this.parent == null; // FileSystem == null == true

        public TreeEntry() {
            this.dataKeys = new Dictionary<string, object>();
            this.items = new ObservableCollection<TreeEntry>();
            this.Items = new ReadOnlyObservableCollection<TreeEntry>(this.items);
            this.items.CollectionChanged += this.OnChildrenCollectionChanged;
        }

        protected virtual void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.ItemCount));
            this.RaisePropertyChanged(nameof(this.IsEmpty));
        }

        public T GetData<T>(string key) {
            return this.TryGetData(key, out T value) ? value : throw new Exception("No such data with the key: " + key);
        }

        public T GetData<T>(string key, T def) {
            return this.TryGetData(key, out T value) ? value : def;
        }

        public bool TryGetData<T>(string key, out T value) {
            if (this.dataKeys.TryGetValue(key, out object o)) {
                value = (T) o;
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(string key) {
            return this.dataKeys.ContainsKey(key);
        }

        public void SetData(string key, object value) {
            if (this.OnDataChanging(key, value)) {
                if (value == null) {
                    this.dataKeys.Remove(key);
                }
                else {
                    this.dataKeys[key] = value;
                }

                this.OnDataChanged(key, value);
            }
        }

        /// <summary>
        /// Called when <see cref="SetData"/> is invoked
        /// </summary>
        /// <param name="key">The data key</param>
        /// <param name="newValue">The data value</param>
        /// <returns>True to allow the data to change, false to stop the data changing</returns>
        protected virtual bool OnDataChanging(string key, object newValue) {
            return true;
        }

        /// <summary>
        /// Called after the underlying data map is modified via a call to <see cref="SetData"/>
        /// </summary>
        /// <param name="key">The data key</param>
        /// <param name="value">The data value</param>
        protected virtual void OnDataChanged(string key, object value) {

        }

        private async void OnExpandInternal() {
            this.IsExpanding = true;
            try {
                this.isExpanded = await this.OnExpandAsync();
            }
            finally {
                // hope this doesn't throw... that would be tragic
                this.IsExpanding = false;
            }

            if (this.HasExpandedOnce) {
                this.RaisePropertyChanged(nameof(this.IsExpanded));
            }
            else {
                this.HasExpandedOnce = true;
                this.RaisePropertyChanged(nameof(this.IsExpanded));
                this.RaisePropertyChanged(nameof(this.HasExpandedOnce));
            }
        }

        protected virtual async Task<bool> OnExpandAsync() {
            if (this.CanHoldItems && this.FileSystem != null) {
                if (!this.isLazilyLoaded) {
                    this.isLazilyLoaded = true;
                    await this.FileSystem.QueryContent(this);
                }

                return this.items.Count > 0;
            }

            return false;
        }

        /// <summary>
        /// Refreshes this item, causing any data to be reloaded
        /// </summary>
        public virtual Task RefreshAsync() {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when this item has been added to a new parent item. <see cref="Parent"/> will not be null
        /// </summary>
        protected virtual void OnAddedToParent() {

        }

        /// <summary>
        /// Called before an item is about to be removed from its parent. <see cref="Parent"/> will remain the same as it was before this call
        /// </summary>
        protected virtual void OnRemovingFromParent() {
            if (this.CanHoldItems) {
                this.ClearItemsRecursiveInternal();
            }
        }

        /// <summary>
        /// Called after an item was fully removed from its parent
        /// </summary>
        /// <param name="parent">The previous parent</param>
        protected virtual void OnRemovedFromParent(TreeEntry parent) {

        }

        public void AddItemCore(TreeEntry item) {
            this.InsertItemCore(this.items.Count, item);
        }

        public void AddItemsCore(IEnumerable<TreeEntry> enumerable) {
            this.EnsureCanHoldItems();
            int i = this.items.Count;
            foreach (TreeEntry file in enumerable) {
                this.InsertItemInternal(i++, file);
            }
        }

        public void InsertItemCore(int index, TreeEntry item) {
            this.EnsureCanHoldItems();
            this.InsertItemInternal(index, item);
        }

        private void InsertItemInternal(int index, TreeEntry item) {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.parent != null)
                throw new Exception("Item already exists in another parent item");
            else if (this.IsPartOfParentHierarchy(item))
                throw new Exception("Cannot add an parent to itself");

            item.parent = this;
            this.items.Insert(index, item);
            item.RaisePropertyChanged(nameof(item.Parent));
            item.OnAddedToParent();
        }

        public bool RemoveItemCore(TreeEntry item) {
            this.EnsureCanHoldItems();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            int index = this.items.IndexOf(item);
            if (index == -1)
                return false;
            this.RemoveItemAtInternal(index);
            return true;
        }

        public void RemoveItemAtCore(int index) {
            this.EnsureCanHoldItems();
            this.RemoveItemAtInternal(index);
        }

        private void RemoveItemAtInternal(int index) {
            TreeEntry item = this.items[index];
            if (item.parent != this)
                throw new Exception("Expected item's parent to equal the current instance");

            item.OnRemovingFromParent();
            item.parent = null;
            this.items.RemoveAt(index);
            item.RaisePropertyChanged(nameof(item.Parent));
            item.OnRemovedFromParent(this);
        }

        public void ClearItemsRecursiveCore() {
            this.EnsureCanHoldItems();
            this.ClearItemsRecursiveInternal();
        }

        private void ClearItemsRecursiveInternal() {
            for (int i = this.items.Count - 1; i >= 0; i--) {
                TreeEntry item = this.items[i];
                if (item.CanHoldItems) {
                    item.ClearItemsRecursiveInternal();
                }

                this.RemoveItemAtInternal(i);
            }
        }

        private void EnsureCanHoldItems() {
            if (!this.CanHoldItems)
                throw new Exception("This item cannot hold child items");
        }

        private bool IsPartOfParentHierarchy(TreeEntry item) {
            for (TreeEntry src = this; src != null; src = src.parent) {
                if (src == item)
                    return true;
            }

            return false;
        }
    }
}