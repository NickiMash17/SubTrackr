using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SubTrackr.Core.Interfaces;

namespace SubTrackr.Core.Repositories
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected List<T> _data;
        private readonly Func<T, string> _getIdFunc;

        protected GenericRepository(Func<T, string> getIdFunc)
        {
            _data = new List<T>();
            _getIdFunc = getIdFunc ?? throw new ArgumentNullException(nameof(getIdFunc));
        }

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _data.Add(entity);
        }

        public T GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return _data.FirstOrDefault(e => _getIdFunc(e) == id);
        }

        public List<T> GetAll()
        {
            return _data.ToList();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string id = _getIdFunc(entity);
            var existing = GetById(id);
            if (existing == null)
                throw new InvalidOperationException($"Entity with ID {id} not found");

            int index = _data.IndexOf(existing);
            if (index >= 0)
            {
                _data[index] = entity;
            }
        }

        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return;

            var entity = GetById(id);
            if (entity != null)
            {
                _data.Remove(entity);
            }
        }

        public void SaveToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(_data, options);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            if (!File.Exists(filePath))
            {
                _data = new List<T>();
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _data = new List<T>();
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var deserialized = JsonSerializer.Deserialize<List<T>>(json, options);
                _data = deserialized ?? new List<T>();
            }
            catch (JsonException)
            {
                _data = new List<T>();
            }
        }
    }
}

