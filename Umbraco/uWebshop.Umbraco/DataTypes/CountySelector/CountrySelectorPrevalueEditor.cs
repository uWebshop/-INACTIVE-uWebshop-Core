using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.CountySelector
{
	public class PrevalueEditor : PlaceHolder, IDataPrevalue
	{
		public void Save()
		{
		}

		public Control Editor
		{
			get { return this; }
		}
	}
}