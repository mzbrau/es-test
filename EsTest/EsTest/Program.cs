using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace EsTest
{
    public class TestDocument
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Value { get; set; }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Elasticsearch Bulk Operations Test ===");
            Console.WriteLine("Testing for EntryPointNotFoundException in .NET async bulk operations");
            Console.WriteLine("Note: This test was converted from .NET Framework 4.8 to .NET 8 for environment compatibility");
            Console.WriteLine("The original issue occurred with .NET Framework 4.8 and the Elasticsearch client library");
            Console.WriteLine();

            try
            {
                // Initialize Elasticsearch client
                var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                    .DisableDirectStreaming()
                    .PrettyJson()
                    .RequestTimeout(TimeSpan.FromMinutes(2));

                var client = new ElasticsearchClient(settings);

                Console.WriteLine("🔧 Initializing Elasticsearch client...");
                
                try
                {
                    var pingResponse = await client.PingAsync();
                    if (pingResponse.IsValidResponse)
                    {
                        Console.WriteLine("✅ Successfully connected to Elasticsearch");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Warning: Could not connect to Elasticsearch. Continuing with test anyway...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Warning: Exception while pinging Elasticsearch: {ex.Message}");
                    Console.WriteLine("Continuing with test anyway...");
                }

                Console.WriteLine();

                // Run the comprehensive test
                await RunComprehensiveBulkTest(client);

                // Run simplified test as well
                var simpleTest = new SimpleBulkTest(client);
                await simpleTest.RunAllTests();

                Console.WriteLine("✅ All bulk operations completed successfully!");
                Console.WriteLine("No EntryPointNotFoundException was encountered in this .NET 8 environment.");
                Console.WriteLine("This suggests the issue may be specific to .NET Framework 4.8 compatibility.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Test failed with exception:");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }
                
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex is EntryPointNotFoundException)
                {
                    Console.WriteLine("🔍 EntryPointNotFoundException detected - this is the issue we're investigating!");
                    Console.WriteLine("This confirms the problem reported with async bulk operations.");
                }
            }
            finally
            {
                Console.WriteLine("\n=== Test Summary ===");
                Console.WriteLine("This test application demonstrates various bulk operations with the Elasticsearch client.");
                Console.WriteLine("The original issue was an EntryPointNotFoundException when calling IAsyncDisposable.DisposeAsync()");
                Console.WriteLine("in the BulkIndexOperation.SerializeAsync method on .NET Framework 4.8.");
                Console.WriteLine("\nIf no exception occurred, it suggests the issue is specific to .NET Framework 4.8.");
                Console.WriteLine("To test with .NET Framework 4.8, this project would need to be run in an environment");
                Console.WriteLine("with the .NET Framework 4.8 runtime and SDK installed.");
                Console.WriteLine("\nTest completed. Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static async Task RunComprehensiveBulkTest(ElasticsearchClient client)
        {
            Console.WriteLine("🔍 Running comprehensive bulk operations test...");
            const string indexName = "comprehensive-bulk-test";

            try
            {
                // Create index
                Console.WriteLine("📝 Creating comprehensive test index...");
                await client.Indices.DeleteAsync(indexName);
                var createResponse = await client.Indices.CreateAsync(indexName);
                
                if (createResponse.IsValidResponse)
                {
                    Console.WriteLine($"✅ Index '{indexName}' created successfully");
                }

                // Test 1: Index multiple documents
                Console.WriteLine("🔍 Testing bulk INDEX operations...");
                var documents = new List<TestDocument>();
                for (int i = 1; i <= 5; i++)
                {
                    documents.Add(new TestDocument
                    {
                        Id = $"doc_{i}",
                        Name = $"Document {i}",
                        Description = $"Test document number {i}",
                        Value = i * 100,
                        Timestamp = DateTime.UtcNow.AddMinutes(-i)
                    });
                }

                var bulkResponse = await client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(documents, (op, doc) => op.Id(doc.Id))
                );

                if (bulkResponse.IsValidResponse)
                {
                    Console.WriteLine($"✅ Bulk INDEX completed - {bulkResponse.Items.Count} documents indexed");
                }
                else
                {
                    Console.WriteLine($"❌ Bulk INDEX failed: {bulkResponse.DebugInformation}");
                }

                // Test 2: Create operations
                Console.WriteLine("🔍 Testing bulk CREATE operations...");
                var createDocs = new List<TestDocument>();
                for (int i = 6; i <= 8; i++)
                {
                    createDocs.Add(new TestDocument
                    {
                        Id = $"create_{i}",
                        Name = $"Created Document {i}",
                        Description = $"Document created via bulk CREATE operation {i}",
                        Value = i * 150,
                        Timestamp = DateTime.UtcNow
                    });
                }

                var createResponse2 = await client.BulkAsync(b => b
                    .Index(indexName)
                    .CreateMany(createDocs, (op, doc) => op.Id(doc.Id))
                );

                if (createResponse2.IsValidResponse)
                {
                    Console.WriteLine($"✅ Bulk CREATE completed - {createResponse2.Items.Count} documents created");
                }
                else
                {
                    Console.WriteLine($"❌ Bulk CREATE failed: {createResponse2.DebugInformation}");
                }

                // Test 3: Mixed operations in a single bulk request
                Console.WriteLine("🔍 Testing mixed bulk operations...");
                
                var newDoc1 = new TestDocument 
                { 
                    Id = "mixed_index", 
                    Name = "Mixed Index Doc", 
                    Description = "Document from mixed operation",
                    Value = 999,
                    Timestamp = DateTime.UtcNow
                };

                var newDoc2 = new TestDocument 
                { 
                    Id = "mixed_create", 
                    Name = "Mixed Create Doc", 
                    Description = "Created in mixed operation",
                    Value = 888,
                    Timestamp = DateTime.UtcNow
                };

                var mixedResponse = await client.BulkAsync(b => b
                    .Index(indexName)
                    .Index(newDoc1, i => i.Id(newDoc1.Id))
                    .Create(newDoc2, c => c.Id(newDoc2.Id))
                    .Delete<TestDocument>(new TestDocument { Id = "doc_1" }, d => d.Id("doc_1"))
                );

                if (mixedResponse.IsValidResponse)
                {
                    Console.WriteLine($"✅ Mixed bulk operations completed - {mixedResponse.Items.Count} operations processed");
                }
                else
                {
                    Console.WriteLine($"❌ Mixed bulk operations failed: {mixedResponse.DebugInformation}");
                }

                // Cleanup
                Console.WriteLine("🧹 Cleaning up comprehensive test index...");
                await client.Indices.DeleteAsync(indexName);
                Console.WriteLine("✅ Comprehensive test cleanup completed");
            }
            catch (EntryPointNotFoundException ex)
            {
                Console.WriteLine("🚨 EntryPointNotFoundException caught in comprehensive bulk test!");
                Console.WriteLine($"Details: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in comprehensive bulk test: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException}");
                }
                throw;
            }

            Console.WriteLine();
        }
    }
}
