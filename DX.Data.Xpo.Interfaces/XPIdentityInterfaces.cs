﻿using System;
using System.Collections;
using System.Security.Claims;

namespace DX.Data.Xpo.Identity
{


#if (NETSTANDARD2_1 || NETCOREAPP)
	public interface IXPUser<TKey>
		 where TKey : IEquatable<TKey>
	{
		TKey Id { get; set; }
		string UserName { get; set; }
		string NormalizedName { get; set; }
		string NormalizedEmail { get; set; }
#else
	public interface IXPUser<TKey> : Microsoft.AspNet.Identity.IUser<TKey>
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

		string RefreshToken { get; set; }
		DateTime? RefreshTokenExpiryTime { get; set; }

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

	}



	public interface IXPUserLogin<TKey>
		where TKey : IEquatable<TKey>
	{
		//Id
		TKey UserId { get; }
		string LoginProvider { get; set; }
		string ProviderKey { get; set; }	
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

		
	}

#if (NETSTANDARD2_1 || NETCOREAPP)
	public interface IXPRole<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey Id { get; set; }
		string Name { get; set; }
		string NormalizedName { get; set; }
#else
	public interface IXPRole<TKey> : Microsoft.AspNet.Identity.IRole<TKey>
		where TKey : IEquatable<TKey>
	{
		new TKey Id { get; set; }
#endif
	}
		public interface IXPRoleClaim<TKey> : IXPBaseClaim<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey RoleId { get; }
	}
}