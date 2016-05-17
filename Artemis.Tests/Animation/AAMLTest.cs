using Artemis.Engine;
using Artemis.Engine.Graphics.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Artemis.Tests.Animation
{
    [TestClass]
    public class AAMLFileReaderTests
    {
        [TestMethod]
        public void AAMLLoadTileTest()
        {
            ArtemisEngine.Setup("game.constants", Setup);
            ArtemisEngine.Begin(Initialize);
        }

        static void Setup()
        {
            
        }

        static void Initialize()
        {
            ArtemisEngine.RegisterMultiforms(new MultiformTemplate());
            ArtemisEngine.StartWith("MultiformTemplate");

            AAMLFileReader fileReader = new AAMLFileReader("../../Animation/LoadTileAAMLTestFile.aaml");
            try
            {
                fileReader.Read();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message + "\nAt:\n" + e.StackTrace);
            }
        }
    }
}