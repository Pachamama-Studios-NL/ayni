using NUnit.Framework;
using UnityEngine;

namespace Ayni.Tests.EditMode
{
    public class SampleEditModeTests
    {
        [Test]
        public void VectorAddition_Works()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            Assert.AreEqual(new Vector3(5, 7, 9), a + b);
        }
    }
}

