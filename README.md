# es-test
A test application to investigate the Elasticsearch library bulk operations issue with .NET Framework 4.8

## Overview

This test application was created to investigate an `EntryPointNotFoundException` that occurs when calling async bulk operations with the Elasticsearch client library on .NET Framework 4.8.

### Original Issue

The reported error was:
```
FailureReason: Unrecoverable/Unexpected BadRequest while attempting POST on http://localhost:9200/_bulk?pretty=true
 - [1] BadRequest: Node: http://localhost:9200/ Exception: EntryPointNotFoundException Took: 00:00:00.0412934
# Audit exception in step 1 BadRequest:
System.EntryPointNotFoundException: Entry point was not found.
   at System.IAsyncDisposable.DisposeAsync()
   at Elastic.Clients.Elasticsearch.Core.Bulk.BulkIndexOperation`1.<SerializeAsync>d__29.MoveNext() in /_/src/Elastic.Clients.Elasticsearch/_Shared/Types/Core/Bulk/BulkIndexOperation.cs:line 81
```

## Project Structure

- **EsTest.csproj**: Converted from .NET Framework 4.8 to .NET 8 for build compatibility
- **Program.cs**: Main test application with comprehensive bulk operations testing
- **SimpleBulkTest.cs**: Simplified bulk operations test class

## Features

The test application includes:

1. **Elasticsearch Client Initialization**: Connection setup with proper configuration
2. **Index Management**: Automatic creation and cleanup of test indexes
3. **Comprehensive Bulk Operations Testing**:
   - Bulk INDEX operations (inserting/updating documents)
   - Bulk CREATE operations (creating new documents)
   - Mixed bulk operations (combining index, create, and delete in single request)
4. **Error Handling**: Specific handling for `EntryPointNotFoundException`
5. **Detailed Console Output**: Progress reporting and operation results
6. **Self-contained**: Generates test data and manages cleanup

## Running the Tests

### Prerequisites

1. Elasticsearch server running on `http://localhost:9200` (optional - the app will continue without it)
2. .NET 8 SDK installed

### Execute Tests

```bash
cd EsTest/EsTest
dotnet run
```

### Expected Output

The application will:
1. Display startup information and framework compatibility notes
2. Initialize the Elasticsearch client and test connectivity
3. Run comprehensive bulk operations tests
4. Run simplified bulk operations tests
5. Report results and any exceptions encountered
6. Provide a summary of findings

## Framework Compatibility Notes

- **Original Issue**: Occurred on .NET Framework 4.8
- **Current Implementation**: Converted to .NET 8 for environment compatibility
- **Testing Strategy**: If no `EntryPointNotFoundException` occurs on .NET 8, it suggests the issue is specific to .NET Framework 4.8

## Understanding the Issue

The `EntryPointNotFoundException` in the original issue appears to be related to:
1. .NET Framework 4.8's limited support for `IAsyncDisposable` interface
2. The Elasticsearch client library's use of async disposal patterns
3. Potential compatibility issues between the library version and .NET Framework 4.8

## Investigating Further

To properly test with .NET Framework 4.8:
1. Set up an environment with .NET Framework 4.8 SDK
2. Revert the project file to target .NET Framework 4.8
3. Run the same test suite to reproduce the original issue

## Test Operations

The application tests these bulk operations:
- **IndexMany**: Bulk indexing of multiple documents
- **CreateMany**: Bulk creation of new documents  
- **Mixed Operations**: Combining index, create, and delete operations
- **Error Scenarios**: Proper exception handling and reporting

Each operation is wrapped with comprehensive error handling to catch and report the specific `EntryPointNotFoundException` mentioned in the original issue.
