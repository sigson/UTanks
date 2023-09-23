using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class CharacteristicTableChainElement : MonoBehaviour
    {
        public CharacteristicTableChainElementColumn characteristicTableChainElementColumnExample;
        public SerializableDictionary<string, CharacteristicTableChainElementColumn> characteristicColumns = new SerializableDictionary<string, CharacteristicTableChainElementColumn>();
        public GameObject GradeContainer;
        public Image RankAccessIcon;
        public Text GradePrice;
    }
}