using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.Currencies
{
	public class CurrenciesDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		#region FormElements

		private Label _lblCurrencies;
		private DropDownList _dlCurrencies;
		private Label _lblPriceIndex;
		private TextBox _txtPriceIndex;
		private ListBox _lbCurrencies;
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

		#endregion

		public CurrenciesDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = string.Join("#", _lbCurrencies.Items.Cast<ListItem>().Select(li => li.Value));
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
			// |Code:     | Dropdown |              |  TEXTAREA   
			// ------------------             --            
			// |Index:    | txtBox   | ADD + REMOVE |       
			// ------------------             --
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

			_Table.Rows.Add(_tableRow1);
			_Table.Rows.Add(_tableRow2);

			var lblCurrencyValue = library.GetDictionaryItem("CurrencyCode");
			if (string.IsNullOrEmpty(lblCurrencyValue))
			{
				lblCurrencyValue = "Currency Code";
			}

			_lblCurrencies = new Label {Text = lblCurrencyValue};

			_dlCurrencies = new DropDownList();

			_tR1Cell1.Controls.Add(_lblCurrencies);
			_tR1Cell2.Controls.Add(_dlCurrencies);

			var lblPriceIndexValue = library.GetDictionaryItem("PriceIndex");
			if (string.IsNullOrEmpty(lblPriceIndexValue))
			{
				lblPriceIndexValue = "Price Index";
			}

			_lblPriceIndex = new Label {Text = lblPriceIndexValue};
			_txtPriceIndex = new TextBox();

			_tR2Cell1.Controls.Add(_lblPriceIndex);
			_tR2Cell2.Controls.Add(_txtPriceIndex);

			_lbCurrencies = new ListBox {SelectionMode = ListSelectionMode.Single, Rows = 5, Width = 250};

			_lbCurrencies.Items.Clear();

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

			_tR1Cell4.Controls.Add(_lbCurrencies);

			_tableRow1.Cells.Add(_tR1Cell1);
			_tableRow1.Cells.Add(_tR1Cell2);
			_tableRow1.Cells.Add(_tR1Cell3);
			_tableRow1.Cells.Add(_tR1Cell4);

			_tableRow2.Cells.Add(_tR2Cell1);
			_tableRow2.Cells.Add(_tR2Cell2);


			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_Table);

			#endregion

			FillWithISOCurrencySymbols(_dlCurrencies);

			//if (_data.Value.ToString().Split('#').Length <= 0) return;

			foreach (var item in _data.Value.ToString().Split('#').Select(c => new CurrencyCode(c).ToListItem()))
			{
				_lbCurrencies.Items.Add(item);
			}
			SortItems();
			Save();
		}

		private static IEnumerable<RegionInfo> GetAllCurrencyRegions()
		{
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(c => new RegionInfo(c.LCID)).Distinct(ri => ri.ISOCurrencySymbol).OrderBy(ri => ri.ISOCurrencySymbol);
		}

		private static IEnumerable<RegionInfo> GetImportantCurrencyregions()
		{
			var allRegions = GetAllCurrencyRegions().ToDictionary(c => c.ISOCurrencySymbol, c => c);

			return new[] {"EUR", "USD", "GBP", "DKK", "AUD", "NZD", "CHF", "CAD", "JPY"}.Select(c => allRegions[c]);
		}

		private IEnumerable<CurrencyCode> Values()
		{
			return _lbCurrencies.Items.Cast<ListItem>().Select(li => new CurrencyCode(li.Value));
		}

		/// <summary>
		/// Fills the ListControl with ISO currency symbols.
		/// </summary>
		/// <param name="dropDownList">The ListControl.</param>
		public static void FillWithISOCurrencySymbols(DropDownList dropDownList)
		{
			// put 'major' currencies on top of the list
			var currencies = GetImportantCurrencyregions().Union(GetAllCurrencyRegions());
			foreach (var currencyRegion in currencies)
			{
				dropDownList.Items.Add(new ListItem(currencyRegion.ISOCurrencySymbol + " (" + currencyRegion.CurrencySymbol + " - " + currencyRegion.CurrencyEnglishName + ")", currencyRegion.ISOCurrencySymbol));
			}

			var spacerItem = new ListItem("----------", "NONE");
			spacerItem.Attributes.Add("disabled", "disabled");
			dropDownList.Items.Insert(9, spacerItem);
		}

		private void BtnRemoveClick(object sender, EventArgs e)
		{
			if (_lbCurrencies.Items.Count > 0)
				if (_lbCurrencies.SelectedItem != null)
					_lbCurrencies.Items.Remove(_lbCurrencies.SelectedItem);

			SortItems();
			Save();
		}

		private void BtnAddClick(object sender, EventArgs e)
		{
			var currencyCode = _dlCurrencies.SelectedItem.Value;
			var priceIndex = _txtPriceIndex.Text;
			var currency = new CurrencyCode(currencyCode, priceIndex);

			var existing = _lbCurrencies.Items.Cast<ListItem>().FirstOrDefault(li => li.Value.StartsWith(currencyCode));
			if (existing != null)
			{
				_lbCurrencies.Items.Remove(existing);
			}

			_lbCurrencies.Items.Add(currency.ToListItem());

			SortItems();
			Save();
		}

		private void BtnEditClick(object sender, EventArgs e)
		{
			var selectedVal = _lbCurrencies.SelectedValue;
			if (!string.IsNullOrEmpty(selectedVal))
			{
				var cc = new CurrencyCode(selectedVal);
				_txtPriceIndex.Text = cc.PriceIndex.ToString();
				_dlCurrencies.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = false);
				_dlCurrencies.Items.FindByValue(cc.Code).Selected = true;
			}
		}

		private void SortItems()
		{
			if (_lbCurrencies.Items.Count <= 1) return;
			var items = _lbCurrencies.Items.Cast<ListItem>().ToList();

			var orderedItems = items.OrderBy(x => x.Value);

			_lbCurrencies.Items.Clear();
			_lbCurrencies.Items.AddRange(orderedItems.ToArray());
		}
	}

	internal static class Extensions
	{
		public static IEnumerable<T> Distinct<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> predicate)
		{
			var distinctKeys = new HashSet<TCompare>();
			foreach (var item in items)
			{
				var key = predicate(item);
				if (distinctKeys.Contains(key)) continue;
				distinctKeys.Add(key);
				yield return item;
			}
		}
	}

	internal class CurrencyCode
	{
		public string Code;
		public double PriceIndex;

		public CurrencyCode(string source)
		{
			var temp = source.Split('|');
			if (temp.Length < 2) return;
			Code = temp[0];
			double.TryParse(temp[1], out PriceIndex);
		}

		public CurrencyCode(string code, string priceIndex)
		{
			Code = code;
			double.TryParse(priceIndex, out PriceIndex);
		}

		public override string ToString()
		{
			return string.Format("{0}|{1}", Code, PriceIndex);
		}

		public string ToDisplayString()
		{
			return string.Format("{0} ({1})", Code, PriceIndex);
		}

		public ListItem ToListItem()
		{
			return new ListItem {Text = ToDisplayString(), Value = ToString()};
		}
	}
}