using System.Collections.Generic;

namespace SubTrackr.Core.Interfaces
{
	public interface IRepository<T> where T : class
	{
		void Add(T entity);
		T GetById(string id);
		List<T> GetAll();
		void Update(T entity);
		void Delete(string id);
		void SaveToFile(string filePath);
		void LoadFromFile(string filePath);
	}
}