﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DX.Data.Xpo.Identity.Persistent;
using DX.Utils.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if (NETSTANDARD2_1 || NETCOREAPP)
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

namespace DX.Data.Xpo.Identity
{
#if (NETSTANDARD2_1 || NETCOREAPP)
	public class XPRoleStore<TRole, TXPORole> : XPRoleStore<string, TRole, TXPORole, XpoDxRoleClaim>
		where TRole : class, IXPRole<string>, new()
		where TXPORole : XPBaseObject, IXPRole<string>
#else
    public class XPRoleStore<TRole, TXPORole> : XPRoleStore<string, TRole, TXPORole>
        where TRole : class, IXPRole<string>, IRole<string>, new()
		where TXPORole : XPBaseObject, IXPRole<string>
#endif
	{
		public XPRoleStore(XpoDatabase db, XPDataMapper<string, TRole, TXPORole> mapper, XPDataValidator<string, TRole, TXPORole> validator)
			: base(db, mapper, validator)
		{

		}

		//public XPRoleStore(string connectionName, XPDataMapper<string, TRole, TXPORole> mapper, XPDataValidator<string, TRole, TXPORole> validator) 
		//	: base(connectionName, mapper, validator)
		//{

		//}
	}

#if (NETSTANDARD2_1 || NETCOREAPP)
	public class XPRoleStore<TKey, TRole, TXPORole, TXPORoleClaim> : XPDataStore<TKey, TRole, TXPORole>,
				IQueryableRoleStore<TRole>, IRoleClaimStore<TRole>
		where TKey : IEquatable<TKey>
		where TRole : class, IXPRole<TKey>, new()
		where TXPORole : XPBaseObject, IXPRole<TKey>
		where TXPORoleClaim : XPBaseObject, IXPRoleClaim<TKey>
#else
	public class XPRoleStore<TKey, TRole, TXPORole> : XPDataStore<TKey, TRole, TXPORole>,
    			IQueryableRoleStore<TRole, TKey>
    	where TKey : IEquatable<TKey>
        where TRole : class, IXPRole<TKey>, IRole<TKey>, new()
        where TXPORole : XPBaseObject, IXPRole<TKey>
#endif
	{
		//public XPRoleStore(string connectionName, XPDataMapper<TKey, TRole, TXPORole> mapper, XPDataValidator<TKey, TRole, TXPORole> validator)
		//	: this(new XpoDatabase(connectionName), mapper, validator)
		//{

		//}
		public XPRoleStore(XpoDatabase db, XPDataMapper<TKey, TRole, TXPORole> mapper, XPDataValidator<TKey, TRole, TXPORole> validator)
			: base(db, mapper, validator)
		{

		}


		#region abstract implementation
		public override TKey GetModelKey(TRole model) => model.Id;
		public override void SetModelKey(TRole model, TKey key) => model.Id = key;
		protected override IQueryable<TXPORole> XPQuery(Session s)
		{
			var r = from n in s.Query<TXPORole>()
					select n;
			return r;

		}
        public override IQueryable<TRole> Query() => XPQuery().Select(CreateModelInstance).AsQueryable();

		#endregion

		#region Overrides

		#endregion


		#region Generic Helper methods and members
		protected static Type XPORoleType { get { return typeof(TXPORole); } }
		protected static TXPORole XPOCreateRole(Session s) { return Activator.CreateInstance(typeof(TXPORole), s) as TXPORole; }

#if (NETSTANDARD2_1 || NETCOREAPP)
		protected static Type XPORoleClaimType { get { return typeof(TXPORoleClaim); } }
		protected static TXPORoleClaim XPOCreateRoleClaim(Session s) { return Activator.CreateInstance(typeof(TXPORoleClaim), s) as TXPORoleClaim; }
#endif
		#endregion







		public virtual IQueryable<TRole> Roles
		{
			get
			{
				//TODO: Might need to check this for memoryleak
				var session = DB.GetSession();
				session.Disposed += (s, e) =>
				{
					Trace.WriteLine("IQueryable<TRole> Users session disposed");
				};

				var r = session.Query<TXPORole>().Select(CreateModelInstance);
				return r.AsQueryable();
			}
		}

		public new async Task CreateAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}			
			var result = await base.CreateAsync(role);
		}

		public new async Task DeleteAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			var result = await base.DeleteAsync(role.Id);
		}



		public async virtual Task<TRole> FindByIdAsync(TKey roleId)
		{
			ThrowIfDisposed();
			var result = await base.GetByKeyAsync(roleId);

			return result;
		}

		public async virtual Task<TRole> FindByIdAsync(string roleId)
		{
			ThrowIfDisposed();

			var result = await DB.ExecuteAsync((db, wrk) =>
			 {
				 var xpoRole = wrk.GetObjectByKey(XPORoleType, roleId) as TXPORole;
				 if (xpoRole != null)
				 {
					 TRole r = Mapper.CreateModel(xpoRole);
					 return r;
				 }
				 return null;
			 });
			return result;
		}
		public async Task<TRole> FindByNameAsync(string roleName)
		{
			ThrowIfDisposed();
			if (String.IsNullOrEmpty(roleName))
				throw new ArgumentException("roleName is null or empty");

			var result = await DB.ExecuteAsync((db, wrk) =>
			{
#if (NETSTANDARD2_1 || NETCOREAPP)
				var xpoRole = wrk.FindObject(XPORoleType, CriteriaOperator.Parse("NormalizedName == ?", roleName));
#else
                var xpoRole = wrk.FindObject(XPORoleType, CriteriaOperator.Parse("NameUpper == ?", roleName.ToUpperInvariant()));
#endif
				if (xpoRole != null)
				{
					TRole r = Mapper.CreateModel(xpoRole as TXPORole);
					return r;
				}
				return null;
			});
			return result;
		}
		public new async Task UpdateAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException("roleName");

			var result = await base.UpdateAsync(role);
		}


