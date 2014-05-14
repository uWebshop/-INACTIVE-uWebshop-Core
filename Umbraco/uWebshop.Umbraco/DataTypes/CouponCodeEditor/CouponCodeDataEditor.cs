using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Umbraco.DataTypes.CouponCodeEditor
{
	static class CouponDisplayExtensions
	{
		public static string ToDisplayString(this ICoupon coupon)
		{
			return string.Format("{0} ({1})", coupon.CouponCode, coupon.NumberAvailable);
		}
		public static ListItem ToListItem(this ICoupon coupon)
		{
			return new ListItem { Text = ToDisplayString(coupon), Value = coupon.ToString() };
		}
	}
	public class CouponCodeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		#region FormElements

		private Label _lblCouponCode;
		private TextBox _txtCouponCode;
		private Label _lblCount;
		private TextBox _txtCount;
		private ListBox _lbCoupons;
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

		public CouponCodeDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			//if (_data != null) _data.Value = _lbCoupons.Items.Cast<ListItem>().Aggregate(string.Empty, (current, i) => current + (i.Value + "#$#"));
			var nodeIdString = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdString, out nodeId);

			IO.Container.Resolve<ICouponCodeService>().Save(nodeId, _lbCoupons.Items.Cast<ListItem>().Select(li =>
				{
					var liSplit = li.Value.Split('|');
					int numberAvailable;
					int.TryParse(liSplit[1], out numberAvailable);
					return new Couponn{ DiscountId = nodeId,  CouponCode = liSplit[0], NumberAvailable = numberAvailable };
				}));
		}

		class Couponn : ICoupon
		{
			public int DiscountId { get; set; }
			public string CouponCode { get; set; }
			public int NumberAvailable { get; set; }
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
			// |Code:  | txtBox |              |  TEXTAREA   
			// ------------------             --            
			// |Count:    | txtBox | ADD + REMOVE |        
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

			var rangeFromText = library.GetDictionaryItem("CouponCode");
			if (string.IsNullOrEmpty(rangeFromText))
			{
				rangeFromText = "Coupon Code";
			}

			_lblCouponCode = new Label {Text = rangeFromText};
			_txtCouponCode = new TextBox();

			_tR1Cell1.Controls.Add(_lblCouponCode);
			_tR1Cell2.Controls.Add(_txtCouponCode);

			var rangeToText = library.GetDictionaryItem("Count");
			if (string.IsNullOrEmpty(rangeToText))
			{
				rangeToText = "Count";
			}

			_lblCount = new Label {Text = rangeToText};
			_txtCount = new TextBox();

			_tR2Cell1.Controls.Add(_lblCount);
			_tR2Cell2.Controls.Add(_txtCount);


			_lbCoupons = new ListBox {SelectionMode = ListSelectionMode.Single, Rows = 5, Width = 250};

			_lbCoupons.Items.Clear();

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

			_tR1Cell4.Controls.Add(_lbCoupons);

			_tableRow1.Cells.Add(_tR1Cell1);
			_tableRow1.Cells.Add(_tR1Cell2);
			_tableRow1.Cells.Add(_tR1Cell3);
			_tableRow1.Cells.Add(_tR1Cell4);

			_tableRow2.Cells.Add(_tR2Cell1);
			_tableRow2.Cells.Add(_tR2Cell2);

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_Table);

			#endregion

			var nodeIdString = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdString, out nodeId);

			var coupons = IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(nodeId);

			_lbCoupons.Items.AddRange(coupons.Select(i => i.ToListItem()).ToArray());
			SortItems();
			Save();
		}

		private void BtnRemoveClick(object sender, EventArgs e)
		{
			if (_lbCoupons.Items.Count > 0)
				if (_lbCoupons.SelectedItem != null)
					_lbCoupons.Items.Remove(_lbCoupons.SelectedItem);

			SortItems();
			Save();
		}

		private void BtnAddClick(object sender, EventArgs e)
		{
			var nodeIdString = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdString, out nodeId);

			int numberAvailable = 1;
			int.TryParse(_txtCount.Text, out numberAvailable);

			var coupon = new Coupon(nodeId, _txtCouponCode.Text, numberAvailable);

			var couponAsString = coupon.ToDisplayString();

			if (!_lbCoupons.Items.Cast<ListItem>().Select(item => item.Value).Contains(couponAsString))
				_lbCoupons.Items.Add(coupon.ToListItem());

			SortItems();
			Save();
		}

		private void BtnEditClick(object sender, EventArgs e)
		{
			if (_lbCoupons.Items.Count > 0)
				if (_lbCoupons.SelectedItem != null)
				{
					var setCoupon = _lbCoupons.SelectedValue;

					var couponArray = setCoupon.Split('|');

					var nodeIdString = HttpContext.Current.Request["id"];

					int nodeId;
					int.TryParse(nodeIdString, out nodeId);

					int numberAvailable = 1;
					int.TryParse(couponArray[1], out numberAvailable);

					var range = new Coupon(nodeId, couponArray[0], numberAvailable);
					_txtCouponCode.Text = range.CouponCode;
					_txtCount.Text = range.NumberAvailable.ToString();

					_lbCoupons.Items.Remove(_lbCoupons.SelectedItem);
				}
		}

		private void SortItems()
		{
			if (_lbCoupons.Items.Count <= 1) return;
			var items = _lbCoupons.Items.Cast<ListItem>().Select(i => i.Value).ToList();

			var nodeIdString = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdString, out nodeId);

			items.Sort();

			_lbCoupons.Items.Clear();
			_lbCoupons.Items.AddRange(items.Select(li =>
				{
					var liSplit = li.Split('|');
					int numberAvailable = 1;
					int.TryParse(liSplit[1], out numberAvailable);
					return new Coupon(nodeId, liSplit[0], numberAvailable).ToListItem();
				}).ToArray());
		}
	}
}