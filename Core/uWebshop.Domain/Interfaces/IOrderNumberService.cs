namespace uWebshop.Domain.Interfaces
{
	interface IOrderNumberService
	{
		void GenerateAndPersistOrderNumber(OrderInfo order);
		IOrderNumberTransaction GetTransaction(OrderInfo order);
	}
}