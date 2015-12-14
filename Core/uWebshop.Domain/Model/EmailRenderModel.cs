using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Model
{
	public class EmailRenderModel
	{
		public int Id { get; set; }
		public IOrder Order { get; set; }
		public Domain.OrderInfo OrderInfo { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}
}