﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
#if(NETSTANDARD2_0)
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

namespace DX.Data.Xpo.Identity
{
    public class XPRoleStore<TRole, TXPORole> : XPRoleStore<string, TRole, TXPORole>
		where TRole : XPIdentityRole<string, TXPORole>, IRole<string>
		where TXPORole : XPBaseObject, IDxRole<string>, IRole<string>
	{
		public XPRoleStore() : 
			base() { }
		public XPRoleStore(string connectionName) : 
			base(connectionName) { }
		public XPRoleStore(string connectionString, string connectionName) : 
			base(connectionString, connectionName) { }
		public XPRoleStore(XpoDatabase database) : 
			base(database) { }
    }

#if (NETSTANDARD2_0)
    public class XPRoleStore<TKey, TRole, TXPORole/*, TXPOUser*/> : XpoStore<TXPORole, TKey>,
        IQueryableRoleStore<TRole>
        //IRoleClaimStore<TRole>
        where TKey : IEquatable<TKey>
        where TRole : XPIdentityRole<TKey, TXPORole>, IRole<TKey>
        where TXPORole : XPBaseObject, IDxRole<TKey>, IRole<TKey>
#else
    public class XPRoleStore<TKey, TRole, TXPORole/*, TXPOUser*/> : XpoStore<TXPORole, TKey>,
    	IQueryableRoleStore<TRole, TKey>
    	where TKey : IEquatable<TKey>
    	where TRole : XPIdentityRole<TKey, TXPORole>, IRole<TKey>
    	where TXPORole : XPBaseObject, IDxRole<TKey>, IRole<TKey>
#endif
    {
        public XPRoleStore() :
			base()
		{
            
		}

		public XPRoleStore(string connectionName) :
			base(connectionName)
		{

		}
		public XPRoleStore(string connectionString, string connectionName) :
			base(connectionString, connectionName)
		{

		}

		public XPRoleStore(XpoDatabase database) :
			base(database)
		{

		}

#region Generic Helper methods and members

		//protected static Type XPOUserType { get { return typeof(TXPOUser); } }
		protected static Type XPORoleType { get { return typeof(TXPORole); } }

		//protected static TXPOUser XPOCreateUser(Session s) { return Activator.CreateInstance(typeof(TXPOUser), s) as TXPOUser; }
		protected static TXPORole XPOCreateRole(Session s) { return Activator.CreateInstance(typeof(TXPORole), s) as TXPORole; }
#endregion

		public virtual IQueryable<TRole> Roles
		{
			get
			{
				XPQuery<TXPORole> q = new XPQuery<TXPORole>(GetSession());
				var result = from u in q
							 select Activator.CreateInstance(typeof(TRole), u as TXPORole) as TRole;
				return result;
			}
		}

        public virtual Task CreateAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}

			return Task.FromResult(XPOExecute<object>((db, wrk) =>
			{
				var xpoRole = XPOCreateRole(wrk);
				xpoRole.Assign(role, 0);

				return null;
			}));
		}
        public Task DeleteAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
			{
				throw new ArgumentNullException("role");
			}

			return Task.FromResult(XPOExecute<object>((db, wrk) =>
			{
				wrk.Delete(wrk.FindObject(XPORoleType, CriteriaOperator.Parse($"{KeyField} == ?", role.Id)));
				return null;
			}));
		}

        public virtual Task<TRole> FindByIdAsync(TKey roleId)
		{
			ThrowIfDisposed();

			return Task.FromResult(XPOExecute((db, wrk) =>
			{
				var xpoRole = wrk.FindObject(XPORoleType, CriteriaOperator.Parse($"{KeyField} == ?", roleId));
				return xpoRole == null ? null : Activator.CreateInstance(typeof(TRole), xpoRole, 0) as TRole;
			}));

		}

        public virtual Task<TRole> FindByIdAsync(string roleId)
		{
			ThrowIfDisposed();

			return Task.FromResult(XPOExecute((db, wrk) =>
			{
				var xpoRole = wrk.FindObject(XPORoleType, CriteriaOperator.Parse($"{KeyField} == ?", roleId));
				return xpoRole == null ? null : Activator.CreateInstance(typeof(TRole), xpoRole, 0) as TRole;
			}));
		}
        public Task<TRole> FindByNameAsync(string roleName)
		{
			ThrowIfDisposed();
			if (String.IsNullOrEmpty(roleName))
				throw new ArgumentException("roleName is null or empty");

			return Task.FromResult(XPOExecute((db, wrk) =>
			{
				var xpoRole = wrk.FindObject(XPORoleType, CriteriaOperator.Parse("NameUpper == ?", roleName.ToUpperInvariant()));
				return xpoRole == null ? null : Activator.CreateInstance(typeof(TRole), xpoRole, 0) as TRole;
			}));

		}
        public Task UpdateAsync(TRole role)
		{
			ThrowIfDisposed();
			if (role == null)
				throw new ArgumentNullException("roleName");

			return Task.FromResult(XPOExecute<object>((db, wrk) =>
			{
				TXPORole r = wrk.FindObject(XPORoleType, CriteriaOperator.Parse($"{KeyField} == ?", role.Id)) as TXPORole;
				if (r != null)
				{
					r.Assign(role, 0);
				}
				return null;
			}));
		}
#if (NETSTANDARD2_0)
        public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateAsync(role);
            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await DeleteAsync(role);
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
#endif
    }

}
