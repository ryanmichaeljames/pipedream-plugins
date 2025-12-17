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

        #region Mode Checks

        [Fact]
        public void IsSynchronous_WhenModeIs0_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(mode: Mode.Synchronous);

            // Act & Assert
            Assert.True(context.IsSynchronous());
        }

        [Fact]
        public void IsSynchronous_WhenModeIs1_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(mode: Mode.Asynchronous);

            // Act & Assert
            Assert.False(context.IsSynchronous());
        }

        [Fact]
        public void IsAsynchronous_WhenModeIs1_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(mode: Mode.Asynchronous);

            // Act & Assert
            Assert.True(context.IsAsynchronous());
        }

        [Fact]
        public void IsAsynchronous_WhenModeIs0_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(mode: Mode.Synchronous);

            // Act & Assert
            Assert.False(context.IsAsynchronous());
        }

        #endregion

        #region Depth Checks

        [Fact]
        public void ExceedsDepth_WhenDepthGreaterThanMax_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(depth: 2);

            // Act & Assert
            Assert.True(context.ExceedsDepth(1));
        }

        [Fact]
        public void ExceedsDepth_WhenDepthEqualsMax_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(depth: 1);

            // Act & Assert
            Assert.False(context.ExceedsDepth(1));
        }

        [Fact]
        public void ExceedsDepth_WhenDepthLessThanMax_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(depth: 1);

            // Act & Assert
            Assert.False(context.ExceedsDepth(5));
        }

        [Fact]
        public void IsInitialInvocation_WhenDepthIs1_ReturnsTrue()
        {
            // Arrange
            var context = CreateMockContext(depth: Depth.Initial);

            // Act & Assert
            Assert.True(context.IsInitialInvocation());
        }

        [Fact]
        public void IsInitialInvocation_WhenDepthIsGreaterThan1_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext(depth: 2);

            // Act & Assert
            Assert.False(context.IsInitialInvocation());
        }

        #endregion

        #region Shared Variables

        [Fact]
        public void GetSharedVariable_WhenExists_ReturnsValue()
        {
            // Arrange
            var sharedVars = new ParameterCollection { { "TestKey", "TestValue" } };
            var context = CreateMockContext(sharedVariables: sharedVars);

            // Act
            var result = context.GetSharedVariable<string>("TestKey");

            // Assert
            Assert.Equal("TestValue", result);
        }

        [Fact]
        public void GetSharedVariable_WhenNotExists_ReturnsDefault()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetSharedVariable<string>("NonExistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSharedVariable_WhenWrongType_ReturnsDefault()
        {
            // Arrange
            var sharedVars = new ParameterCollection { { "TestKey", 123 } };
            var context = CreateMockContext(sharedVariables: sharedVars);

            // Act
            var result = context.GetSharedVariable<string>("TestKey", "default");

            // Assert
            Assert.Equal("default", result);
        }

        [Fact]
        public void SetSharedVariable_SetsValue()
        {
            // Arrange
            var sharedVars = new ParameterCollection();
            var context = CreateMockContext(sharedVariables: sharedVars);

            // Act
            context.SetSharedVariable("NewKey", "NewValue");

            // Assert
            Assert.Equal("NewValue", sharedVars["NewKey"]);
        }

        [Fact]
        public void HasSharedVariable_WhenExists_ReturnsTrue()
        {
            // Arrange
            var sharedVars = new ParameterCollection { { "TestKey", "TestValue" } };
            var context = CreateMockContext(sharedVariables: sharedVars);

            // Act & Assert
            Assert.True(context.HasSharedVariable("TestKey"));
        }

        [Fact]
        public void HasSharedVariable_WhenNotExists_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext();

            // Act & Assert
            Assert.False(context.HasSharedVariable("NonExistent"));
        }

        [Fact]
        public void RemoveSharedVariable_WhenExists_RemovesAndReturnsTrue()
        {
            // Arrange
            var sharedVars = new ParameterCollection { { "TestKey", "TestValue" } };
            var context = CreateMockContext(sharedVariables: sharedVars);

            // Act
            var result = context.RemoveSharedVariable("TestKey");

            // Assert
            Assert.True(result);
            Assert.False(sharedVars.ContainsKey("TestKey"));
        }

        [Fact]
        public void RemoveSharedVariable_WhenNotExists_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.RemoveSharedVariable("NonExistent");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Parent Context

        [Fact]
        public void GetRootContext_WhenNoParent_ReturnsSelf()
        {
            // Arrange
            var context = CreateMockContext();

            // Act
            var result = context.GetRootContext();

            // Assert
            Assert.Same(context, result);
        }

        [Fact]
        public void GetRootContext_WhenHasParent_ReturnsRoot()
        {
            // Arrange
            var rootContext = CreateMockContext();
            var parentContext = CreateMockContext(parentContext: rootContext);
            var context = CreateMockContext(parentContext: parentContext);

            // Act
            var result = context.GetRootContext();

            // Assert
            Assert.Same(rootContext, result);
        }

        [Fact]
        public void HasParentContext_WhenHasParent_ReturnsTrue()
        {
            // Arrange
            var parentContext = CreateMockContext();
            var context = CreateMockContext(parentContext: parentContext);

            // Act & Assert
            Assert.True(context.HasParentContext());
        }

        [Fact]
        public void HasParentContext_WhenNoParent_ReturnsFalse()
        {
            // Arrange
            var context = CreateMockContext();

            // Act & Assert
            Assert.False(context.HasParentContext());
        }

        #endregion

        private static IPluginExecutionContext CreateMockContext(
            int stage = Stage.PreOperation,
            string messageName = Message.Create,
            int mode = Mode.Synchronous,
            int depth = Depth.Initial,
            ParameterCollection sharedVariables = null,
            IPluginExecutionContext parentContext = null)
        {
            var mock = new Mock<IPluginExecutionContext>();
            mock.Setup(x => x.Stage).Returns(stage);
            mock.Setup(x => x.MessageName).Returns(messageName);
            mock.Setup(x => x.Mode).Returns(mode);
            mock.Setup(x => x.Depth).Returns(depth);
            mock.Setup(x => x.SharedVariables).Returns(sharedVariables ?? new ParameterCollection());
            mock.Setup(x => x.ParentContext).Returns(parentContext);
            return mock.Object;
        }
    }
}
