using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.UI.GameUI
{
    public class StatsCommandElement : MonoBehaviour
    {
        public SerializableDictionary<long, StatsUserRowElement> commandStats = new SerializableDictionary<long, StatsUserRowElement>();
    }
}
