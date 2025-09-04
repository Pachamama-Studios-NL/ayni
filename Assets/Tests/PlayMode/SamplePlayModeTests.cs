using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ayni.Tests.PlayMode
{
    public class SamplePlayModeTests
    {
        [UnityTest]
        public IEnumerator NewGameObject_ExistsNextFrame()
        {
            var go = new GameObject("temp");
            yield return null; // wait one frame
            Assert.IsNotNull(GameObject.Find("temp"));
            Object.Destroy(go);
        }
    }
}

