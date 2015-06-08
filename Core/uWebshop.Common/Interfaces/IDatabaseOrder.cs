namespace uWebshop.Common.Interfaces
{
	public interface IDatabaseOrder
	{
		int DatabaseId { set; }
		void SetNewSeriesId(int newSeriesId);
	}
}