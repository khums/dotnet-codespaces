using NUnit.Framework;
using Bunit;
using FrontEnd.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

namespace ImageProcessingAppTests
{
    public class ImageEdgeDetectionTests
    {
        private Bunit.TestContext testContext;
        private ImageEdgeDetection component;

        [SetUp]
        public void Setup()
        {
            testContext = new Bunit.TestContext();

            // Mock IJSRuntime (if needed for your component)
            var jsRuntimeMock = new Mock<IJSRuntime>();
            testContext.Services.AddSingleton<IJSRuntime>(jsRuntimeMock.Object);

            // Create the component
            component = testContext.RenderComponent<ImageEdgeDetection>().Instance;
        }

        [Test]
        public async Task ProcessImage_ValidImage_ReturnsBase64String()
        {
            // Arrange
            var image = new Image<Rgba32>(100, 100);
            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                await image.SaveAsPngAsync(ms);
                imageBytes = ms.ToArray();
            }

            // Act
            string? cannyResult = await component.ProcessImage(imageBytes, "canny");
            string? sobelResult = await component.ProcessImage(imageBytes, "sobel");
            string? laplacianResult = await component.ProcessImage(imageBytes, "laplacian");

            // Assert
            Assert.That(cannyResult, Is.Not.Null.And.StartsWith("data:image/png;base64,"));
            Assert.That(sobelResult, Is.Not.Null.And.StartsWith("data:image/png;base64,"));
            Assert.That(laplacianResult, Is.Not.Null.And.StartsWith("data:image/png;base64,"));
        }

        [Test]
        public async Task ProcessImage_InvalidAlgorithm_ReturnsNull()
        {
            // Arrange
            var image = new Image<Rgba32>(100, 100);
            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                await image.SaveAsPngAsync(ms);
                imageBytes = ms.ToArray();
            }
            // Act
            string? result = await component.ProcessImage(imageBytes, "invalid");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ProcessImage_InvalidImageBytes_ReturnsNull()
        {
            // Arrange
            byte[] invalidImageBytes = { 1, 2, 3 }; // Invalid image data

            // Act
            string? result = await component.ProcessImage(invalidImageBytes, "canny");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CannyEdgeDetection_ValidImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(100, 100);

            // Act
            var result = component.CannyEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(100));
            Assert.That(result.Height, Is.EqualTo(100));
        }

        [Test]
        public void SobelEdgeDetection_ValidImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(100, 100);

            // Act
            var result = component.SobelEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(100));
            Assert.That(result.Height, Is.EqualTo(100));
        }

        [Test]
        public void LaplacianEdgeDetection_ValidImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(100, 100);

            // Act
            var result = component.LaplacianEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(100));
            Assert.That(result.Height, Is.EqualTo(100));
        }

        [Test]
        public void CannyEdgeDetection_EmptyImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(0, 0);

            // Act
            var result = component.CannyEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(0));
            Assert.That(result.Height, Is.EqualTo(0));
        }

        [Test]
        public void SobelEdgeDetection_EmptyImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(0, 0);

            // Act
            var result = component.SobelEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(0));
            Assert.That(result.Height, Is.EqualTo(0));
        }

        [Test]
        public void LaplacianEdgeDetection_EmptyImage_ReturnsImage()
        {
            // Arrange
            var image = new Image<Rgba32>(0, 0);

            // Act
            var result = component.LaplacianEdgeDetection(image);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(0));
            Assert.That(result.Height, Is.EqualTo(0));
        }
    }
}