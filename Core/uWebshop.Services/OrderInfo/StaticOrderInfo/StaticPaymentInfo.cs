using System;
using SuperSimpleWebshop.Common;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class StaticPaymentInfo
    {
        /// <summary>
        /// Gets the chosen payment module of the order
        /// </summary>
        public string PaymentModule;

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
        public string PaymentTransactionId;
        public PaymentParameterRenderMethod PaymentParameterRenderMethod;
        public string PaymentErrorMessage;
        public string PaymentType;
        public string PaymentParameters;
    }
}
