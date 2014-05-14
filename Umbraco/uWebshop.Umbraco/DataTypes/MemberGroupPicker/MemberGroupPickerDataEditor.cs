using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.MemberGroupPicker
{
	public class MemberGroupPickerDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private Label _lblAllItems;
		private ListBox _lbAllItems;

		private Label _lblSelectedItems;
		private ListBox _lbSelectedItems;

		private Button _btnMoveLeft;
		private Button _btnMoveRight;

		private Table _table;
		private TableRow _tableRow;
		private TableCell _tCellFirst;
		private TableCell _tCellSecond;
		private TableCell _tCellThird;

		public MemberGroupPickerDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			var memberGroups = _lbSelectedItems.Items.Cast<ListItem>();

			var csvGroupData = string.Join(",", memberGroups.Select(x => x.ToString()).ToArray());

			if (_data != null) _data.Value = csvGroupData;
		}

		public bool ShowLabel
		{
			get { return true; }
		}

		public bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		public Control Editor
		{
			get { return this; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			_table = new Table();

			_tableRow = new TableRow();

			_tCellFirst = new TableCell {VerticalAlign = VerticalAlign.Top, Width = 260};
			_tableRow.Cells.Add(_tCellFirst);
			_tCellSecond = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tableRow.Cells.Add(_tCellSecond);
			_tCellThird = new TableCell {VerticalAlign = VerticalAlign.Top, Width = 260};
			_tableRow.Cells.Add(_tCellThird);

			_table.Rows.Add(_tableRow);


			var items = GetAllMemberGroups().Select(memberGroup => new ListItem(memberGroup, memberGroup)).OrderBy(x => x.Text).ToList();
			var selectedItems = GetSelectedMemberGroups().Select(memberGroup => new ListItem(memberGroup, memberGroup)).OrderBy(x => x.Text).ToList();

			foreach (var selectedItem in selectedItems)
			{
				if (items.Contains(selectedItem))
				{
					items.Remove(selectedItem);
				}
			}

			_lbAllItems = new ListBox {SelectionMode = ListSelectionMode.Multiple, Width = 250, Rows = 10};
			foreach (var item in items)
			{
				_lbAllItems.Items.Add(new ListItem(item.Text, item.Value));
			}
			_lbAllItems.DataBind();

			var allItemsText = library.GetDictionaryItem("AllItems");
			if (string.IsNullOrEmpty(allItemsText))
			{
				allItemsText = "All items";
			}

			_lblAllItems = new Label {Text = allItemsText, AssociatedControlID = _lbAllItems.ID};


			var addText = library.GetDictionaryItem("Add");
			if (string.IsNullOrEmpty(addText))
			{
				addText = "Add";
			}

			var removeText = library.GetDictionaryItem("Remove");
			if (string.IsNullOrEmpty(removeText))
			{
				removeText = "Remove";
			}

			_tCellFirst.Controls.Add(_lblAllItems);
			_tCellFirst.Controls.Add(_lbAllItems);

			_btnMoveLeft = new Button {Text = removeText};
			_btnMoveLeft.Click += BtnMoveLeftClick;

			_btnMoveRight = new Button {Text = addText};
			_btnMoveRight.Click += BtnMoveRightClick;

			_tCellSecond.Controls.Add(_btnMoveLeft);
			_tCellSecond.Controls.Add(_btnMoveRight);


			_lbSelectedItems = new ListBox {SelectionMode = ListSelectionMode.Multiple, Width = 250, Rows = 10};
			foreach (var selectedItem in selectedItems)
			{
				_lbSelectedItems.Items.Add(new ListItem(selectedItem.Text, selectedItem.Value));
			}
			_lbSelectedItems.DataBind();

			var selectedItemsText = library.GetDictionaryItem("SelectedItems");
			if (string.IsNullOrEmpty(selectedItemsText))
			{
				selectedItemsText = "Selected items";
			}

			_lblSelectedItems = new Label {Text = selectedItemsText, AssociatedControlID = _lbSelectedItems.ID};

			_tCellThird.Controls.Add(_lblSelectedItems);
			_tCellThird.Controls.Add(_lbSelectedItems);


			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_table);
		}

		private void BtnMoveLeftClick(object sender, EventArgs e)
		{
			var itemsToAdd = _lbSelectedItems.Items.Cast<ListItem>().Where(item => item.Selected).ToList();

			foreach (var item in itemsToAdd)
			{
				_lbAllItems.Items.Add(item);
				_lbSelectedItems.Items.Remove(item);
			}

			var items = _lbAllItems.Items.Cast<ListItem>().ToList();

			var compare = new Comparison<ListItem>(CompareListItems);
			items.Sort(compare);
			_lbAllItems.Items.Clear();
			_lbAllItems.Items.AddRange(items.ToArray());
		}

		private void BtnMoveRightClick(object sender, EventArgs e)
		{
			var itemsToAdd = _lbAllItems.Items.Cast<ListItem>().Where(item => item.Selected).ToList();

			foreach (var item in itemsToAdd)
			{
				_lbSelectedItems.Items.Add(item);
				_lbAllItems.Items.Remove(item);
			}

			var items = _lbSelectedItems.Items.Cast<ListItem>().ToList();

			var compare = new Comparison<ListItem>(CompareListItems);
			items.Sort(compare);
			_lbSelectedItems.Items.Clear();
			_lbSelectedItems.Items.AddRange(items.ToArray());

			Save();
		}

		private static int CompareListItems(ListItem li1, ListItem li2)
		{
			return string.CompareOrdinal(li1.Text, li2.Text);
		}

		private IEnumerable<string> GetSelectedMemberGroups()
		{
			var memberGroups = new List<string>();

			var value = _data.Value.ToString();

			if (!string.IsNullOrEmpty(value))
			{
				if (value.Contains(','))
				{
					memberGroups.AddRange(value.Split(','));
				}
				else
				{
					memberGroups.Add(value);
				}
			}

			return memberGroups;
		}

		private static IEnumerable<string> GetAllMemberGroups()
		{
			return Roles.GetAllRoles().ToList();
		}
	}
}