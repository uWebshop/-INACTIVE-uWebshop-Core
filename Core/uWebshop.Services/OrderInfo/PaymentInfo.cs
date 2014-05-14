using System;
using System.Linq;
using SuperSimpleWebshop.Common;
using SuperSimpleWebshop.Domain.Helpers;
using System.Collections.Generic;
using SuperSimpleWebshop.Domain.Interfaces;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class PaymentInfo
    {
        /// <summary>
        /// Gets the chosen payment module of the order
        /// </summary>
        public string PaymentModule;

        /// <summary>
        /// Display title in specific language
        /// </summary>
        public string PaymentProviderTitle;

        /// <summary>
        /// Gets the payment url of the order
        /// </summary>
        public string PaymentURL;

        /// <summary>
        /// Gets the chosen payment method id of the order
        /// </summary>
        public string PaymentMethodId;

        public string PaymentProviderName;
        public string PaymentProviderNodeName;
        public string PaymentProviderMethodId;
        public string PaymentProviderMethodName;
        public string PaymentTransactionId;
        public PaymentParameterRenderMethod PaymentParameterRenderMethod;
        public string PaymentErrorMessage;
        public PaymentProviderType PaymentProviderType
        {
            get
            {
                if(!string.IsNullOrEmpty(PaymentProviderNodeName)) {
                PaymentProvider PayPro = new PaymentProvider(DomainHelper.GetNodeIdForDocument("sswsPaymentProvider", PaymentProviderNodeName));

                return PayPro.PaymentProviderType;
                }
                else {
                    return PaymentProviderType.Unknown;
                }
            }
        }
        public string PaymentParameters;
    }
}
