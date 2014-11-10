using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace TelldusCoreWrapper.Service.UnitTest
{
    [TestFixture]
    public class TelldusCoreServiceTests
    {
        private static Mock<ITelldusCoreLibraryWrapper> CreateMockForLibraryWrapper()
        {
            var telldusCoreLibraryWrapperMock = new Mock<ITelldusCoreLibraryWrapper>();
            return telldusCoreLibraryWrapperMock;
        }

        [Test]
        public void Ctor_ServiceIsCreated_InitShouldBeCalled()
        {
            var wrapperMock = CreateMockForLibraryWrapper();
            wrapperMock.Setup(m => m.GetNumberOfDevices()).Returns(0);
            // ReSharper disable once ObjectCreationAsStatement
            new TelldusCoreService(wrapperMock.Object);

            wrapperMock.Verify(m => m.Init(), Times.Once);
        }

        [Test]
        public void Dispose_ServiceIsDisposed_ShouldCallClose()
        {
            var wrapperMock = CreateMockForLibraryWrapper();
            wrapperMock.Setup(m => m.GetNumberOfDevices()).Returns(0);
            
            using (new TelldusCoreService(wrapperMock.Object)){}

            wrapperMock.Verify(m => m.Close(), Times.Once);
        }

        [Test]
        public void GetDevices_NoDevicesRegistered_ReturnEmptyList()
        {
            var wrapperMock = CreateMockForLibraryWrapper();
            wrapperMock.Setup(m => m.GetNumberOfDevices()).Returns(0);
            var service = new TelldusCoreService(wrapperMock.Object);
            
            var result = service.GetDevices().ToList();
            
            wrapperMock.Verify(m => m.GetNumberOfDevices(), Times.Once);
            wrapperMock.Verify(m => m.GetDeviceId(It.IsAny<int>()), Times.Never);
            result.Should().BeEmpty();
        }

        [Test]
        public void GetDevices_OneDeviceRegistered_DeviceIdAndNameIsRetrieved()
        {
            var wrapperMock = CreateMockForLibraryWrapper();
            wrapperMock.Setup(m => m.GetNumberOfDevices()).Returns(1);
            wrapperMock.Setup(m => m.GetDeviceId(0)).Returns(1);
            wrapperMock.Setup(m => m.GetName(1)).Returns("Living room");
            var service = new TelldusCoreService(wrapperMock.Object);

            var result = service.GetDevices().ToList();

            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
            result.First().Name.Should().Be("Living room");
            result.First().Index.Should().Be(0);
        }

        [Test]
        public void GetDevices_DeviceSupportsOnAndOff_VerifyGetSupportedMethods()
        {
            var wrapperMock = CreateMockForLibraryWrapper();
            wrapperMock.Setup(m => m.GetNumberOfDevices()).Returns(1);
            wrapperMock.Setup(m => m.GetDeviceId(0)).Returns(1);
            wrapperMock.Setup(m => m.Methods(1, 1023)).Returns(3); // 1 | 2 = 3
            
            var service = new TelldusCoreService(wrapperMock.Object);

            var result = service.GetDevices().ToList();
            var methods = result.First().SupportedMethod.ToList();

            methods[0].Code.Should().Be(1);
            methods[1].Code.Should().Be(2);
        }
    }
}
