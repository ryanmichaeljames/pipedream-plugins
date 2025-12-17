using System;
using Microsoft.Xrm.Sdk;
using PipeDream.Plugins.Constants;

namespace PipeDream.Plugins.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IExecutionContext"/> to simplify working with entity images and attributes.
    /// </summary>
    public static class ExecutionContextExtensions
    {
        #region Get Image Methods

        /// <summary>
        /// Gets the pre-image entity with the specified name, converted to the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to convert to.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="imageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The pre-image entity, or null if not found.</returns>
        public static TEntity GetPreImage<TEntity>(this IExecutionContext context, string imageName = ImageName.PreImage) 
            where TEntity : Entity
        {
            if (context.PreEntityImages != null && context.PreEntityImages.Contains(imageName))
            {
                return context.PreEntityImages[imageName].ToEntity<TEntity>();
            }
            return null;
        }

        /// <summary>
        /// Gets the post-image entity with the specified name, converted to the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to convert to.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="imageName">The name of the post-image (defaults to "PostImage").</param>
        /// <returns>The post-image entity, or null if not found.</returns>
        public static TEntity GetPostImage<TEntity>(this IExecutionContext context, string imageName = ImageName.PostImage) 
            where TEntity : Entity
        {
            if (context.PostEntityImages != null && context.PostEntityImages.Contains(imageName))
            {
                return context.PostEntityImages[imageName].ToEntity<TEntity>();
            }
            return null;
        }

        /// <summary>
        /// Gets the pre-image entity with the specified name.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="imageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The pre-image entity, or null if not found.</returns>
        public static Entity GetPreImage(this IExecutionContext context, string imageName = ImageName.PreImage)
        {
            if (context.PreEntityImages != null && context.PreEntityImages.Contains(imageName))
            {
                return context.PreEntityImages[imageName];
            }
            return null;
        }

        /// <summary>
        /// Gets the post-image entity with the specified name.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="imageName">The name of the post-image (defaults to "PostImage").</param>
        /// <returns>The post-image entity, or null if not found.</returns>
        public static Entity GetPostImage(this IExecutionContext context, string imageName = ImageName.PostImage)
        {
            if (context.PostEntityImages != null && context.PostEntityImages.Contains(imageName))
            {
                return context.PostEntityImages[imageName];
            }
            return null;
        }

        #endregion

        #region Get Attribute Methods

        /// <summary>
        /// Gets the final value of an attribute, checking the Target entity first, then falling back to the pre-image.
        /// Useful for getting the value that will be persisted after the operation completes.
        /// </summary>
        /// <typeparam name="T">The type of the attribute value.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The final attribute value, or default if not found.</returns>
        public static T GetFinalAttributeValue<T>(this IExecutionContext context, string attributeName, string preImageName = ImageName.PreImage)
        {
            var value = context.GetFinalAttribute(attributeName, preImageName);
            if (value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        /// <summary>
        /// Gets the final value of an attribute, checking the Target entity first, then falling back to the pre-image.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The final attribute value, or null if not found.</returns>
        public static object GetFinalAttribute(this IExecutionContext context, string attributeName, string preImageName = ImageName.PreImage)
        {
            bool isBeingSet;
            return context.GetFinalAttribute(attributeName, preImageName, out isBeingSet);
        }

        /// <summary>
        /// Gets the final value of an attribute, checking the Target entity first, then falling back to the pre-image.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image.</param>
        /// <param name="isBeingSet">Output parameter indicating if the attribute is being set in this operation.</param>
        /// <returns>The final attribute value, or null if not found.</returns>
        public static object GetFinalAttribute(this IExecutionContext context, string attributeName, string preImageName, out bool isBeingSet)
        {
            var contextValue = context.GetContextAttribute(attributeName, out isBeingSet);
            if (isBeingSet)
            {
                return contextValue;
            }

            return context.GetOriginalAttribute(attributeName, preImageName);
        }

        /// <summary>
        /// Gets an attribute value from the Target entity in InputParameters.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="isBeingSet">Output parameter indicating if the attribute is present in the Target.</param>
        /// <returns>The attribute value, or null if not in Target.</returns>
        public static object GetContextAttribute(this IExecutionContext context, string attributeName, out bool isBeingSet)
        {
            isBeingSet = false;

            if (context.InputParameters.ContainsKey(InputParameter.Target))
            {
                var target = context.InputParameters[InputParameter.Target] as Entity;
                if (target != null && target.Contains(attributeName))
                {
                    isBeingSet = true;
                    return target[attributeName];
                }
            }

            // Handle SetState message
            if (attributeName.Equals(AttributeName.StateCode, StringComparison.OrdinalIgnoreCase) && 
                context.InputParameters.Contains(InputParameter.State))
            {
                isBeingSet = true;
                return context.InputParameters[InputParameter.State];
            }

            if (attributeName.Equals(AttributeName.StatusCode, StringComparison.OrdinalIgnoreCase) && 
                context.InputParameters.Contains(InputParameter.Status))
            {
                isBeingSet = true;
                return context.InputParameters[InputParameter.Status];
            }

            return null;
        }

        /// <summary>
        /// Gets an attribute value from the pre-image.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The attribute value from the pre-image, or null if not found.</returns>
        public static object GetOriginalAttribute(this IExecutionContext context, string attributeName, string preImageName = ImageName.PreImage)
        {
            if (context.PreEntityImages != null && context.PreEntityImages.Contains(preImageName))
            {
                var preImage = context.PreEntityImages[preImageName];
                if (preImage.Contains(attributeName))
                {
                    return preImage[attributeName];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an attribute value from the pre-image, typed.
        /// </summary>
        /// <typeparam name="T">The type of the attribute value.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>The attribute value from the pre-image, or default if not found.</returns>
        public static T GetOriginalAttributeValue<T>(this IExecutionContext context, string attributeName, string preImageName = ImageName.PreImage)
        {
            var value = context.GetOriginalAttribute(attributeName, preImageName);
            if (value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        /// <summary>
        /// Gets an attribute value from the post-image.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="postImageName">The name of the post-image (defaults to "PostImage").</param>
        /// <returns>The attribute value from the post-image, or null if not found.</returns>
        public static object GetPostImageAttribute(this IExecutionContext context, string attributeName, string postImageName = ImageName.PostImage)
        {
            if (context.PostEntityImages != null && context.PostEntityImages.Contains(postImageName))
            {
                var postImage = context.PostEntityImages[postImageName];
                if (postImage.Contains(attributeName))
                {
                    return postImage[attributeName];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an attribute value from the post-image, typed.
        /// </summary>
        /// <typeparam name="T">The type of the attribute value.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="postImageName">The name of the post-image (defaults to "PostImage").</param>
        /// <returns>The attribute value from the post-image, or default if not found.</returns>
        public static T GetPostImageAttributeValue<T>(this IExecutionContext context, string attributeName, string postImageName = ImageName.PostImage)
        {
            var value = context.GetPostImageAttribute(attributeName, postImageName);
            if (value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        #endregion

        #region Comparison Methods

        /// <summary>
        /// Checks if an attribute value has changed between the pre-image and the final value.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <param name="preImageName">The name of the pre-image (defaults to "PreImage").</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public static bool HasAttributeChanged(this IExecutionContext context, string attributeName, string preImageName = ImageName.PreImage)
        {
            var originalValue = context.GetOriginalAttribute(attributeName, preImageName);
            var finalValue = context.GetFinalAttribute(attributeName, preImageName);

            return IsValueDifferent(finalValue, originalValue);
        }

        /// <summary>
        /// Checks if two EntityReference values are equal (comparing by Id).
        /// </summary>
        private static bool AreEqual(EntityReference e1, EntityReference e2)
        {
            if (e1 == null && e2 == null) return true;
            if (e1 == null || e2 == null) return false;
            return e1.Id == e2.Id;
        }

        /// <summary>
        /// Checks if two OptionSetValue values are equal.
        /// </summary>
        private static bool AreEqual(OptionSetValue o1, OptionSetValue o2)
        {
            if (o1 == null && o2 == null) return true;
            if (o1 == null || o2 == null) return false;
            return o1.Value == o2.Value;
        }

        /// <summary>
        /// Checks if two string values are equal (treats null and empty string as equal).
        /// </summary>
        private static bool AreEqual(string s1, string s2)
        {
            // In CRM pipeline null is treated as string.Empty sometimes
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
            return s1 == s2;
        }

        /// <summary>
        /// Checks if two values are different, handling common Dataverse types.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>True if the values are different, false if equal.</returns>
        private static bool IsValueDifferent(object newValue, object originalValue)
        {
            if (newValue == null && originalValue == null) return false;
            if (newValue == null || originalValue == null) return true;

            // String comparison (null/empty treated as equal)
            if (newValue is string newString)
            {
                return !AreEqual(newString, originalValue as string);
            }

            // OptionSetValue comparison
            if (newValue is OptionSetValue newOsv)
            {
                return !AreEqual(newOsv, originalValue as OptionSetValue);
            }

            // EntityReference comparison
            if (newValue is EntityReference newEr)
            {
                return !AreEqual(newEr, originalValue as EntityReference);
            }

            // Money comparison
            if (newValue is Money newMoney)
            {
                var oldMoney = originalValue as Money;
                if (oldMoney == null) return true;
                return newMoney.Value != oldMoney.Value;
            }

            // Default comparison
            return !newValue.Equals(originalValue);
        }

        #endregion

        #region Target Helpers

        /// <summary>
        /// Gets the Target entity from InputParameters.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>The Target entity, or null if not present.</returns>
        public static Entity GetTarget(this IExecutionContext context)
        {
            if (context.InputParameters.Contains(InputParameter.Target) && 
                context.InputParameters[InputParameter.Target] is Entity entity)
            {
                return entity;
            }
            return null;
        }

        /// <summary>
        /// Gets the Target entity from InputParameters, converted to the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to convert to.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <returns>The Target entity, or null if not present.</returns>
        public static TEntity GetTarget<TEntity>(this IExecutionContext context) where TEntity : Entity
        {
            var target = context.GetTarget();
            return target?.ToEntity<TEntity>();
        }

        /// <summary>
        /// Gets the Target entity reference from InputParameters (for Delete operations).
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>The Target EntityReference, or null if not present.</returns>
        public static EntityReference GetTargetReference(this IExecutionContext context)
        {
            if (context.InputParameters.Contains(InputParameter.Target) && 
                context.InputParameters[InputParameter.Target] is EntityReference entityRef)
            {
                return entityRef;
            }
            return null;
        }

        #endregion
    }
}
