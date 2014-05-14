using System.Configuration;

namespace SuperSimpleWebshop.Domain.Configuration
{
    // We're building the following piece of xml in this class:
    //
    // <system.serviceModel>
    //   <bindings>
    //     <basicHttpBinding>
    //       <binding name="BasicHttpBinding_ILicenseService" closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard" maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536" messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered" useDefaultWebProxy="true">
    //         <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="16384"/>
    //         <security mode="None">
    //           <transport clientCredentialType="None" proxyCredentialType="None" realm=""/>
    //           <message clientCredentialType="UserName" algorithmSuite="Default"/>
    //         </security>
    //       </binding>
    //     </basicHttpBinding>
    //   </bindings>
    //   <client>
    //     <endpoint address="http://licensing.supersimplewebshop.com/LicenseService.svc" binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_ILicenseService" contract="ILicenseService" name="BasicHttpBinding_ILicenseService"/>
    //   </client>
    // </system.serviceModel>
    //
    // It will be added to the web.config using:
    // <sectionGroup name="SuperSimpleWebshop">
    //   <section name="system.serviceModel" type="SuperSimpleWebshop.Domain.Configuration.LicenseSection" allowLocation="true" allowDefinition="Everywhere"/>
    // </sectionGroup>

    public class LicenseSection : ConfigurationSection
    {
        [ConfigurationProperty("bindings")]
        public BindingsElement Bindings
        {
            get
            {
                return (BindingsElement)this["bindings"];
            }
            set
            {
                this["bindings"] = value;
            }
        }

        [ConfigurationProperty("client")]
        public ClientElement Client
        {
            get
            {
                return (ClientElement)this["client"];
            }
            set
            {
                this["client"] = value;
            }
        }
    }

    public class BindingsElement : ConfigurationElement
    {
        [ConfigurationProperty("basicHttpBinding")]
        public BasicHttpBindingElement BasicHttpBinding
        {
            get
            {
                return (BasicHttpBindingElement)this["basicHttpBinding"];
            }
            set
            {
                this["basicHttpBinding"] = value;
            }
        }
    }

