using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace ListWithLists
{
    [Activity(Label = "ListWithLists", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private RecyclerView recyclerView;
        private ListWithListAdapter listAdapter;
        int count = 10;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            InitializeRecyclerView();
            ButtonStuff();
        }

        private void InitializeRecyclerView()
        {
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            listAdapter = new ListWithListAdapter();
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            recyclerView.SetAdapter(listAdapter);

            AddAdapterData();
        }

        private void AddAdapterData()
        {
            var itemSource = new List<ScrollListData>();

            for (int i = 0; i < count; i++)
            {
                var list = new ScrollListData();
                itemSource.Add(list);
                for (int x = 0; x < i + 1; x++)
                {
                    list.Items.Add("Item " + x);
                }
            }

            listAdapter.ItemSource = itemSource;
        }

        private class SimpleListAdapter : RecyclerView.Adapter
        {
            private ScrollListData _itemSource;

            public ScrollListData ItemSource
            {
                get
                {
                    return _itemSource;
                }

                set
                {
                    _itemSource = value;
                    NotifyDataSetChanged();
                }
            }

            public override int ItemCount
            {
                get
                {
                    var itemCount = 0;

                    itemCount = ItemSource.Items.Count;

                    return itemCount;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is CellHolder cellHolder)
                {
                    cellHolder.PopulateWithData(Resource.Drawable.notification_template_icon_bg);
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var cellView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CellLayout, parent, false);
                var cellHolder = new CellHolder(cellView);

                return cellHolder;
            }
        }

        private class ListWithListAdapter : RecyclerView.Adapter
        {
            private const int HEADER_ITEM = 0;
            private const int CELL_ITEM = 1;

            private List<ScrollListData> _itemSource;
            private List<ListInfo> listInfos;

            public ListWithListAdapter()
            {
                ItemSource = new List<ScrollListData>();
            }

            public List<ScrollListData> ItemSource
            {
                get { return _itemSource; }
                set
                {
                    _itemSource = value;
                    SetListInfo();

                    NotifyDataSetChanged();
                }
            }

            public override int ItemCount
            {
                get
                {
                    var itemCount = 0;

                    foreach (var list in ItemSource)
                    {
                        itemCount++;
                        itemCount++;
                    }

                    return itemCount;
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is HeaderHolder headerHolder)
                {
                    var listInfo = listInfos[position];
                    var groupIndex = listInfo.GroupIndex;
                    headerHolder.PopulateWithData("Group " + groupIndex);
                }

                if (holder is CellWithList cellHolder)
                {
                    var listInfo = listInfos[position];
                    var groupIndex = listInfo.GroupIndex;

                    cellHolder.PopulateWithData(ItemSource[groupIndex]);
                }
            }

            public override void OnViewRecycled(Object holder)
            {
                base.OnViewRecycled(holder);

                if (holder is CellWithList cellHolder)
                {
                    var scrollX = cellHolder.GetScroll();
                    var position = cellHolder.AdapterPosition;

                    var itemPosition = GetPosition(position);
                    ItemSource[itemPosition].ScrollOffset = scrollX;

                    System.Diagnostics.Debug.WriteLine($"Position {position}, scroll is {scrollX}");
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == HEADER_ITEM)
                {
                    var headerView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.HeaderLayout, parent, false);
                    var headerHolder = new HeaderHolder(headerView);

                    return headerHolder;
                }

                var cellView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CellWithListLayout, parent, false);
                var cellHolder = new CellWithList(cellView);

                return cellHolder;
            }

            public override int GetItemViewType(int position)
            {
                if (listInfos[position].IsHeader)
                {
                    return HEADER_ITEM;
                }

                return CELL_ITEM;
            }

            private int GetPosition(int rawPosition)
            {
                return (int)rawPosition / 2;
            }

            private void SetListInfo()
            {
                listInfos = new List<ListInfo>();
                for (int groupIndex = 0; groupIndex < ItemSource.Count; groupIndex++)
                {
                    listInfos.Add(new ListInfo() { IsHeader = true, GroupIndex = groupIndex });
                    listInfos.Add(new ListInfo() { IsHeader = false, GroupIndex = groupIndex });
                }
            }

            ////private void SetListInfo()
            ////{
            ////    listInfos = new List<ListInfo>();
            ////    for (int groupIndex = 0; groupIndex < ItemSource.Count; groupIndex++)
            ////    {
            ////        listInfos.Add(new ListInfo() { IsHeader = true, GroupIndex = groupIndex });
            ////        var currentGroup = ItemSource[groupIndex];
            ////        for (int itemIndex = 0; itemIndex < currentGroup.Items.Count; itemIndex++)
            ////        {
            ////            listInfos.Add(new ListInfo() { GroupIndex = groupIndex, ItemIndex = itemIndex });
            ////        }
            ////    }
            ////}
        }

        private class ListInfo
        {
            public bool IsHeader { get; set; }

            public int GroupIndex { get; set; }

            public int ItemIndex { get; set; }
        }

        private class HeaderHolder : RecyclerView.ViewHolder
        {
            private TextView _textView;

            public HeaderHolder(View itemView)
                : base(itemView)
            {
                _textView = itemView.FindViewById<TextView>(Resource.Id.HeaderTextView);
            }

            public void PopulateWithData(string header)
            {
                _textView.Text = header;
            }
        }

        private class CellHolder : RecyclerView.ViewHolder
        {
            private ImageView _imageView;
            private TextView _textView;

            public CellHolder(View itemView)
                : base(itemView)
            {
                _imageView = itemView.FindViewById<ImageView>(Resource.Id.CellImageView);
                _textView = itemView.FindViewById<TextView>(Resource.Id.CellImageUrlTextView);
            }

            public void PopulateWithData(int imageResource)
            {
                _imageView.SetImageResource(Resource.Drawable.notification_template_icon_bg);

                SetImageAsync();
                ////SetImage();

                _textView.Text = "Item " + AdapterPosition;
            }

            private void SetImage()
            {
                _imageView.SetImageResource(Resource.Mipmap.Icon);
            }

            private async void SetImageAsync()
            {
                await Task.Delay(300);

                _imageView.SetImageResource(Resource.Mipmap.Icon);
            }
        }

        private class CellWithList : RecyclerView.ViewHolder
        {
            private RecyclerView _recyclerView;
            private SimpleListAdapter _adapter;

            public CellWithList(View itemView)
                : base(itemView)
            {
                _recyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.CellRecyclerView);
                _recyclerView.SetLayoutManager(new LinearLayoutManager(itemView.Context, LinearLayoutManager.Horizontal, false));
                _adapter = new SimpleListAdapter();
                _recyclerView.SetAdapter(_adapter);
            }

            public void PopulateWithData(ScrollListData items)
            {
                _adapter.ItemSource = items;
                var layoutManager = (LinearLayoutManager)_recyclerView.GetLayoutManager();
                layoutManager.ScrollToPositionWithOffset(0, -items.ScrollOffset);
            }

            public int GetScroll()
            {
                var scrollX = _recyclerView.ComputeHorizontalScrollOffset();
                return scrollX;
            }
        }

        private void ButtonStuff()
        {
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                button.Text = $"{count++} clicks!";
                AddAdapterData();
            };
        }

        private class ScrollListData
        {
            public ScrollListData()
            {
                Items = new List<string>();
            }

            public List<string> Items { get; set; }

            public int ScrollOffset { get; set; }
        }
    }
}