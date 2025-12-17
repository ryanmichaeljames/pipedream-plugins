using Microsoft.Xrm.Sdk;
using Moq;
using PipeDream.Plugins.Constants;
using PipeDream.Plugins.Extensions;
using Xunit;

namespace PipeDream.Plugins.Tests.Extensions
{
    public class PluginExecutionContextExtensionsTests
    {
        #region Stage Checks

        [Fact]
        public void IsPreValidation_WhenStageIs10_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(stage: Stage.PreValidation);

            // Act & Assert
            Assert.True(context.IsPreValidation());
        }

        [Fact]
        public void IsPreValidation_WhenStageIsNot10_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(stage: Stage.PostOperation);

            // Act & Assert
            Assert.False(context.IsPreValidation());
        }

        [Fact]
        public void IsPreOperation_WhenStageIs20_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(stage: Stage.PreOperation);

            // Act & Assert
            Assert.True(context.IsPreOperation());
        }

        [Fact]
        public void IsMainOperation_WhenStageIs30_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(stage: Stage.MainOperation);

            // Act & Assert
            Assert.True(context.IsMainOperation());
        }

        [Fact]
        public void IsPostOperation_WhenStageIs40_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(stage: Stage.PostOperation);

            // Act & Assert
            Assert.True(context.IsPostOperation());
        }

        #endregion

        #region Message Checks

        [Fact]
        public void IsCreate_WhenMessageIsCreate_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Create);

            // Act & Assert
            Assert.True(context.IsCreate());
        }

        [Fact]
        public void IsCreate_WhenMessageIsNotCreate_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Update);

            // Act & Assert
            Assert.False(context.IsCreate());
        }

        [Fact]
        public void IsUpdate_WhenMessageIsUpdate_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Update);

            // Act & Assert
            Assert.True(context.IsUpdate());
        }

        [Fact]
        public void IsDelete_WhenMessageIsDelete_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Delete);

            // Act & Assert
            Assert.True(context.IsDelete());
        }

        [Fact]
        public void IsRetrieve_WhenMessageIsRetrieve_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Retrieve);

            // Act & Assert
            Assert.True(context.IsRetrieve());
        }

        [Fact]
        public void IsRetrieveMultiple_WhenMessageIsRetrieveMultiple_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.RetrieveMultiple);

            // Act & Assert
            Assert.True(context.IsRetrieveMultiple());
        }

        [Fact]
        public void IsAssociate_WhenMessageIsAssociate_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Associate);

            // Act & Assert
            Assert.True(context.IsAssociate());
        }

        [Fact]
        public void IsDisassociate_WhenMessageIsDisassociate_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.Disassociate);

            // Act & Assert
            Assert.True(context.IsDisassociate());
        }

        [Fact]
        public void IsSetState_WhenMessageIsSetState_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.SetState);

            // Act & Assert
            Assert.True(context.IsSetState());
        }

        [Fact]
        public void IsSetStateDynamicEntity_WhenMessageIsSetStateDynamicEntity_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: Message.SetStateDynamicEntity);

            // Act & Assert
            Assert.True(context.IsSetStateDynamicEntity());
        }

        [Fact]
        public void IsMessage_WithMatchingMessage_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(messageName: "CustomMessage");

            // Act & Assert
            Assert.True(context.IsMessage("CustomMessage"));
        }

        [Fact]
        public void IsMessage_WithNonMatchingMessage_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(messageName: "Create");

            // Act & Assert
            Assert.False(context.IsMessage("Update"));
        }

        #endregion

        private static IPluginExecutionContext CreateMockContext(
            int stage = Stage.PreOperation,
            string messageName = Message.Create)
        {
            var mock = new Mock<IPluginExecutionContext>();
            mock.Setup(x => x.Stage).Returns(stage);
            mock.Setup(x => x.MessageName).Returns(messageName);
            return mock.Object;
        }
    }
}