#if (NETSTANDARD2_1 || NETCOREAPP)
		public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await CreateAsync(role);
			return IdentityResult.Success;
		}
		public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await DeleteAsync(role.Id);
			return IdentityResult.Success;
		}

		public async virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = await FindByIdAsync(roleId);
			return result;
		}

		public async virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = await FindByNameAsync(normalizedRoleName);
			return result;
		}

		public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				await UpdateAsync(role);
			}
			catch (Exception err)
			{
				return IdentityResult.Failed(new IdentityError { Code = "100", Description = err.Message });
			}
			return IdentityResult.Success;
		}
		public virtual string ConvertIdToString(TKey id)
		{
			if (id.Equals(default(TKey)))
			{
				return null;
			}
			return id.ToString();
		}
		public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			return Task.FromResult(ConvertIdToString(role.Id));
		}

		public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			return Task.FromResult(role.Name);

		}

		public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			role.Name = roleName;
			return Task.CompletedTask;
		}

		public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			return Task.FromResult(role.NormalizedName);
		}

		public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			role.NormalizedName = normalizedName;
			return Task.CompletedTask;
		}

		public async virtual Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			var result = await Task.FromResult(DB.Execute<IList<Claim>>((db, wrk) =>
			{
				var results = new List<Claim>();
				foreach (var c in new XPCollection(wrk, XPORoleClaimType, CriteriaOperator.Parse("[Role!Key] == ?", role.Id)))
				{
					TXPORoleClaim xpoClaim = c as TXPORoleClaim;
					results.Add(new Claim(xpoClaim.ClaimType, xpoClaim.ClaimValue));
				}
				return results;
			}, false));
			return result;
		}

		public async virtual Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			if (claim == null)
			{
				throw new ArgumentNullException(nameof(claim));
			}
			await DB.ExecuteAsync((db, wrk) =>
			{
				var xpoClaim = XPOCreateRoleClaim(wrk);
				xpoClaim.SetMemberValue("Role", wrk.GetObjectByKey(XPORoleType, role.Id));
				xpoClaim.ClaimType = claim.Type;
				xpoClaim.ClaimValue = claim.Value;
			});
		}

		public async virtual Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}
			if (claim == null)
			{
				throw new ArgumentNullException(nameof(claim));
			}
			await DB.ExecuteAsync((db, wrk) =>
			{
				wrk.Delete(new XPCollection(wrk, XPORoleClaimType,
						 CriteriaOperator.Parse("([Role!Key] == ?) AND (ClaimType == ?) AND (ClaimValue == ?)",
						 role.Id, claim.Type, claim.Value)));

			});
		}
#endif
	}

}
