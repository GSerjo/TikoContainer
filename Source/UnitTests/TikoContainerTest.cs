using System;
using NUnit.Framework;
using Tiko;

namespace UnitTests
{
	[TestFixture]
    public sealed class TikoContainerTest
    {
        [Test]
        public void BuildUp()
        {
            TikoContainer.Register<TestProperty>();
            var result = new TestClass();
            TikoContainer.BuildUp(result);
            Assert.IsNull(result.TestProperty1);
            Assert.IsNotNull(result.TestProperty);
        }

        [Test]
        public void Register()
        {
            TikoContainer.Register<ITestClass, TestClass>();
            var result = TikoContainer.Resolve<ITestClass>();
            Assert.IsNotNull(result);
        }

        [Test]
        public void RegisterAsSingleton()
        {
            TikoContainer.Register<ITestClass, TestClass>();
            var result = TikoContainer.Resolve<ITestClass>();
            var result1 = TikoContainer.Resolve<ITestClass>();
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, result1);
        }

        [Test]
        public void Resolve()
        {
            TikoContainer.Register<TestProperty>();
            var result = TikoContainer.Resolve<TestClass>();
            Assert.IsNotNull(result);
            Assert.IsNull(result.TestProperty1);
            Assert.IsNotNull(result.TestProperty);
            var result1 = TikoContainer.Resolve<TestClass>();
            Assert.AreEqual(result.TestProperty1, result1.TestProperty1);
        }

        [Test, ExpectedException(typeof (DependencyMissingException))]
        public void Resolve_MissingDependency()
        {
            TikoContainer.Resolve<TestClass>();
        }

        [SetUp]
        public void SetUp()
        {
            TikoContainer.Clear();
        }
    }

    public interface ITestClass
    {
    }

    public sealed class TestClass : ITestClass
    {
        [Dependency]
        public TestProperty TestProperty { get; set; }

        public TestProperty TestProperty1 { get; set; }
    }

    public sealed class TestProperty
    {
    }
}

