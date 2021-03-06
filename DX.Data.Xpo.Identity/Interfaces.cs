﻿using DevExpress.Xpo;
using DX.Utils.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
#if (NETSTANDARD2_1)
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

namespace DX.Data.Xpo.Identity
{
	//#if (NETSTANDARD2_1)
	//	public interface IUser<TKey>
	//		 where TKey : IEquatable<TKey>
	//	{
	//		TKey Id { get; }
	//		string UserName { get; set; }
	//		string NormalizedName { get; set; }
	//		string NormalizedEmail { get; set; }
	//	}
	//	public interface IRole<TKey>
	//		where TKey : IEquatable<TKey>
	//	{
	//		TKey Id { get; }
	//		string Name { get; set; }
	//		string NormalizedName { get; set; }
	//	}

	//#endif
	//public interface IXPUser<TKey, TXPOUser, TXPORole>: IXPUser<TKey, TXPOUser >

#if (NETSTANDARD2_1)
	public interface IXPUser<TKey>
		 where TKey : IEquatable<TKey>
	{
		TKey Id { get; set; }
		string UserName { get; set; }
		string NormalizedName { get; set; }
		string NormalizedEmail { get; set; }
#else
	public interface IXPUser<TKey> : IUser<TKey>
		 where TKey : IEquatable<TKey>
	{
		new TKey Id { get; set; }
#endif
		//UserName
		string Email { get; set; }
		bool EmailConfirmed { get; set; }

		string PhoneNumber { get; set; }
		bool PhoneNumberConfirmed { get; set; }

		bool TwoFactorEnabled { get; set; }

		int AccessFailedCount { get; set; }

		bool LockoutEnabled { get; set; }
		DateTime? LockoutEndDateUtc { get; set; }

		string SecurityStamp { get; set; }
		string PasswordHash { get; set; }
		IList RolesList { get; }
		IList ClaimsList { get; }
		IList LoginsList { get; }

		void AssignRoles(IList roles);
		void AssignClaims(IList claims);
		void AssignLogins(IList logins);
		/*
		XPCollection<TXPORole> Roles { get; }
		XPCollection<TXPOUserLogin> Logins { get; }
		XPCollection<TXPOUserToken> Tokens { get; }
		XPCollection<TXPOUserClaim> Claims { get; }
		*/
	}



	public interface IXPUserLogin<TKey> 
		where TKey : IEquatable<TKey>
	{
		//Id
		TKey UserId { get; }
		string LoginProvider { get; set; }
		string ProviderKey { get; set; }

		//TXPOUser User { get; set; }
	}

	public interface IXPBaseClaim<TKey> 
		where TKey : IEquatable<TKey>
	{
		string ClaimType { get; set; }
		string ClaimValue { get; set; }

		Claim ToClaim();

		void InitializeFromClaim(Claim other);
	}
	public interface IXPUserClaim<TKey> : IXPBaseClaim<TKey>
		where TKey : IEquatable<TKey>
	{
		//Id
		TKey UserId { get; }
	}

	public interface IXPUserToken<TKey> 
		where TKey : IEquatable<TKey>
	{
		TKey UserId { get; }
		string LoginProvider { get; set; }
		string Name { get; set; }
		string Value { get; set; }

		//TXPOUser User { get; set; }
	}
#if (NETSTANDARD2_1)
	public interface IXPRole<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey Id { get; set; }
		string Name { get; set; }
		string NormalizedName { get; set; }
#else
	public interface IXPRole<TKey> : IRole<TKey>
		where TKey : IEquatable<TKey>
	{
		new TKey Id { get; set; }
#endif
		//#if (NETSTANDARD2_1)
		//		string NormalizedName { get; set; }
		//#endif
		//		IList UsersList { get; }
		//		XPCollection<TXPOUser> Users { get; }
		//#if (NETSTANDARD2_1)
		//		XPCollection<TXPORoleClaim> Claims { get; }
		//#endif
	}

	public interface IXPRoleClaim<TKey> : IXPBaseClaim<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey RoleId { get; }
	}
}
