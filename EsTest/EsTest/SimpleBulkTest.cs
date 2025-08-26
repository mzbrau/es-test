using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace EsTest
{
    public class SimpleBulkTest
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName = "simple-bulk-test";

        public SimpleBulkTest(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task RunAllTests()
        {
            Console.WriteLine("🚀 Starting simplified bulk operations test...");
            
            try
            {
                await CreateIndex();
                await TestBasicBulkOperations();
                Console.WriteLine("✅ All bulk operations completed successfully!");
            }
            catch (EntryPointNotFoundException ex)
            {
                Console.WriteLine("🚨 EntryPointNotFoundException detected!");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                await CleanupIndex();
            }
        }

        private async Task CreateIndex()
        {
            Console.WriteLine("📝 Creating test index...");
            try
            {
                await _client.Indices.DeleteAsync(_indexName);
                var response = await _client.Indices.CreateAsync(_indexName);
                if (response.IsValidResponse)
                {
                    Console.WriteLine($"✅ Index '{_indexName}' created successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: {ex.Message}");
            }
        }

        private async Task TestBasicBulkOperations()
        {
            Console.WriteLine("🔍 Testing basic bulk operations...");

            var documents = new List<TestDocument>
            {
                new TestDocument { Id = "1", Name = "Document 1", Description = "First test document", Value = 100 },
                new TestDocument { Id = "2", Name = "Document 2", Description = "Second test document", Value = 200 },
                new TestDocument { Id = "3", Name = "Document 3", Description = "Third test document", Value = 300 }
            };

            // Test IndexMany
            var response = await _client.BulkAsync(b => b
                .Index(_indexName)
                .IndexMany(documents, (op, doc) => op.Id(doc.Id))
            );

            if (response.IsValidResponse)
            {
                Console.WriteLine($"✅ Bulk INDEX operation completed - {response.Items.Count} items processed");
            }
            else
            {
                Console.WriteLine($"❌ Bulk operation failed: {response.DebugInformation}");
            }
        }

        private async Task CleanupIndex()
        {
            Console.WriteLine("🧹 Cleaning up...");
            try
            {
                await _client.Indices.DeleteAsync(_indexName);
                Console.WriteLine("✅ Cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Cleanup warning: {ex.Message}");
            }
        }
    }
}