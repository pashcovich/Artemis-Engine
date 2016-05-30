using Artemis.Engine;
using Artemis.Engine.Graphics.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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

        [TestMethod]
        public void AAMLLoadDirectoryTest()
        {
            ArtemisEngine.Setup("game.constants", Setup);
            ArtemisEngine.Begin(Initialize);

            AAMLFileReader fileReader = new AAMLFileReader("../../Animation/LoadDirectoryAAMLTestFile.aaml");
            try
            {
                fileReader.Read();
                Assert.AreEqual(20, fileReader.Map.SpriteSheet.LoadedTextures.Count);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message + "\nAt:\n" + e.StackTrace);
            }
        }

        [TestMethod]
        public void AAMLLoadDirectoryFullTest()
        {
            ArtemisEngine.Setup("game.constants", Setup);
            ArtemisEngine.Begin(Initialize);

            AAMLFileReader fileReader = new AAMLFileReader("../../Animation/LoadDirectoryFullAAMLTestFile.aaml");
            try
            {
                fileReader.Read();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message + "\nAt:\n" + e.StackTrace);
            }
        }

        static void Setup()
        {
            
        }

        static void Initialize()
        {
            ArtemisEngine.RegisterMultiforms(new MultiformTemplate());
            ArtemisEngine.StartWith("MultiformTemplate");
        }
    }
}