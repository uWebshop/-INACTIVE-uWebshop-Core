using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.Ranges
{
	public class RangesDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		#region FormElements

		private Label _lblRangeFrom;
		private TextBox _txtRangeFrom;
		private Label _lblRangeTo;
		private TextBox _txtRangeTo;
		private Label _lblPrice;
		private TextBox _txtPrice;
		private ListBox _lbRanges;
		private Button _btnAdd;
		private Button _btnRemove;
		private Button _btnEdit;

		private Table _Table;
		private TableRow _tableRow1;
		private TableCell _tR1Cell1;
		private TableCell _tR1Cell2;
		private TableCell _tR1Cell3;
		private TableCell _tR1Cell4;

		private TableRow _tableRow2;
		private TableCell _tR2Cell1;
		private TableCell _tR2Cell2;

		private TableRow _tableRow3;
		private TableCell _tR3Cell1;
		private TableCell _tR3Cell2;

		#endregion

		public RangesDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _lbRanges.Items.Cast<ListItem>().Aggregate(string.Empty, (current, i) => current + (i.Value + "#")).TrimEnd('#');
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

			#region Arrange GUI

			// ----------------------------------------------
			// |From:  | txtBox |              |  TEXTAREA   
			// ------------------             --            
			// |To:    | txtBox | ADD + REMOVE |        
			// ------------------             --
			// |Price: | txtBox |              |
			// ---------------------------------------------

			_Table = new Table();

			_tableRow1 = new TableRow();
			_tR1Cell1 = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tR1Cell2 = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tR1Cell3 = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tR1Cell4 = new TableCell {VerticalAlign = VerticalAlign.Middle, RowSpan = 3, Width = 260};

			_tableRow2 = new TableRow();
			_tR2Cell1 = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tR2Cell2 = new TableCell {VerticalAlign = VerticalAlign.Middle};

			_tableRow3 = new TableRow();
			_tR3Cell1 = new TableCell {VerticalAlign = VerticalAlign.Middle};
			_tR3Cell2 = new TableCell {VerticalAlign = VerticalAlign.Middle};


			_Table.Rows.Add(_tableRow1);
			_Table.Rows.Add(_tableRow2);
			_Table.Rows.Add(_tableRow3);

			var rangeFromText = library.GetDictionaryItem("RangeFrom");
			if (string.IsNullOrEmpty(rangeFromText))
			{
				rangeFromText = "Range From";
			}

			_lblRangeFrom = new Label {Text = rangeFromText};
			_txtRangeFrom = new TextBox();

			_tR1Cell1.Controls.Add(_lblRangeFrom);
			_tR1Cell2.Controls.Add(_txtRangeFrom);

			var rangeToText = library.GetDictionaryItem("RangeTo");
			if (string.IsNullOrEmpty(rangeToText))
			{
				rangeToText = "Range To";
			}

			_lblRangeTo = new Label {Text = rangeToText};
			_txtRangeTo = new TextBox();

			_tR2Cell1.Controls.Add(_lblRangeTo);
			_tR2Cell2.Controls.Add(_txtRangeTo);

			var priceText = library.GetDictionaryItem("Price");
			if (string.IsNullOrEmpty(priceText))
			{
				priceText = "Price";
			}

			_lblPrice = new Label {Text = priceText};
			_txtPrice = new TextBox();

			_tR3Cell1.Controls.Add(_lblPrice);
			_tR3Cell2.Controls.Add(_txtPrice);

			_lbRanges = new ListBox {SelectionMode = ListSelectionMode.Single, Rows = 5, Width = 250};

			_lbRanges.Items.Clear();

			var addText = library.GetDictionaryItem("Add");
			if (string.IsNullOrEmpty(addText))
			{
				addText = "Add";
			}

			_btnAdd = new Button {Text = addText};
			_btnAdd.Click += BtnAddClick;

			var removeText = library.GetDictionaryItem("Remove");
			if (string.IsNullOrEmpty(removeText))
			{
				removeText = "Remove";
			}

			_btnRemove = new Button {Text = removeText};
			_btnRemove.Click += BtnRemoveClick;

			var editText = library.GetDictionaryItem("Edit");
			if (string.IsNullOrEmpty(editText))
			{
				editText = "Edit";
			}

			_btnEdit = new Button {Text = editText};
			_btnEdit.Click += BtnEditClick;

			_tR1Cell3.Controls.Add(_btnAdd);
			_tR1Cell3.Controls.Add(_btnEdit);
			_tR1Cell3.Controls.Add(_btnRemove);

			_tR1Cell4.Controls.Add(_lbRanges);

			_tableRow1.Cells.Add(_tR1Cell1);
			_tableRow1.Cells.Add(_tR1Cell2);
			_tableRow1.Cells.Add(_tR1Cell3);
			_tableRow1.Cells.Add(_tR1Cell4);

			_tableRow2.Cells.Add(_tR2Cell1);
			_tableRow2.Cells.Add(_tR2Cell2);

			_tableRow3.Cells.Add(_tR3Cell1);
			_tableRow3.Cells.Add(_tR3Cell2);

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_Table);

			#endregion

			//if (_data.Value.ToString().Split('#').Length <= 0) return;

			if (!string.IsNullOrEmpty(_data.Value.ToString()))
			{
				_lbRanges.Items.AddRange(_data.Value.ToString().Split('#').Select(i => new Range(i).ToListItem()).ToArray());
			}

			SortItems();
			Save();
		}

		private void BtnRemoveClick(object sender, EventArgs e)
		{
			if (_lbRanges.Items.Count > 0)
				if (_lbRanges.SelectedItem != null)
					_lbRanges.Items.Remove(_lbRanges.SelectedItem);

			SortItems();
			Save();
		}

		private void BtnAddClick(object sender, EventArgs e)
		{
			var amount = string.Empty;
			float price;
			if (_data != null && float.TryParse(_txtPrice.Text, out price))
			{
				amount = Math.Round(price*100).ToString();
			}

			var range = new Range(_txtRangeFrom.Text, _txtRangeTo.Text, amount);

			if (!_lbRanges.Items.Cast<ListItem>().Select(item => new Range(item.Value)).Contains(range))
				_lbRanges.Items.Add(range.ToListItem());

			SortItems();
			Save();
		}

		private void BtnEditClick(object sender, EventArgs e)
		{
			if (_lbRanges.Items.Count > 0)
				if (_lbRanges.SelectedItem != null)
				{
					var range = new Range(_lbRanges.SelectedValue);
					_txtRangeFrom.Text = range.From.ToString();
					_txtRangeTo.Text = range.To == 0 ? "" : range.To.ToString();

					decimal price;
					decimal.TryParse(range.Price.ToString(), out price);
					var value = price/100m;

					_txtPrice.Text = value.ToString("F");
					_lbRanges.Items.Remove(_lbRanges.SelectedItem);
				}
		}

		private int CompareListItems(ListItem li1, ListItem li2)
		{
			var range1 = new Range(li1.Value);
			var range2 = new Range(li2.Value);

			return range1.From != range2.From ? range1.From - range2.From : (range1.To != range2.To ? range1.To - range2.To : range1.Price - range2.Price);
		}

		private void SortItems()
		{
			if (_lbRanges.Items.Count <= 1) return;
			var items = _lbRanges.Items.Cast<ListItem>().ToList();

			var compare = new Comparison<ListItem>(CompareListItems);
			items.Sort(compare);

			_lbRanges.Items.Clear();
			_lbRanges.Items.AddRange(items.ToArray());
		}
	}

	internal class Range
	{
		public int From;
		public int To;
		public int Price;

		public Range(string source)
		{
			var temp = source.Split('|');
			if (temp.Length < 3) return;
			int.TryParse(temp[0], out From);
			int.TryParse(temp[1], out To);
			if (temp[1] == "*") To = 0;
			int.TryParse(temp[2], out Price);
		}

		public Range(string from, string to, string price)
		{
			int.TryParse(from, out From);
			int.TryParse(to, out To);
			if (to == "*") To = 0;
			int.TryParse(price, out Price);
		}

		public override string ToString()
		{
			return string.Format("{0}|{1}|{2}", From, To, Price);
		}

		public string ToDisplayString()
		{
			return string.Format("{0}-{1} -> {2}", From, To == 0 ? "*" : To.ToString(), (Price/100m).ToString("F2"));
		}

		public ListItem ToListItem()
		{
			return new ListItem {Text = ToDisplayString(), Value = ToString()};
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Range) obj);
		}

		protected bool Equals(Range other)
		{
			return From == other.From && To == other.To && Price == other.Price;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = From;
				hashCode = (hashCode*397) ^ To;
				hashCode = (hashCode*397) ^ Price;
				return hashCode;
			}
		}
	}
}