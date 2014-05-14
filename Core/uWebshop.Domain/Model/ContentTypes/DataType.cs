using System;

namespace uWebshop.Domain.ContentTypes
{
#pragma warning disable 1591
	/// <summary>
	/// 
	/// </summary>
	public class DataTypeDefinitionAttribute : Attribute
	{
		public string DefinitionGuid;
		public string KeyGuid;
		public string Name;
		public DatabaseType Type;
	}

	public enum DatabaseType
	{
		Integer,
		Nvarchar,
		Ntext,
		Date
	}

	//		(-92, 0, -1, 0, 11, '-1,-92', 37, 'f0bc4bfb-b499-40d6-ba86-058885a5178c', 'Label', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-90, 0, -1, 0, 11, '-1,-90', 35, '84c6b441-31df-4ffe-b67e-67d5bc3ae65a', 'Upload', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-89, 0, -1, 0, 11, '-1,-89', 34, 'c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3', 'Textbox multiple', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-88, 0, -1, 0, 11, '-1,-88', 33, '0cc0eba1-9960-42c9-bf9b-60e150b429ae', 'Textstring', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-87, 0, -1, 0, 11, '-1,-87', 32, 'ca90c950-0aff-4e72-b976-a30b1ac57dad', 'Richtext editor', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-51, 0, -1, 0, 11, '-1,-51', 4, '2e6d3631-066e-44b8-aec4-96f09099b2b5', 'Numeric', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-49, 0, -1, 0, 11, '-1,-49', 2, '92897bc6-a5f3-4ffe-ae27-f2e7e33dda49', 'True/false', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/09/30 14:01:49.920'),
	//(-43, 0, -1, 0, 1, '-1,-43', 2, 'fbaf13a8-4036-41f2-93a3-974f678c312a', 'Checkbox list', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:11:04.367'),
	//(-42, 0, -1, 0, 1, '-1,-42', 2, '0b6a45e7-44ba-430d-9da5-4e46060b9e03', 'Dropdow', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:59.000'),
	//(-41, 0, -1, 0, 1, '-1,-41', 2, '5046194e-4237-453c-a547-15db3a07c4e1', 'Date Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:54.303'),
	//(-40, 0, -1, 0, 1, '-1,-40', 2, 'bb5f57c9-ce2b-4bb9-b697-4caca783a805', 'Radiobox', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:49.253'),
	//(-39, 0, -1, 0, 1, '-1,-39', 2, 'f38f0ac7-1d27-439c-9f3f-089cd8825a53', 'Dropdown multiple', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:44.480'),
	//(-38, 0, -1, 0, 1, '-1,-38', 2, 'fd9f1447-6c61-4a7c-9595-5aa39147d318', 'Folder Browser', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:37.020'),
	//(-37, 0, -1, 0, 1, '-1,-37', 2, '0225af17-b302-49cb-9176-b9f35cab9c17', 'Approved Color', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:30.580'),
	//(-36, 0, -1, 0, 1, '-1,-36', 2, 'e4d66c0f-b935-4200-81f0-025f7256b89a', 'Date Picker with time', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2004/10/15 14:10:23.007'),
	//(1031, 0, -1, 1, 1, '-1,1031', 2, 'f38bd2d7-65d0-48e6-95dc-87ce06ec2d3d', 'Folder', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:40.743'),
	//(1032, 0, -1, 1, 1, '-1,1032', 2, 'cc07b313-0843-4aa8-bbda-871c8da728c8', 'Image', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:43.737'),
	//(1033, 0, -1, 1, 1, '-1,1033', 2, '4c52d8ab-54e6-40cd-999c-7a5f24903e4d', 'File', '4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e', '2004/12/01 00:13:46.210'),
	//(1034, 0, -1, 0, 1, '-1,1034', 2, 'a6857c73-d6e9-480c-b6e6-f15f6ad11125', 'Content Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:29.203'),
	//(1035, 0, -1, 0, 1, '-1,1035', 2, '93929b9a-93a2-4e2a-b239-d99334440a59', 'Media Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:36.143'),
	//(1036, 0, -1, 0, 1, '-1,1036', 2, '2b24165f-9782-4aa3-b459-1de4a4d21f60', 'Member Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:40.260'),
	//(1038, 0, -1, 0, 1, '-1,1038', 2, '1251c96c-185c-4e9b-93f4-b48205573cbd', 'Simple Editor', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1039, 0, -1, 0, 1, '-1,1039', 2, '06f349a9-c949-4b6a-8660-59c10451af42', 'Ultimate Picker', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1040, 0, -1, 0, 1, '-1,1040', 2, '21e798da-e06e-4eda-a511-ed257f78d4fa', 'Related Links', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1041, 0, -1, 0, 1, '-1,1041', 2, 'b6b73142-b9c1-4bf8-a16d-e1c23320b549', 'Tags', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1042, 0, -1, 0, 1, '-1,1042', 2, '0a452bd5-83f9-4bc3-8403-1286e13fb77e', 'Macro Container', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250'),
	//(1043, 0, -1, 0, 1, '-1,1042', 2, '1df9f033-e6d4-451f-b8d2-e0cbc50a836f', 'Image Cropper', '30a2a501-1978-4ddb-a57b-f7efed43ba3c', '2006/01/03 13:07:55.250')
	//	if (CulturesDataType == null)
	//{
	//	CulturesDataType = new DataTypeDefinition(-1, new Guid("24a53e42-4bb4-4c9a-a16c-3651e3083f30"));
	//	CulturesDataType.Name = "uWebshop Language Picker";
	//	CulturesDataType.Key = new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010");
	//	CulturesDataType.DatabaseType = DataTypeDatabaseType.Ntext;

	//	newDataTypesList.Add(CulturesDataType);
	//}
    public enum DataType
    {
        [DataTypeDefinition(DefinitionGuid = "92897bc6-a5f3-4ffe-ae27-f2e7e33dda49")]
        TrueFalse,
        String,

        [DataTypeDefinition(DefinitionGuid = "24a53e42-4bb4-4c9a-a16c-3651e3083f30")] // reference issue => move naar Umbraco[6] project
        Cultures,
        Countries,
        Numeric,
        ContentPicker,
        Tags,
        MultiContentPickerCategories,
        MultiContentPickerImages,
        TextboxMultiple,
        RichText,
        Price,
        VatPicker,
        OrderedCount,
        EnableDisable,
        Ranges,
        Stock,
        MultiContentPickerFiles,
        DiscountType,
        MemberGroups,
        DiscountOrderCondition,
        MultiNodePicker,
        MultiContentPickerProducts,
        TemplatePicker,
        MediaPicker,
        MultiContentPickerPaymentZones,
        PaymentProviderType,
        Zones,
        ShippingProviderType,
        ShippingProviderRangeType,
        OrderSection,
        Label,
        OrderStatusPicker,
        StoreTemplatePicker,
        StorePicker,
        ProductOverview,
        OrderInfoViewer,
        MultiContentPickerShippingZones,
        EmailDetails,
        CouponCodes,
        PaymentProviderAmountType,
        Currencies,
        MultiContentPickerCatalog
    }
}