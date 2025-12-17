using System;
using Microsoft.Xrm.Sdk;
using Moq;
using PipeDream.Plugins.Extensions;
using Xunit;

namespace PipeDream.Plugins.Tests.Extensions
{
    public class ExecutionContextExtensionsTests
    {
        #region Get Image Methods

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

        [Fact]
        public void GetPreImageGeneric_WhenImageDoesNotExist_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetPreImage<Entity>();

            // Assert
            Assert.Null(result);
        }

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

        [Fact]
        public void GetPostImageGeneric_WhenImageDoesNotExist_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetPostImage<Entity>();

            // Assert
            Assert.Null(result);
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

        [Fact]
        public void GetFinalAttributeValue_WhenTargetHasNullValue_ReturnsNull()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["name"] = null;
            var preImage = new Entity("account") { Id = target.Id };
            preImage["name"] = "Old Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetFinalAttributeValue<string>("name");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetFinalAttribute Tests

        [Fact]
        public void GetFinalAttribute_WithIsBeingSet_SetsOutputParameterCorrectly()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["name"] = "New Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            bool isBeingSet;
            var result = context.GetFinalAttribute("name", "PreImage", out isBeingSet);

            // Assert
            Assert.True(isBeingSet);
            Assert.Equal("New Name", result);
        }

        [Fact]
        public void GetFinalAttribute_WhenNotInTarget_SetsIsBeingSetFalse()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            var preImage = new Entity("account") { Id = target.Id };
            preImage["name"] = "Original Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            bool isBeingSet;
            var result = context.GetFinalAttribute("name", "PreImage", out isBeingSet);

            // Assert
            Assert.False(isBeingSet);
            Assert.Equal("Original Name", result);
        }

        #endregion

        #region GetContextAttribute Tests

        [Fact]
        public void GetContextAttribute_WhenInTarget_ReturnsValueAndSetsIsBeingSet()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["name"] = "Test Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            bool isBeingSet;
            var result = context.GetContextAttribute("name", out isBeingSet);

            // Assert
            Assert.True(isBeingSet);
            Assert.Equal("Test Name", result);
        }

        [Fact]
        public void GetContextAttribute_WhenNotInTarget_ReturnsNullAndSetsIsBeingSetFalse()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            bool isBeingSet;
            var result = context.GetContextAttribute("nonexistent", out isBeingSet);

            // Assert
            Assert.False(isBeingSet);
            Assert.Null(result);
        }

        [Fact]
        public void GetContextAttribute_ForStateCode_ReturnsStateFromInputParameters()
        {
            // Arrange
            var stateValue = new OptionSetValue(1);
            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "State", stateValue } });

            // Act
            bool isBeingSet;
            var result = context.GetContextAttribute("statecode", out isBeingSet);

            // Assert
            Assert.True(isBeingSet);
            Assert.Same(stateValue, result);
        }

        [Fact]
        public void GetContextAttribute_ForStatusCode_ReturnsStatusFromInputParameters()
        {
            // Arrange
            var statusValue = new OptionSetValue(2);
            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Status", statusValue } });

            // Act
            bool isBeingSet;
            var result = context.GetContextAttribute("statuscode", out isBeingSet);

            // Assert
            Assert.True(isBeingSet);
            Assert.Same(statusValue, result);
        }

        #endregion

        #region GetOriginalAttribute Tests

        [Fact]
        public void GetOriginalAttribute_WhenInPreImage_ReturnsValue()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["name"] = "Original Name";

            var context = CreateMockContext(
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetOriginalAttribute("name");

            // Assert
            Assert.Equal("Original Name", result);
        }

        [Fact]
        public void GetOriginalAttribute_WhenNotInPreImage_ReturnsNull()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetOriginalAttribute("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetOriginalAttributeValue_ReturnsTypedValue()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["revenue"] = new Money(1000m);

            var context = CreateMockContext(
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetOriginalAttributeValue<Money>("revenue");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000m, result.Value);
        }

        [Fact]
        public void GetOriginalAttributeValue_WhenWrongType_ReturnsDefault()
        {
            // Arrange
            var preImage = new Entity("account") { Id = Guid.NewGuid() };
            preImage["name"] = "Test";

            var context = CreateMockContext(
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.GetOriginalAttributeValue<int>("name");

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region GetPostImageAttribute Tests

        [Fact]
        public void GetPostImageAttribute_WhenInPostImage_ReturnsValue()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            postImage["name"] = "Updated Name";

            var context = CreateMockContext(
                postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImageAttribute("name");

            // Assert
            Assert.Equal("Updated Name", result);
        }

        [Fact]
        public void GetPostImageAttribute_WhenNotInPostImage_ReturnsNull()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImageAttribute("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPostImageAttributeValue_ReturnsTypedValue()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            postImage["numberofemployees"] = 100;

            var context = CreateMockContext(
                postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImageAttributeValue<int>("numberofemployees");

            // Assert
            Assert.Equal(100, result);
        }

        [Fact]
        public void GetPostImageAttributeValue_WhenWrongType_ReturnsDefault()
        {
            // Arrange
            var postImage = new Entity("account") { Id = Guid.NewGuid() };
            postImage["name"] = "Test";

            var context = CreateMockContext(
                postImages: new EntityImageCollection { { "PostImage", postImage } });

            // Act
            var result = context.GetPostImageAttributeValue<int>("name");

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region HasAttributeChanged Tests

        [Fact]
        public void HasAttributeChanged_WhenValueChanged_ReturnsTrue()
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
            var result = context.HasAttributeChanged("name");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenValueSame_ReturnsFalse()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["name"] = "Same Name";
            var preImage = new Entity("account") { Id = target.Id };
            preImage["name"] = "Same Name";

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("name");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenNotInTargetOrPreImage_ReturnsFalse()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            var preImage = new Entity("account") { Id = target.Id };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("nonexistent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenOptionSetValueChanged_ReturnsTrue()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["statuscode"] = new OptionSetValue(2);
            var preImage = new Entity("account") { Id = target.Id };
            preImage["statuscode"] = new OptionSetValue(1);

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("statuscode");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenEntityReferenceChanged_ReturnsTrue()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["primarycontactid"] = new EntityReference("contact", Guid.NewGuid());
            var preImage = new Entity("account") { Id = target.Id };
            preImage["primarycontactid"] = new EntityReference("contact", Guid.NewGuid());

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("primarycontactid");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenMoneyChanged_ReturnsTrue()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["revenue"] = new Money(2000m);
            var preImage = new Entity("account") { Id = target.Id };
            preImage["revenue"] = new Money(1000m);

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("revenue");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenBothNullOrEmpty_ReturnsFalse()
        {
            // Arrange - both null
            var target = new Entity("account") { Id = Guid.NewGuid() };
            var preImage = new Entity("account") { Id = target.Id };
            preImage["description"] = null;

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("description");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasAttributeChanged_WhenEmptyStringSetOverNull_ReturnsTrue()
        {
            // Arrange - setting empty string (which is a change from null in the target context)
            var target = new Entity("account") { Id = Guid.NewGuid() };
            target["description"] = "";
            var preImage = new Entity("account") { Id = target.Id };
            preImage["description"] = null;

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } },
                preImages: new EntityImageCollection { { "PreImage", preImage } });

            // Act
            var result = context.HasAttributeChanged("description");

            // Assert - The attribute IS being set (even to empty), so it's considered changed
            Assert.True(result);
        }

        #endregion

        #region Target Helper Tests

        [Fact]
        public void GetTarget_WhenEntityExists_ReturnsEntity()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            var result = context.GetTarget();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(target.Id, result.Id);
        }

        [Fact]
        public void GetTarget_WhenEntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetTarget();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTarget_WhenTargetIsEntityReference_ReturnsNull()
        {
            // Arrange
            var targetRef = new EntityReference("account", Guid.NewGuid());

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", targetRef } });

            // Act
            var result = context.GetTarget();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTargetGeneric_ReturnsTypedEntity()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            var result = context.GetTarget<Entity>();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Entity>(result);
        }

        [Fact]
        public void GetTargetReference_WhenEntityReferenceExists_ReturnsEntityReference()
        {
            // Arrange
            var targetRef = new EntityReference("account", Guid.NewGuid());

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", targetRef } });

            // Act
            var result = context.GetTargetReference();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetRef.Id, result.Id);
        }

        [Fact]
        public void GetTargetReference_WhenTargetIsEntity_ReturnsNull()
        {
            // Arrange
            var target = new Entity("account") { Id = Guid.NewGuid() };

            var context = CreateMockContext(
                inputParams: new ParameterCollection { { "Target", target } });

            // Act
            var result = context.GetTargetReference();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTargetReference_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetTargetReference();

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
