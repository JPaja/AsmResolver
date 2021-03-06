using System.Collections.Generic;

namespace AsmResolver.DotNet.Builder.Discovery
{
    /// <summary>
    /// Provides a collection of members that were discovered during a traversal of a module.
    /// </summary>
    public class MemberDiscoveryResult
    {
        /// <summary>
        /// Gets a list of types that are discovered during the traversal of the module.
        /// </summary>
        public List<TypeDefinition> Types
        {
            get;
        } = new();

        /// <summary>
        /// Gets a list of fields that are discovered during the traversal of the module.
        /// </summary>
        public List<FieldDefinition> Fields
        {
            get;
        } = new();

        /// <summary>
        /// Gets a list of modules that are discovered during the traversal of the module.
        /// </summary>
        public List<MethodDefinition> Methods
        {
            get;
        } = new();

        /// <summary>
        /// Gets a list of parameters that are discovered during the traversal of the module.
        /// </summary>
        public List<ParameterDefinition> Parameters
        {
            get;
        } = new();

        /// <summary>
        /// Gets a list of properties that are discovered during the traversal of the module.
        /// </summary>
        public List<PropertyDefinition> Properties
        {
            get;
        } = new();

        /// <summary>
        /// Gets a list of events that are discovered during the traversal of the module.
        /// </summary>
        public List<EventDefinition> Events
        {
            get;
        } = new();
    }
}
