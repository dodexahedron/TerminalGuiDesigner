﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    internal class TreeViewTests : Tests
    {
        [Test]
        public void TestRoundTrip_TreeView_FileSystemInfo()
        {
            Assert.That(
                TTypes.GetSupportedTTypesForGenericViewOfType(typeof(TreeView<>)),
                Contains.Item(typeof(FileSystemInfo)));

            var sliderIn = RoundTrip<Dialog, TreeView<FileSystemInfo>>((d, v) =>
            {
                var objectsProperty = d.GetDesignableProperty("Objects");
                Assert.That(objectsProperty,Is.InstanceOf<TreeObjectsProperty<FileSystemInfo>>());
                objectsProperty.SetValue(
                    new List<FileSystemInfo>(
                        new FileSystemInfo[] {
                            new DirectoryInfo("/"),
                            new FileInfo("/blah.txt")
                        }));
            }, out var viewAfter);

            

            Assert.That(viewAfter.Objects.Count, Is.EqualTo(2));

            Assert.That(viewAfter.Objects.Count, Is.EqualTo(2));
            Assert.That(viewAfter.Objects.Count, Is.EqualTo(2));

            Assert.That(viewAfter.Objects.ElementAt(0), Is.EqualTo(new DirectoryInfo("/")));
            Assert.That(viewAfter.Objects.ElementAt(1), Is.EqualTo(new FileInfo("/blah.txt")));
        }

    }
}
