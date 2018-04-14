using System;

namespace AntData.ORM
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true,
        Inherited = true)]
    public class ExpressionMethodAttribute : Attribute
    {
        /// <summary>
        /// Creates instance of attribute.
        /// </summary>
        /// <param name="methodName">Name of method in the same class that returns substitution expression.</param>
        public ExpressionMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Creates instance of attribute.
        /// </summary>
        /// <param name="configuration">Connection configuration, for which this attribute should be taken into account.</param>
        /// <param name="methodName">Name of method in the same class that returns substitution expression.</param>
        public ExpressionMethodAttribute(string configuration, string methodName)
        {
            Configuration = configuration;
            MethodName = methodName;
        }

        /// <summary>
        /// Mapping schema configuration name, for which this attribute should be taken into account.
        /// <see cref="ProviderName"/> for standard names.
        /// Attributes with <c>null</c> or empty string <see cref="Configuration"/> value applied to all configurations (if no attribute found for current configuration).
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Name of method in the same class that returns substitution expression.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Indicates whether a property should be mapped with this expression Method. </summary>
        /// <value>
        /// True if the property should be mapped with this expression Method. </value>
        public bool IsColumn { get; set; }
    }
}
