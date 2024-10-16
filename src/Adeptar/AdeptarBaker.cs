using FastMember;
using System;

namespace Adeptar
{
    /// <summary>
    /// Manages <see cref="AdeptarPreBake"/> objects used in serialization.
    /// </summary>
    public class AdeptarBaker
    {
        /// <summary>
        /// Cached <see cref="Type"/> for <see cref="AdeptarConfiguration"/> class.
        /// </summary>
        private static Type _adeptarConfigurationTypeCache = typeof( AdeptarConfiguration );

        /// <summary>
        /// Bakes an <see cref="AdeptarPreBake"/> object for the given class/struct <see cref="Type"/>
        /// and its child class/struct <see cref="Type"/>s. Must be called before serialization.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object.</param>
        /// <param name="obj">The object itself must be passed, even if just empty new().</param>
        /// <returns></returns>
        public static void BakeClassStruct( Type type, object obj )
        {
            AdeptarPreBake bake = new AdeptarPreBake()
            {
                Accessor = TypeAccessor.Create( type )
            };

            bake.Set = bake.Accessor.GetMembers();

            int arrayLen = bake.Set.Count;
            bake.MemberSerializableTypes = new SerializableType[arrayLen];
            bake.DefaultMemberTypes = new Type[arrayLen];
            bake.DefaultValues = new object[arrayLen];

            for ( int i = 0; i < arrayLen; i++ )
            {
                object value = null;

                if ( obj is not null )
                {
                    value = bake.Accessor[obj, bake.Set[i].Name];
                }

                bake.DefaultMemberTypes[i] = bake.Set[i].Type;

                if ( bake.DefaultMemberTypes[i] == _adeptarConfigurationTypeCache )
                {
                    bake.Configuration = value is null ? AdeptarConfiguration.DefaultConfiguration : ( AdeptarConfiguration ) value;
                    bake.HasAdeptarConfiguration = true;
                }

                bake.MemberSerializableTypes[i] = value is null
                    ? TypeGetters.GetSerializableType( bake.DefaultMemberTypes[i] )
                    : TypeGetters.FetchType( value );

                if ( bake.MemberSerializableTypes[i] == SerializableType.Class && !AdeptarWriter.BakedTypes.ContainsKey( bake.DefaultMemberTypes[i] ) )
                {
                    BakeClassStruct( bake.DefaultMemberTypes[i], value );
                }
            }

            if ( bake.Configuration is null )
            {
                bake.Configuration = AdeptarConfiguration.DefaultConfiguration;
                bake.HasAdeptarConfiguration = true;
            }

            AdeptarWriter.BakedTypes.Add( type, bake );
        }

        /// <summary>
        /// Bakes an <see cref="AdeptarPreBake"/> object for the given class/struct <see cref="Type"/>
        /// and its child class/struct <see cref="Type"/>s. Must be called before serialization. This
        /// function is used for testing purposes.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object.</param>
        /// <param name="obj">The object itself must be passed, even if just empty new().</param>
        /// <returns>An integer of 1.</returns>
        public static int IntBakeClassStruct( Type type, object obj )
        {
            AdeptarPreBake bake = new AdeptarPreBake()
            {
                Accessor = TypeAccessor.Create( type )
            };

            bake.Set = bake.Accessor.GetMembers();

            int arrayLen = bake.Set.Count;
            bake.MemberSerializableTypes = new SerializableType[arrayLen];
            bake.DefaultMemberTypes = new Type[arrayLen];
            bake.DefaultValues = new object[arrayLen];

            for ( int i = 0; i < arrayLen; i++ )
            {
                object value = null;

                if ( obj is not null )
                {
                    value = bake.Accessor[obj, bake.Set[i].Name];
                }

                bake.DefaultMemberTypes[i] = bake.Set[i].Type;

                if ( bake.DefaultMemberTypes[i] == _adeptarConfigurationTypeCache )
                {
                    bake.Configuration = value is null ? AdeptarConfiguration.DefaultConfiguration : ( AdeptarConfiguration ) value;
                    bake.HasAdeptarConfiguration = true;
                }

                bake.MemberSerializableTypes[i] = value is null
                    ? TypeGetters.GetSerializableType( bake.DefaultMemberTypes[i] )
                    : TypeGetters.FetchType( value );

                if ( bake.MemberSerializableTypes[i] == SerializableType.Class && !AdeptarWriter.BakedTypes.ContainsKey( bake.DefaultMemberTypes[i] ) )
                {
                    BakeClassStruct( bake.DefaultMemberTypes[i], value );
                }
            }

            if ( bake.Configuration is null )
            {
                bake.Configuration = AdeptarConfiguration.DefaultConfiguration;
                bake.HasAdeptarConfiguration = true;
            }

            AdeptarWriter.BakedTypes.Add( type, bake );
            return 1;
        }
    }
}