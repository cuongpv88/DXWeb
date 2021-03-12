﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DX.Utils.Data
{
	public interface IAssignable
	{
		void Assign(object source);
	}
	//public interface IAssignable<TFrom>
	//{
	//	void Assign(TFrom source);
	//}
	//public interface IDataStoreModel<TKey>
	//	where TKey : IEquatable<TKey>
	//{
	//	//TKey ID { get; set; }
	//}
	public interface IDataStoreMapper<TKey, TModel, TDBModel>
	{

	}

	public interface IDataStoreLookupModel<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey ID { get; set; }
		int Order { get; set; }
		string Name { get; set; }
		string Code { get; set; }
	}
	public interface IDataStoreLookupModel : IDataStoreLookupModel<Guid>
	{

	}
	public interface IDataStoreModel<TKey>
		where TKey : IEquatable<TKey>
	{
		TKey Id { get; set; }
	}
	public interface IDataStore<TKey, TModel>
		where TKey : IEquatable<TKey>
	{
		Type KeyType { get; }
		Type ModelType { get; }

		//IDataMapper<TKey, TModel, TDBModel> GetMapper<TDBModel>()
		//	where TDBModel : class, IDataStoreModel<TKey>;

		//IDataStoreValidator<TKey, TModel> GetValidator();

		//IEnumerable<TModel> Query();
		//Task<IEnumerable<TModel>> QueryAsync();
		TModel GetByKey(TKey key);

		IDataValidationResults<TKey> Create(IEnumerable<TModel> items);
		IDataValidationResults<TKey> Create(TModel item);
		Task<IDataValidationResults<TKey>> CreateAsync(IEnumerable<TModel> items);
		Task<IDataValidationResults<TKey>> CreateAsync(TModel item);

		IDataValidationResults<TKey> Update(IEnumerable<TModel> items);
		IDataValidationResults<TKey> Update(TKey id, TModel item);
		Task<IDataValidationResults<TKey>> UpdateAsync(IEnumerable<TModel> items);
		Task<IDataValidationResults<TKey>> UpdateAsync(TKey id, TModel item);

		IDataValidationResults<TKey> Store(IEnumerable<TModel> items);
		IDataValidationResults<TKey> Store(TKey id, TModel item);
		Task<IDataValidationResults<TKey>> StoreAsync(IEnumerable<TModel> items);
		Task<IDataValidationResults<TKey>> StoreAsync(TKey id, TModel item);

		IDataValidationResults<TKey> Delete(IEnumerable<TKey> ids);
		IDataValidationResults<TKey> Delete(IEnumerable<TModel> items);
		IDataValidationResults<TKey> Delete(TKey id);
		Task<IDataValidationResults<TKey>> DeleteAsync(TKey id);
		Task<IDataValidationResults<TKey>> DeleteAsync(IEnumerable<TKey> ids);
		Task<IDataValidationResults<TKey>> DeleteAsync(IEnumerable<TModel> items);

	}

	public interface ILookupDataStore<TKey, TModel> : IDataStore<TKey, TModel>
		where TKey : IEquatable<TKey>
		where TModel : class, new()
	{

		IEnumerable<IDataStoreLookupModel<TKey>> QueryByName(string name);
		IEnumerable<IDataStoreLookupModel<TKey>> QueryByCode(string code);

		TKey FindKeyByCode(string code);
		TKey FindKeyByName(string name);

	}

	public interface IDataValidationResult<TKey>
	where TKey : IEquatable<TKey>
	{
		DataValidationResultType ResultType { get; set; }
		string FieldName { get; set; }
		string Message { get; set; }
		int Code { get; set; }
		TKey ID { get; set; }

	}

	public interface IDataValidationResults<TKey>
	where TKey : IEquatable<TKey>
	{
		IEnumerable<IDataValidationResult<TKey>> Results { get; }
		void Add(IDataValidationResult<TKey> error);
		void Add(DataValidationResultType resultType, TKey id, string fieldName, string message, int code,
			DataValidationEventType eventType);

		bool Success { get; }

		string[] Messages(params DataValidationResultType[] resultsTypes);

		void AddRange(IEnumerable<IDataValidationResult<TKey>> range);
		void AddRange(IDataValidationResults<TKey> source);
	}

	public interface IDataStoreValidator<TKey, TModel>
		where TKey : IEquatable<TKey>
		where TModel : class
	{
		IDataValidationResults<TKey> Inserting(TModel model, IDataValidationResults<TKey> validationResults);
		IDataValidationResults<TKey> Updating(TKey id, TModel model, IDataValidationResults<TKey> validationResults);
		IDataValidationResults<TKey> Deleting(TKey id, IDataValidationResults<TKey> validationResults, params object[] args);

	}
	public interface IDataStoreValidator<TKey, TModel, TDBModel> : IDataStoreValidator<TKey, TModel>
		where TKey : IEquatable<TKey>
		where TModel : class
		where TDBModel : class
	{
		IDataValidationResults<TKey> Inserted(TKey id, TModel model, TDBModel dbModel, IDataValidationResults<TKey> validationResults);
		IDataValidationResults<TKey> Updated(TKey id, TModel model, TDBModel dbModel, IDataValidationResults<TKey> validationResults);
		IDataValidationResults<TKey> Deleted(TKey id, TDBModel dbModel, IDataValidationResults<TKey> validationResults);
	}

	public interface IDataMapper<TKey, TModel, TDBModel>
		where TKey : IEquatable<TKey>
		where TModel : class, new()
		where TDBModel : class
	{
		Func<TDBModel, TModel> CreateModel { get; }
		TDBModel Assign(TModel source, TDBModel destination);
		string Map(string sourceField);
	}
}