    public class BasicHttpBindingElement : ConfigurationElement
    {
        #region attributes
        #region name
        internal const string NamePropertyName = "name";

        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "BasicHttpBinding_ILicenseService")]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }
        #endregion

        #region closeTimeout
        internal const string CloseTimeoutPropertyName = "closeTimeout";

        [ConfigurationProperty(CloseTimeoutPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "00:01:00")]
        public string CloseTimeout
        {
            get { return (string)base[CloseTimeoutPropertyName]; }
            set { base[CloseTimeoutPropertyName] = value; }
        }
        #endregion

        #region openTimeout
        internal const string OpenTimeoutPropertyName = "openTimeout";

        [ConfigurationProperty(OpenTimeoutPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "00:01:00")]
        public string OpenTimeout
        {
            get { return (string)base[OpenTimeoutPropertyName]; }
            set { base[OpenTimeoutPropertyName] = value; }
        }
        #endregion

        #region receiveTimeout
        internal const string ReceiveTimeoutPropertyName = "receiveTimeout";

        [ConfigurationProperty(ReceiveTimeoutPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "00:10:00")]
        public string ReceiveTimeout
        {
            get { return (string)base[ReceiveTimeoutPropertyName]; }
            set { base[ReceiveTimeoutPropertyName] = value; }
        }
        #endregion

        #region sendTimeout
        internal const string SendTimeoutPropertyName = "sendTimeout";

        [ConfigurationProperty(SendTimeoutPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "00:01:00")]
        public string SendTimeout
        {
            get { return (string)base[SendTimeoutPropertyName]; }
            set { base[SendTimeoutPropertyName] = value; }
        }
        #endregion

        #region allowCookies
        internal const string AllowCookiesPropertyName = "allowCookies";

        [ConfigurationProperty(AllowCookiesPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "false")]
        public string AllowCookies
        {
            get { return (string)base[AllowCookiesPropertyName]; }
            set { base[AllowCookiesPropertyName] = value; }
        }
        #endregion

        #region bypassProxyOnLocal
        internal const string BypassProxyOnLocalPropertyName = "bypassProxyOnLocal";

        [ConfigurationProperty(BypassProxyOnLocalPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "false")]
        public string BypassProxyOnLocal
        {
            get { return (string)base[BypassProxyOnLocalPropertyName]; }
            set { base[BypassProxyOnLocalPropertyName] = value; }
        }
        #endregion

        #region hostNameComparisonMode
        internal const string HostNameComparisonModePropertyName = "hostNameComparisonMode";

        [ConfigurationProperty(HostNameComparisonModePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "StrongWildcard")]
        public string HostNameComparisonMode
        {
            get { return (string)base[HostNameComparisonModePropertyName]; }
            set { base[HostNameComparisonModePropertyName] = value; }
        }
        #endregion

        #region maxBufferSize
        internal const string MaxBufferSizePropertyName = "maxBufferSize";

        [ConfigurationProperty(MaxBufferSizePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "65536")]
        public string MaxBufferSize
        {
            get { return (string)base[MaxBufferSizePropertyName]; }
            set { base[MaxBufferSizePropertyName] = value; }
        }
        #endregion

        #region maxBufferPoolSize
        internal const string MaxBufferPoolSizePropertyName = "maxBufferPoolSize";

        [ConfigurationProperty(MaxBufferPoolSizePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "524288")]
        public string MaxBufferPoolSize
        {
            get { return (string)base[MaxBufferPoolSizePropertyName]; }
            set { base[MaxBufferPoolSizePropertyName] = value; }
        }
        #endregion

        #region maxReceivedMessageSize
        internal const string MaxReceivedMessageSizePropertyName = "maxReceivedMessageSize";

        [ConfigurationProperty(MaxReceivedMessageSizePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "65536")]
        public string MaxReceivedMessageSize
        {
            get { return (string)base[MaxReceivedMessageSizePropertyName]; }
            set { base[MaxReceivedMessageSizePropertyName] = value; }
        }
        #endregion

        #region messageEncoding
        internal const string MessageEncodingPropertyName = "messageEncoding";

        [ConfigurationProperty(MessageEncodingPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "Text")]
        public string MessageEncoding
        {
            get { return (string)base[MessageEncodingPropertyName]; }
            set { base[MessageEncodingPropertyName] = value; }
        }
        #endregion

        #region textEncoding
        internal const string TextEncodingPropertyName = "textEncoding";

        [ConfigurationProperty(TextEncodingPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "utf-8")]
        public string TextEncoding
        {
            get { return (string)base[TextEncodingPropertyName]; }
            set { base[TextEncodingPropertyName] = value; }
        }
        #endregion

        #region transferMode
        internal const string TransferModePropertyName = "transferMode";

        [ConfigurationProperty(TransferModePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "Buffered")]
        public string TransferMode
        {
            get { return (string)base[TransferModePropertyName]; }
            set { base[TransferModePropertyName] = value; }
        }
        #endregion

        #region useDefaultWebProxy
        internal const string UseDefaultWebProxyPropertyName = "useDefaultWebProxy";

        [ConfigurationProperty(UseDefaultWebProxyPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "true")]
        public string UseDefaultWebProxy
        {
            get { return (string)base[UseDefaultWebProxyPropertyName]; }
            set { base[UseDefaultWebProxyPropertyName] = value; }
        }
        #endregion
        #endregion

        [ConfigurationProperty("readerQuotas")]
        public ReaderQuotasElement ReaderQuotas
        {
            get
            {
                return (ReaderQuotasElement)this["readerQuotas"];
            }
            set
            {
                this["readerQuotas"] = value;
            }
        }

        [ConfigurationProperty("security")]
        public SecurityElement Security
        {
            get
            {
                return (SecurityElement)this["security"];
            }
            set
            {
                this["security"] = value;
            }
        }
    }

    public class ReaderQuotasElement : ConfigurationElement
    {
        #region attributes
        #region maxDepth
        internal const string MaxDepthPropertyName = "maxDepth";

        [ConfigurationProperty(MaxDepthPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "32")]
        public string MaxDepth
        {
            get { return (string)base[MaxDepthPropertyName]; }
            set { base[MaxDepthPropertyName] = value; }
        }
        #endregion

        #region maxStringContentLength
        internal const string MaxStringContentLengthPropertyName = "maxStringContentLength";

        [ConfigurationProperty(MaxStringContentLengthPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "8192")]
        public string MaxString
        {
            get { return (string)base[MaxStringContentLengthPropertyName]; }
            set { base[MaxStringContentLengthPropertyName] = value; }
        }
        #endregion

        #region maxArrayLength
        internal const string MaxArrayLengthPropertyName = "maxArrayLength";

        [ConfigurationProperty(MaxArrayLengthPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "16384")]
        public string MaxArrayLength
        {
            get { return (string)base[MaxArrayLengthPropertyName]; }
            set { base[MaxArrayLengthPropertyName] = value; }
        }
        #endregion

        #region maxBytesPerRead
        internal const string MaxBytesPerReadPropertyName = "maxBytesPerRead";

        [ConfigurationProperty(MaxBytesPerReadPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "4096")]
        public string MaxBytesPerRead
        {
            get { return (string)base[MaxBytesPerReadPropertyName]; }
            set { base[MaxBytesPerReadPropertyName] = value; }
        }
        #endregion

        #region maxNameTableCharCount
        internal const string MaxNameTableCharCountPropertyName = "maxNameTableCharCount";

        [ConfigurationProperty(MaxNameTableCharCountPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "16384")]
        public string MaxNameTableCharCount
        {
            get { return (string)base[MaxNameTableCharCountPropertyName]; }
            set { base[MaxNameTableCharCountPropertyName] = value; }
        }
        #endregion
        #endregion
    }

    public class SecurityElement : ConfigurationElement
    {
        #region attributes
        #region mode
        internal const string ModePropertyName = "mode";

        [ConfigurationProperty(ModePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "None")]
        public string Mode
        {
            get { return (string)base[ModePropertyName]; }
            set { base[ModePropertyName] = value; }
        }
        #endregion
        #endregion

        [ConfigurationProperty("transport")]
        public TransportElement Transport
        {
            get
            {
                return (TransportElement)this["transport"];
            }
            set
            {
                this["transport"] = value;
            }
        }

        [ConfigurationProperty("message")]
        public MessageElement Message
        {
            get
            {
                return (MessageElement)this["message"];
            }
            set
            {
                this["message"] = value;
            }
        }
    }

    public class TransportElement : ConfigurationElement
    {
        #region attributes
        #region clientCredentialType
        internal const string ClientCredentialTypePropertyName = "clientCredentialType";

        [ConfigurationProperty(ClientCredentialTypePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "None")]
        public string ClientCredentialType
        {
            get { return (string)base[ClientCredentialTypePropertyName]; }
            set { base[ClientCredentialTypePropertyName] = value; }
        }
        #endregion

        #region proxyCredentialType
        internal const string ProxyCredentialTypePropertyName = "proxyCredentialType";

        [ConfigurationProperty(ProxyCredentialTypePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "None")]
        public string ProxyCredentialType
        {
            get { return (string)base[ProxyCredentialTypePropertyName]; }
            set { base[ProxyCredentialTypePropertyName] = value; }
        }
        #endregion

        #region realm
        internal const string RealmPropertyName = "realm";

        [ConfigurationProperty(RealmPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "")]
        public string Realm
        {
            get { return (string)base[RealmPropertyName]; }
            set { base[RealmPropertyName] = value; }
        }
        #endregion
        #endregion
    }

    public class MessageElement : ConfigurationElement
    {
        #region attributes
        #region clientCredentialType
        internal const string ClientCredentialTypePropertyName = "clientCredentialType";

        [ConfigurationProperty(ClientCredentialTypePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "UserName")]
        public string ClientCredentialType
        {
            get { return (string)base[ClientCredentialTypePropertyName]; }
            set { base[ClientCredentialTypePropertyName] = value; }
        }
        #endregion

        #region algorithmSuite
        internal const string AlgorithmSuitePropertyName = "algorithmSuite";

        [ConfigurationProperty(AlgorithmSuitePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "Default")]
        public string AlgorithmSuite
        {
            get { return (string)base[AlgorithmSuitePropertyName]; }
            set { base[AlgorithmSuitePropertyName] = value; }
        }
        #endregion
        #endregion
    }

    public class ClientElement : ConfigurationElement
    {
        [ConfigurationProperty("endpoint")]
        public EndpointElement Endpoint
        {
            get
            {
                return (EndpointElement)this["endpoint"];
            }
            set
            {
                this["endpoint"] = value;
            }
        }
    }

    public class EndpointElement : ConfigurationElement
    {
        #region attributes
        #region address
        internal const string AddressPropertyName = "address";

        [ConfigurationProperty(AddressPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "http://licensing.supersimplewebshop.com/LicenseService.svc")]
        public string Address
        {
            get { return (string)base[AddressPropertyName]; }
            set { base[AddressPropertyName] = value; }
        }
        #endregion

        #region binding
        internal const string BindingPropertyName = "binding";

        [ConfigurationProperty(BindingPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "basicHttpBinding")]
        public string Binding
        {
            get { return (string)base[BindingPropertyName]; }
            set { base[BindingPropertyName] = value; }
        }
        #endregion

        #region bindingConfiguration
        internal const string BindingConfigurationPropertyName = "bindingConfiguration";

        [ConfigurationProperty(BindingConfigurationPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "BasicHttpBinding_ILicenseService")]
        public string BindingConfiguration
        {
            get { return (string)base[BindingConfigurationPropertyName]; }
            set { base[BindingConfigurationPropertyName] = value; }
        }
        #endregion

        #region contract
        internal const string ContractPropertyName = "contract";

        [ConfigurationProperty(ContractPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "ILicenseService")]
        public string Contract
        {
            get { return (string)base[ContractPropertyName]; }
            set { base[ContractPropertyName] = value; }
        }
        #endregion

        #region name
        internal const string NamePropertyName = "name";

        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "BasicHttpBinding_ILicenseService")]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }
        #endregion
        #endregion
    }
}
