﻿using System.Security.Permissions;
using AsmResolver.Net.Cts.Collections;
using AsmResolver.Net.Metadata;
using AsmResolver.Net.Signatures;

namespace AsmResolver.Net.Cts
{
    public class SecurityDeclaration : MetadataMember<MetadataRow<ushort,uint,uint>>, IHasCustomAttribute
    {
        private readonly LazyValue<IHasSecurityAttribute> _parent;
        private readonly LazyValue<PermissionSetSignature> _permissionSet;

        public SecurityDeclaration(SecurityAction action, PermissionSetSignature permissionSet)
            : base(null, new MetadataToken(MetadataTokenType.DeclSecurity))
        {
            Action = action;
            _parent = new LazyValue<IHasSecurityAttribute>();
            _permissionSet = new LazyValue<PermissionSetSignature>(permissionSet);
            
            CustomAttributes = new CustomAttributeCollection(this);
        }

        internal SecurityDeclaration(MetadataImage image, MetadataRow<ushort, uint, uint> row)
            : base(image, row.MetadataToken)
        {
            var tableStream = image.Header.GetStream<TableStream>();
            Action = (SecurityAction)row.Column1;

            _parent = new LazyValue<IHasSecurityAttribute>(() =>
            {
                var parentToken = tableStream.GetIndexEncoder(CodedIndex.HasDeclSecurity).DecodeIndex(row.Column2);
                return parentToken.Rid != 0 ? (IHasSecurityAttribute)tableStream.ResolveRow(parentToken) : null;
            });

            _permissionSet = new LazyValue<PermissionSetSignature>(() => 
                PermissionSetSignature.FromReader(image, tableStream.MetadataHeader.GetStream<BlobStream>().CreateBlobReader(row.Column3)));
            
            CustomAttributes = new CustomAttributeCollection(this);
        }

        public IHasSecurityAttribute Parent
        {
            get { return _parent.Value; }
            set { _parent.Value = value; }
        }

        public SecurityAction Action
        {
            get;
            set;
        }

        public PermissionSetSignature PermissionSet
        {
            get { return _permissionSet.Value; }
            set { _permissionSet.Value = value; }
        }

        public CustomAttributeCollection CustomAttributes
        {
            get;
            private set;
        }
    }
}
