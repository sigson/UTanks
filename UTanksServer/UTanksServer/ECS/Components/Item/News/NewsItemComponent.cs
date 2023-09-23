using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types;

namespace UTanksServer.ECS.Components.Item.News
{
    [TypeUid(1479374709878L)]
    public class NewsItemComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public NewsItemComponent() { }
        public NewsItemComponent(DateTime? date = null, string headerText = null, string shortText = null,
            string longText = null, string previewImageUrl = null, string previewImageGuid = null,
            string centralIconGuid = null, string toolTip = null, bool previewImageFitInParent = false,
            string externalUrl = null, string internalUrl = null)//, NewsItemLayout layout = NewsItemLayout.LARGE
        {
            Date = date;
            HeaderText = headerText;
            ShortText = shortText;
            LongText = longText;
            PreviewImageUrl = previewImageUrl;
            PreviewImageGuid = previewImageGuid;
            CentralIconGuid = centralIconGuid;
            Tooltip = toolTip;
            PreviewImageFitInParent = previewImageFitInParent;
            ExternalUrl = externalUrl;
            InternalUrl = internalUrl;
            //Layout = layout;
        }

		[OptionalMapped]
		public DateTime? Date { get; set; }

		[OptionalMapped]
		public string HeaderText { get; set; }

		[OptionalMapped]
		public string ShortText { get; set; }

		[OptionalMapped]
		public string LongText { get; set; }

		[OptionalMapped]
		public string PreviewImageUrl { get; set; }

		[OptionalMapped]
		public string PreviewImageGuid { get; set; }

		[OptionalMapped]
		public string CentralIconGuid { get; set; }

		[OptionalMapped]
		public string Tooltip { get; set; }

		public bool PreviewImageFitInParent { get; set; }

		[OptionalMapped]
		public string ExternalUrl { get; set; }

		[OptionalMapped]
		public string InternalUrl { get; set; }

		//public NewsItemLayout Layout { get; set; }
    }
}
