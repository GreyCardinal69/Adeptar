using FastMember;
using System;

namespace Adeptar
{
    /// <summary>
    /// Represents an image of a given class/struct <see cref="Type"/>, with its reflection and Adeptar
    /// properties stored beforehand for faster serialization.
    /// </summary>
    internal class AdeptarPreBake
    {
        /// <summary>
        /// The <see cref="TypeAccessor"/> of the <see cref="Type"/> to bake.
        /// </summary>
        public TypeAccessor Accessor;
        /// <summary>
        /// The <see cref="MemberSet"/> of the <see cref="Type"/> to bake, taken from the <see cref="TypeAccessor"/>.
        /// </summary>
        public MemberSet Set;
        /// <summary>
        /// Is set to True if the <see cref="Type"/> has a defined <see cref="AdeptarConfiguration"/> defined.
        /// </summary>
        public bool HasAdeptarConfiguration;
        /// <summary>
        /// The <see cref="AdeptarConfiguration"/> of the object. If the <see cref="Type"/> has one defined, it is set
        /// to the defined, if not <see cref="AdeptarConfiguration.DefaultConfiguration"/> is used.
        /// </summary>
        public AdeptarConfiguration Configuration;
        /// <summary>
        /// The <see cref="Type"/>s of the object's fields and properties.
        /// </summary>
        public Type[] DefaultMemberTypes;
        /// <summary>
        /// The <see cref="SerializableType"/>s of the object's fields and properties.
        /// </summary>
        public SerializableType[] MemberSerializableTypes;
        /// <summary>
        /// The baked default values of each of the object's fields' and properties' <see cref="Type"/>s.
        /// </summary>
        public object[] DefaultValues;
    }
}