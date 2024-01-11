﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DX.Data.Xpo.Identity.Persistent;

using FluentValidation;
using System;
using System.Linq;


namespace DX.Data.Xpo.Identity
{
    
    //public class XPRoleStoreValidator<TKey, TRole, TXPORole> : XPDataValidator<TKey, TRole, TXPORole>
    //	where TKey : IEquatable<TKey>
    //	where TRole : class, new()
    //	where TXPORole : class, IXPSimpleObject, IXPRole<TKey>
    //{

    //	public override IDataValidationResults<TKey> Deleting(TKey id, IDataValidationResults<TKey> validationResults, params object[] args)
    //	{
    //		if (args == null || args.Length == 0)
    //			throw new ArgumentNullException("args[0] should contain reference to XPO entity");
    //		IDataValidationResults<TKey> result = new DataValidationResults<TKey>();
    //		TXPORole role = args[0] as TXPORole;
    //		if (role != null)
    //		{
    //			int userCount = (int)role.Session.Evaluate(typeof(XpoDxUser),
    //				CriteriaOperator.Parse("Count"),
    //				CriteriaOperator.Parse("Roles[Id == ?]", role.Id));
    //			if (userCount > 0)
    //				result.Add(new DataValidationResult<TKey>
    //				{
    //					ResultType = DataValidationResultType.Error,
    //					ID = id,
    //					Message = $"Role '{role.Name}' cannot be deleted because there are users in this Role"
    //				});
    //		}

    //		if (result == null)
    //		{
    //			result = base.Deleting(id, validationResults, args);
    //		}
    //		validationResults.AddRange(result);
    //		return result;
    //	}


    //}
}
