using ProjetERP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetERP.Repositories
{
    public interface IClientRepository
    {
        Task<Client> CreateClient(Client client);
        Task<Client> GetClientById(int id);
        Task<IEnumerable<Client>> GetAllClients();
        Task UpdateClient(Client client);
        Task DeleteClient(int id);
        Task<ClientCommunication> CreateCommunication(ClientCommunication communication);
        Task<IEnumerable<ClientCommunication>> GetCommunicationsByClientId(int clientId);
        Task<Delivery> CreateDelivery(Delivery delivery);
        Task<IEnumerable<Delivery>> GetDeliveriesByClientId(int clientId);
        Task UpdateDelivery(Delivery delivery);
        Task<Quote> CreateQuote(Quote quote);
        Task<IEnumerable<Quote>> GetQuotesByClientId(int clientId);
        Task UpdateQuote(Quote quote);
    }
}