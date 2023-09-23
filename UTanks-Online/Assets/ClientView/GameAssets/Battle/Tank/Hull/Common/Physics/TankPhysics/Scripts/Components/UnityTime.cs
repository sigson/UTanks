using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public interface UnityTime
    {
        float time { get; }

        float timeSinceLevelLoad { get; }

        float deltaTime { get; }

        float fixedTime { get; }

        float unscaledTime { get; }

        float unscaledDeltaTime { get; }

        float fixedDeltaTime { get; set; }

        float maximumDeltaTime { get; set; }

        float smoothDeltaTime { get; }

        float timeScale { get; set; }

        int frameCount { get; }

        int renderedFrameCount { get; }

        float realtimeSinceStartup { get; }

        int captureFramerate { get; set; }
    }
}