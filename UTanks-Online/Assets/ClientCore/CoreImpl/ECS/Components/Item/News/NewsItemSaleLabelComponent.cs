using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Item.News
{
    [TypeUid(1481290407948L)]
    public class NewsItemSaleLabelComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public NewsItemSaleLabelComponent() { }
        public NewsItemSaleLabelComponent(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
