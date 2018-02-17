﻿
using System;
using AsmResolver.Net.Cts.Collections;
using AsmResolver.Net.Metadata;

namespace AsmResolver.Net.Cts
{
    public class TypeReference : MetadataMember<MetadataRow<uint, uint, uint>>, ITypeDefOrRef, IResolutionScope
    {
        private readonly LazyValue<string> _name;
        private readonly LazyValue<string> _namespace;
        private string _fullName;

        public TypeReference(IResolutionScope resolutionScope, string @namespace, string name)
            : base(null, new MetadataToken(MetadataTokenType.TypeRef))
        {
            ResolutionScope = resolutionScope;
            _namespace = new LazyValue<string>(@namespace);
            _name = new LazyValue<string>(name);
            
            CustomAttributes = new CustomAttributeCollection(this);
        }

        internal TypeReference(MetadataImage image, MetadataRow<uint, uint, uint> row)
            : base(image, row.MetadataToken)
        {
            var stringStream = image.Header.GetStream<StringStream>();
            var tableStream = image.Header.GetStream<TableStream>();

            var resolutionScopeToken = tableStream.GetIndexEncoder(CodedIndex.ResolutionScope).DecodeIndex(row.Column1);
            if (resolutionScopeToken.Rid != 0)
            {
                IMetadataMember resolutionScope;
                if (image.TryResolveMember(resolutionScopeToken, out resolutionScope))
                    ResolutionScope = resolutionScope as IResolutionScope;
            }

            _name = new LazyValue<string>(() => stringStream.GetStringByOffset(row.Column2));
            _namespace = new LazyValue<string>(() => stringStream.GetStringByOffset(row.Column3));
            
            CustomAttributes = new CustomAttributeCollection(this);
        }

        public ITypeDefOrRef DeclaringType
        {
            get { return ResolutionScope as ITypeDefOrRef; }
        }

        ITypeDescriptor ITypeDescriptor.DeclaringTypeDescriptor
        {
            get { return DeclaringType; }
        }

        public IResolutionScope ResolutionScope
        {
            get;
            set;
        }

        public string Name
        {
            get { return _name.Value; }
            set
            {
                _name.Value = value;
                _fullName = null;
            }
        }

        public string Namespace
        {
            get { return _namespace.Value; }
            set
            {
                _namespace.Value = value;
                _fullName = null;
            }
        }

        public virtual string FullName
        {
            get
            {
                if (_fullName != null)
                    return _fullName;
                if (DeclaringType != null)
                    return _fullName = DeclaringType.FullName + '+' + Name;
                return _fullName = string.IsNullOrEmpty(Namespace) ? Name : Namespace + "." + Name;
            }
        }

        public bool IsValueType
        {
            get
            {
                var definition = Resolve();
                return definition != null && definition.IsValueType;
            }
        }
        
        public ITypeDescriptor GetElementType()
        {
            return this;
        }

        public CustomAttributeCollection CustomAttributes
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return FullName;
        }

        public TypeDefinition Resolve()
        {
            if (Image == null || Image.MetadataResolver == null)
                throw new MemberResolutionException(this);
            return Image.MetadataResolver.ResolveType(this);
        }

        IMetadataMember IResolvable.Resolve()
        {
            return Resolve();
        }
    }
}
