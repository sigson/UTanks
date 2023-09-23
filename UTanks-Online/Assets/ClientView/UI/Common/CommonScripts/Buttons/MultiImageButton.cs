using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    public class MultiImageButton : Button
    {
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            var targetColor =
                state == SelectionState.Disabled ? colors.disabledColor :
                state == SelectionState.Highlighted ? colors.highlightedColor :
                state == SelectionState.Normal ? colors.normalColor :
                state == SelectionState.Pressed ? colors.pressedColor :
                state == SelectionState.Selected ? colors.selectedColor : Color.white;

            foreach (var graphic in (GetComponentsInChildren<Graphic>().ToList().Cast<MonoBehaviour>().Concat(GetComponents<Colorizer>().ToList().Cast<MonoBehaviour>())).ToList())
            {
                if (graphic is Graphic _graph)
                    _graph.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                else if (graphic is Colorizer _color)
                    _color.UpdateColor(targetColor);
            }
        }
    }

}