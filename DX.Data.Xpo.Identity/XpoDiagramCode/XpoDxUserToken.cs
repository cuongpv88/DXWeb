﻿using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;



namespace DX.Data.Xpo.Identity.Persistent
{

    public partial class XpoDxUserToken : IXPUserToken<string>
    {
        public XpoDxUserToken(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
		[PersistentAlias("[User.Id]")]
		public string UserId
		{
			get { return (string)(EvaluateAlias("UserId")); }
		}
        string IXPUserToken<string>.UserId
        {
            get => this.UserId;
            set => User = Session.GetObjectByKey<XpoDxUser>(value);
        }

        // Created/Updated: DEV-RIG-DON\don on DEV-RIG-DON at 6/15/2022 3:40 PM
        public new class FieldsClass : XpoDxBase.FieldsClass
        {
            public FieldsClass()
            {

            }

            public FieldsClass(string propertyName) : base(propertyName)
            {

            }

            public const string UserIdFieldName = "UserId";

            public OperandProperty UserId => new OperandProperty(GetNestedName(UserIdFieldName));

            public const string UserFieldName = "User";

            public XpoDxUser.FieldsClass User => new XpoDxUser.FieldsClass(GetNestedName(UserFieldName));

            public const string LoginProviderFieldName = "LoginProvider";

            public OperandProperty LoginProvider => new OperandProperty(GetNestedName(LoginProviderFieldName));

            public const string NameFieldName = "Name";

            public OperandProperty Name => new OperandProperty(GetNestedName(NameFieldName));

            public const string ValueFieldName = "Value";

            public OperandProperty Value => new OperandProperty(GetNestedName(ValueFieldName));
        }

        public new static FieldsClass Fields { get => _Fields; }

        static readonly FieldsClass _Fields = new FieldsClass();


    }

}
