using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scientific.Repositories;

namespace Scientific.Services
{
    public class JournalsMinhPvService : IJournalsMinhPvService
    {
        private readonly Repositories.JournalRepository _journalsMinhPvRepository;

        public JournalsMinhPvService()
            => _journalsMinhPvRepository = new Repositories.JournalRepository();

        public List<Entities.Models.JournalsMinhPv> GetAll() => _journalsMinhPvRepository.GetAll();
        public async Task<List<Entities.Models.JournalsMinhPv>> GetAllAsync() => await _journalsMinhPvRepository.GetAllAsync();
        public void Create(Entities.Models.JournalsMinhPv entity) => _journalsMinhPvRepository.Create(entity);
        public async Task<int> CreateAsync(Entities.Models.JournalsMinhPv entity) => await _journalsMinhPvRepository.CreateAsync(entity);
        public void Update(Entities.Models.JournalsMinhPv entity) => _journalsMinhPvRepository.Update(entity);
        public async Task<int> UpdateAsync(Entities.Models.JournalsMinhPv entity) => await _journalsMinhPvRepository.UpdateAsync(entity);
        public bool Remove(Entities.Models.JournalsMinhPv entity) => _journalsMinhPvRepository.Remove(entity);
        public async Task<bool> RemoveAsync(Entities.Models.JournalsMinhPv entity) => await _journalsMinhPvRepository.RemoveAsync(entity);
        public Entities.Models.JournalsMinhPv GetById(int id) => _journalsMinhPvRepository.GetById(id);
        public async Task<Entities.Models.JournalsMinhPv> GetByIdAsync(int id) => await _journalsMinhPvRepository.GetByIdAsync(id);
        public Entities.Models.JournalsMinhPv GetById(string code) => _journalsMinhPvRepository.GetById(code);
        public async Task<Entities.Models.JournalsMinhPv> GetByIdAsync(string code) => await _journalsMinhPvRepository.GetByIdAsync(code);
        public Entities.Models.JournalsMinhPv GetById(Guid code) => _journalsMinhPvRepository.GetById(code);
        public async Task<Entities.Models.JournalsMinhPv> GetByIdAsync(Guid code) => await _journalsMinhPvRepository.GetByIdAsync(code);

        public async Task<Entities.Models.JournalsMinhPv> GetAllasyn(int id) { 

            try
            {
                return await _journalsMinhPvRepository.GetAllasyn(id);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException("An error occurred while retrieving the journal.", ex);
            }
        }

    }
}
