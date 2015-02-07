using System.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Businesslogic
{
	internal class OrderEvents
	{
		public static void OrderStatusChanged(OrderInfo orderInfo, AfterOrderStatusChangedEventArgs e)
		{
			Log.Instance.LogDebug("AfterOrderStatusChanged Start");

			if (orderInfo == null || orderInfo.OrderLines.Count == 0)
				return;

			Log.Instance.LogDebug("AfterOrderStatusChanged e.OrderStatus: " + e.OrderStatus);

			Log.Instance.LogDebug("AfterOrderStatusChanged e.SendEmails: " + e.SendEmails);

			switch (e.OrderStatus)
			{
				case OrderStatus.Incomplete:
					break;
				case OrderStatus.Confirmed:
					// copy customerdata to shipping data if there is no shippingdata on the order
					OrderHelper.CopyCustomerToShipping(orderInfo);

					if (orderInfo.OrderNodeId == 0)
					{
						if (orderInfo.CustomerInfo.CustomerInformation != null)
						{
							string customerEmail = null, customerFirstName = null, customerLastName = null;
							foreach (var customerProperty in IO.Container.Resolve<ICMSDocumentTypeService>().GetByAlias(Order.NodeAlias).Properties.Where(x => x.Alias.StartsWith("customer")))
							{
								var element = orderInfo.CustomerInfo.CustomerInformation.Element(customerProperty.Alias);
								if (element != null)
								{
									if (element.Name == "customerEmail")
									{
										customerEmail = element.Value;
									}
									else if (element.Name == "customerFirstName")
									{
										customerFirstName = element.Value;
									}
									else if (element.Name == "customerLastName")
									{
										customerLastName = element.Value;
									}
								}
							}
							IO.Container.Resolve<IOrderRepository>().SetCustomerInfo(orderInfo.UniqueOrderId, customerEmail, customerFirstName, customerLastName);
						}

						// remove guid from cookie and expire it immediately
						//OrderHelper.RemoveOrderCookie(orderInfo);
						// set completed order id cookie to have access to old order after payment
						OrderHelper.SetCompletedOrderCookie(orderInfo);

						// update stock when no payment provider is chosen
						if (orderInfo.PaymentInfo.Id == 0 || orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OfflinePaymentInStore || orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OfflinePaymentAtCustomer || orderInfo.PaymentInfo.PaymentType == PaymentProviderType.Unknown)
						{
							OrderHelper.UpdateStock(orderInfo);
						}
					}

					#region send emails

					if (e.SendEmails)
					{
						int customerEmailNodeId;

						int.TryParse(orderInfo.StoreInfo.Store.ConfirmationEmailCustomer, out customerEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.ConfirmationEmailStore, out storeEmailNodeId);


						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Confirmed e.SendEmails == false");
					}

					if (orderInfo.PaymentInfo.Id == 0 && orderInfo.Status != OrderStatus.Confirmed)
					{
						orderInfo.Status = OrderStatus.Confirmed;
					}

					if (orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OfflinePaymentAtCustomer || orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OfflinePaymentInStore)
					{
						orderInfo.Status = OrderStatus.OfflinePayment;
					}

					// if online payment set status to waiting for payment
					if (orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OnlinePayment)
					{
						orderInfo.Status = OrderStatus.WaitingForPayment;

						if (orderInfo.PaymentInfo.TransactionMethod == PaymentTransactionMethod.Inline)
						{
							var paymentProvider =
								PaymentProviderHelper.GetAllPaymentProviders()
									.FirstOrDefault(x => x.Id == orderInfo.PaymentInfo.Id);

							orderInfo = new PaymentRequestHandler().HandleuWebshopPaymentResponse(paymentProvider, orderInfo);
						}
					}

					orderInfo.Save();

					#endregion

					break;
				case OrderStatus.Cancelled:

					// return stock
					OrderHelper.ReturnStock(orderInfo);

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.CancelEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.CancelEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Cancelled e.SendEmails == false");
					}

					break;
				case OrderStatus.Closed:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.ClosedEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.ClosedEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Closed e.SendEmails == false");
					}
					break;
				case OrderStatus.PaymentFailed:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.PaymentFailedEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.PaymentFailedEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged PaymentFailed e.SendEmails == false");
					}
					break;
				case OrderStatus.OfflinePayment:

					OrderHelper.UpdateStock(orderInfo);

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.OfflinePaymentEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.OfflinePaymentEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged OfflinePayment e.SendEmails == false");
					}
					break;
				case OrderStatus.Pending:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.PendingEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.PendingEmailStore, out storeEmailNodeId);
						
						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Pending e.SendEmails == false");
					}
					break;
				case OrderStatus.ReadyForDispatch:

					Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatch orderInfo.PaymentInfo.PaymentType: " + orderInfo.PaymentInfo.PaymentType);
					if (orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OnlinePayment)
					{
						OrderHelper.UpdateStock(orderInfo);
						Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatch after update stock");
					}
					
					Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatch before send emails");
					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.OnlinePaymentEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.OnlinePaymentEmailStore, out storeEmailNodeId);
						
						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatch e.SendEmails == false");
					}

					Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatch after send emails");
					break;
				case OrderStatus.ReadyForDispatchWhenStockArrives:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.TemporaryOutOfStockEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.TemporaryOutOfStockEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged ReadyForDispatchWhenStockArrives e.SendEmails == false");
					}
					break;
				case OrderStatus.Dispatched:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.DispatchEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.DispatchedEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Dispatched e.SendEmails == false");
					}
					break;
				case OrderStatus.Undeliverable:

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.UndeliverableEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.UndeliverableEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}
						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Undeliverable e.SendEmails == false");
					}
					break;
				case OrderStatus.WaitingForPayment:

					
					
					break;
				case OrderStatus.Returned:

					// return stock
					OrderHelper.ReturnStock(orderInfo);

					if (e.SendEmails)
					{
						int customerEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.ReturnedEmailCustomer, out customerEmailNodeId);
						int storeEmailNodeId;
						int.TryParse(orderInfo.StoreInfo.Store.ReturnedEmailStore, out storeEmailNodeId);

						if (customerEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailCustomer(customerEmailNodeId, orderInfo);
						}

						if (storeEmailNodeId != 0)
						{
							EmailHelper.SendOrderEmailStore(storeEmailNodeId, orderInfo);
						}
					}
					else
					{
						Log.Instance.LogDebug("AfterOrderStatusChanged Returned e.SendEmails == false");
					}

					break;
			}
		}
	}
}