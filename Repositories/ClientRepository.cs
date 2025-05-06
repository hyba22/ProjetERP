using Microsoft.EntityFrameworkCore;
using ProjetERP.Data;
using ProjetERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetERP.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ProjetERPDbContext _context;

        public ClientRepository(ProjetERPDbContext context)
        {
            _context = context;
        }

        public async Task<Client> CreateClient(Client client)
        {
            try
            {
                Console.WriteLine($"Attempting to create client: Name={client.Name}, Email={client.ContactEmail}");
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Client created successfully: ClientId={client.ClientId}");
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating client: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<Client> GetClientById(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    Console.WriteLine($"No client found with ID {id}");
                }
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching client by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            try
            {
                return await _context.Clients.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all clients: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateClient(Client client)
        {
            try
            {
                var existingClient = await _context.Clients.FindAsync(client.ClientId);
                if (existingClient != null)
                {
                    _context.Clients.Update(client);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Client with ID {client.ClientId} updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Client with ID {client.ClientId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating client: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteClient(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client != null)
                {
                    _context.Clients.Remove(client);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Client with ID {id} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Client with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting client: {ex.Message}");
                throw;
            }
        }

        public async Task<ClientCommunication> CreateCommunication(ClientCommunication communication)
        {
            try
            {
                _context.ClientCommunications.Add(communication);
                await _context.SaveChangesAsync();
                return communication;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating communication: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ClientCommunication>> GetCommunicationsByClientId(int clientId)
        {
            try
            {
                var communications = await _context.ClientCommunications
                    .Where(c => c.ClientId == clientId)
                    .ToListAsync();

                if (communications == null || !communications.Any())
                {
                    Console.WriteLine($"No communications found for client {clientId}");
                }
                return communications;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching communications for client {clientId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Delivery> CreateDelivery(Delivery delivery)
        {
            try
            {
                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();
                return delivery;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating delivery: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Delivery>> GetDeliveriesByClientId(int clientId)
        {
            try
            {
                var deliveries = await _context.Deliveries
                    .Where(d => d.ClientId == clientId)
                    .ToListAsync();

                if (deliveries == null || !deliveries.Any())
                {
                    Console.WriteLine($"No deliveries found for client {clientId}");
                }
                return deliveries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching deliveries for client {clientId}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateDelivery(Delivery delivery)
        {
            try
            {
                _context.Deliveries.Update(delivery);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Delivery with ID {delivery.DeliveryId} updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating delivery: {ex.Message}");
                throw;
            }
        }

        public async Task<Quote> CreateQuote(Quote quote)
        {
            try
            {
                _context.Quotes.Add(quote);
                await _context.SaveChangesAsync();
                return quote;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating quote: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Quote>> GetQuotesByClientId(int clientId)
        {
            try
            {
                var quotes = await _context.Quotes
                    .Where(q => q.ClientId == clientId)
                    .ToListAsync();

                if (quotes == null || !quotes.Any())
                {
                    Console.WriteLine($"No quotes found for client {clientId}");
                }
                return quotes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching quotes for client {clientId}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateQuote(Quote quote)
        {
            try
            {
                _context.Quotes.Update(quote);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Quote with ID {quote.QuoteId} updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating quote: {ex.Message}");
                throw;
            }
        }
    }
}
