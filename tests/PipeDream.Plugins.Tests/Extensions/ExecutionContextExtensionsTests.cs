using System;
using Microsoft.Xrm.Sdk;
using Moq;
using PipeDream.Plugins.Extensions;
using Xunit;

namespace PipeDream.Plugins.Tests.Extensions
{
    public class ExecutionContextExtensionsTests
    {
        #region GetPreImage Tests

        [Fact]
        public void GetPreImage_WhenImageExists_ReturnsEntity()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["name"] = "Test Account";
            var context = CreateMockContext(preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetPreImage();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Account", result.GetAttributeValue<string>("name"));
        }

        [Fact]
        public void GetPreImage_WhenImageDoesNotExist_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetPreImage();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPreImage_WithCustomName_ReturnsCorrectImage()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            var context = CreateMockContext(preImages: new EntityImageCollection { { "CustomPreImage", preImage } });

            // Act
            var result = context.GetPreImage("CustomPreImage");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetPreImageGeneric_ReturnsTypedEntity()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["name"] = "Test";
            var context = CreateMockContext(preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetPreImage<Entity>();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Entity>(result);
        }

        #endregion

        #region GetPostImage Tests

        [Fact]
        public void GetPostImage_WhenImageExists_ReturnsEntity()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            postImage["name"] = "Updated Account";
            var context = CreateMockContext(postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImage();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Account", result.GetAttributeValue<string>("name"));
        }

        [Fact]
        public void GetPostImage_WhenImageDoesNotExist_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetPostImage();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPostImageGeneric_ReturnsTypedEntity()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            var context = CreateMockContext(postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImage<Entity>();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Entity>(result);
        }

        #endregion

        #region GetFinalAttributeValue Tests

        [Fact]
        public void GetFinalAttributeValue_WhenInTarget_ReturnsTargetValue()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["name"] = "New Name";
            var preImage = new Entity("account") { Id = target.Id };
            preImage["name"] = "Old Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetFinalAttributeValue<string>("name");

            // Assert
            Assert.Equal("New Name", result);
        }

        [Fact]
        public void GetFinalAttributeValue_WhenNotInTarget_ReturnsPreImageValue()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            // target does not have "description"
            var preImage = new Entity("account") { Id = target.Id };
            preImage["description"] = "Original Description";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetFinalAttributeValue<string>("description");

            // Assert
            Assert.Equal("Original Description", result);
        }

        [Fact]
        public void GetFinalAttributeValue_WhenNotInEither_ReturnsDefault()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            var preImage = new Entity("account") { Id = target.Id };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetFinalAttributeValue<string>("nonexistent");

            // Assert
            Assert.Null(result);
        }

        #endregion

        private static IExecutionContext CreateMockContext(
            ParameterCollection inputParams = null,
            EntityImageCollection preImages = null,
            EntityImageCollection postImages = null)
        {
            var mock = new Mock<IExecutionContext>();
            mock.Setup(x => x.InputParameters).Returns(inputParams ?? new ParameterCollection());
            mock.Setup(x => x.PreEntityImages).Returns(preImages ?? new EntityImageCollection());
            mock.Setup(x => x.PostEntityImages).Returns(postImages ?? new EntityImageCollection());
            return mock.Object;
        }
    }
}
